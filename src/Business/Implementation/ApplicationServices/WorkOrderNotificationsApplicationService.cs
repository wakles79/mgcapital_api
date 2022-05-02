using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PushNotifications;
using MGCap.Domain.ViewModels.WorkOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class WorkOrderNotificationsApplicationService : IWorkOrderNotificationsApplicationService
    {
        private readonly List<WorkOrderNotificationTemplate> Templates;
        private readonly IEmailSender EmailSender;
        private readonly ISmsSender SmsSender;
        private readonly IWorkOrdersRepository WORepository;
        private readonly IWorkOrderNotificationTemplatesRepository Repository;
        private readonly IPushNotificationService PushNotificationService;
        private readonly IEmailActivityLogRepository EmailActivityLogRepository;

        /// <summary>
        /// Gets <value>IHttpContextAccessor field</value>
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor { get; }

        // TODO: DRY these properties
        /// <summary>
        /// Gets the <value>strCompanyId</value> of the current User
        /// </summary>
        /// 
        public string StrCompanyId { get; set; }
        /// <summary>
        ///     Gets the <value>UserEmail</value> of the current User
        /// </summary>
        public string UserEmail { get; set; }



        public string AppBaseUrl { get; set; }

        public WorkOrderNotificationsApplicationService(
            IWorkOrderNotificationTemplatesRepository repository,
            IWorkOrdersRepository woRepository,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IPushNotificationService pushNotificationService,
            IEmailActivityLogRepository emailActivityLogRepository,
            IHttpContextAccessor httpContextAccessor,
            IOptions<OneSignalOptions> options)
        {
            this.Repository = repository;
            this.WORepository = woRepository;
            this.EmailSender = emailSender;
            this.SmsSender = smsSender;
            // Storing in memory since we have few records
            this.Templates = this.Repository.Entities.ToList();

            this.PushNotificationService = pushNotificationService;

            this.HttpContextAccessor = httpContextAccessor;

            this.StrCompanyId = this.HttpContextAccessor?.HttpContext?.Request?.Headers["CompanyId"];
            this.UserEmail = this.HttpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";

            this._strTimezoneOffset = this.HttpContextAccessor?.HttpContext?.Request?.Headers["TimezoneOffset"];

            this.AppBaseUrl = $"{this.HttpContextAccessor.HttpContext.Request.Scheme}://{this.HttpContextAccessor.HttpContext.Request.Host}/";

            this.EmailActivityLogRepository = emailActivityLogRepository;
        }


        /// <summary>
        /// Gets the <value>CompanyId</value> of the current User
        /// </summary>
        // HACK: Using default existing company
        public int CompanyId => string.IsNullOrEmpty(this.StrCompanyId) ? 1 : int.Parse(this.StrCompanyId);

        private readonly string _strTimezoneOffset;
        /// <summary>
        ///     Gets the client's UTC Offset in minutes
        /// </summary>
        public string StrTimezoneOffset => string.IsNullOrEmpty(this._strTimezoneOffset) ? "300" : this._strTimezoneOffset;

        public int TimezonOffset => (int)Math.Round(double.Parse(this.StrTimezoneOffset));

        public async Task SendNotificationsAsync(WorkOrder wo)
        {
            try
            {
                var contacts = await this.WORepository.GetWOContactsDapperAsync(wo);
                if (contacts == null)
                {
                    return;
                }

                // Gets details about WO
                var woDetails = await this.WORepository.GetWODetailsDapperAsync(wo, this.CompanyId, this.UserEmail);
                woDetails.StatusId = wo.StatusId;
                var externalStatus = WorkOrderUtils.GetExternalStatusName((WorkOrderStatus)wo.StatusId);

                if (woDetails == null || string.IsNullOrEmpty(woDetails.BuildingName) || wo.DueDate == null)
                {
                    return;
                }
                // Sorts contacts by priority
                contacts = contacts.OrderBy(c => c.WorkOrderContactType.Priority);

                // Dict to store seen contacts
                var seen = new HashSet<string>();
                var pushNotificationRecipients = new List<WorkOrderContactViewModel>();

                foreach (var contact in contacts)
                {
                    try
                    {
                        // Cleanse emails addresses
                        contact.Email = contact.Email.ToLower().ReplaceWhitespace();

                        // Skip contact if email was 'analyzed' before and such contact is not a requester
                        if (seen.Contains(contact.Email) && contact.WorkOrderContactType.Key != WorkOrderContactType.Requester.Key)
                        {
                            continue;
                        }
                        

                        // Add to push notification users
                        if (contact.WorkOrderContactType.Key.Equals(WorkOrderContactType.OfficeStaff.Key) ||
                            contact.WorkOrderContactType.Key.Equals(WorkOrderContactType.OperationsManager.Key) ||
                            contact.WorkOrderContactType.Key.Equals(WorkOrderContactType.Supervisor.Key))
                        {
                            pushNotificationRecipients.Add(contact);
                            seen.Add(contact.Email);
                        }

                        if (contact.SendNotifications == false)
                        {
                            continue;
                        }

                        // If contact is 'Owner' and wo has 'Do not send owner notifications'
                        // HACK: we are not sending notifications to building owner for now
                        bool ownerFlag = contact.WorkOrderContactType.Key == WorkOrderContactType.BuildingOwner.Key;
                        // If contact is 'Property Manager' and wo has 'Do not send property managers notifications'
                        bool managerFlag = contact.WorkOrderContactType.Key == WorkOrderContactType.PropertyManager.Key && !wo.SendPropertyManagersNotifications;
                        // If contact is 'Requester' and wo has 'Do not send requester'
                        bool requesterFlag = contact.WorkOrderContactType.Key == WorkOrderContactType.Requester.Key && !wo.SendRequesterNotifications;

                        if (ownerFlag || managerFlag || requesterFlag)
                        {
                            continue;
                        }
                        seen.Add(contact.Email);

                        // If email was previously analyzed or doesn't exist any templates we skip contact
                        var filter = new Func<WorkOrderNotificationTemplate, bool>(
                            t => t.Type.Equals(NotificationType.Email) &&
                            t.WorkOrderStatusId == (int)wo.StatusId &&
                            t.WorkOrderContactTypeId == contact.WorkOrderContactType.ID);

                        var template = this.Templates.FirstOrDefault(filter);
                        if (template == null)
                        {
                            continue;
                        }
                        var htmlEmail = GetContentFromTemplate(template.RichtextBodyTemplate, woDetails);
                        var plainEmail = GetContentFromTemplate(template.PlainTextTemplate, woDetails);

                        // Safety Check
                        if (string.IsNullOrEmpty(htmlEmail) && string.IsNullOrEmpty(plainEmail))
                        {
                            continue;
                        }
                        string subject = WorkOrderUtils.GetSubject(
                                                contact.WorkOrderContactType,
                                                (WorkOrderType)wo.Type,
                                                woDetails.WONumber,
                                                externalStatus,
                                                woDetails.BuildingName);
                        //string subject = $"{(WorkOrderType)wo.Type} #{woDetails.WONumber} [{externalStatus}]";
                        string source = $@"<h3>INTERNAL INFO <small>(DEBUG ONLY)</small></h3> <br> 
                                        TO_FULLNAME: {contact.FullName} <br> TO_EMAIL: {contact.Email} <br> TO_TYPE: {contact.WorkOrderContactType} <br> 
                                        WO_TYPE: {(WorkOrderType)wo.Type} <br> WO_STATUS: {(WorkOrderStatus)wo.StatusId} <br> DATE: {wo.UpdatedDate.ToLocalTime().ToString("MM/dd/yyyy H:mm:ss zzz")}";

                        var email = contact.Email;

                        // Use debug info only for "external contacts"
                        bool isExternal = false;
                        if (WorkOrderUtils.ExternalContacts.Any(c => c.Key == contact.WorkOrderContactType.Key))
                        {
                            isExternal = true;
                            // HACK: Just to know the source in body
                            var debugHtmlEmail = $"{source}<br>{htmlEmail}";
                            var debugPlainEmail = $"{source}\n{plainEmail}";
                            var debugEmail = WorkOrderUtils.GetDebugEmail(contact.WorkOrderContactType.Key);

                            bool sendBeforeExternal = await this.EmailActivityLogRepository.ExistsDapperAsync(this.CompanyId, subject, email, htmlEmail);
                            // bool sendBeforeExternal = EmailActivityLogRepository.Exists(e => e.CompanyId == this.CompanyId &&
                            //                                                 e.Subject == subject &&
                            //                                                 e.SentTo == email &&
                            //                                                 e.Body == htmlEmail);
                            if (!sendBeforeExternal)
                            {
                                await this.EmailSender.SendEmailAsync(
                                    debugEmail,
                                    subject,
                                    plainTextMessage: debugPlainEmail,
                                    htmlMessage: debugHtmlEmail);

                                // Register Activity
                                EmailActivityLogRepository.Add(new EmailActivityLog
                                {
                                    CompanyId = this.CompanyId,
                                    Subject = subject,
                                    SentTo = debugEmail,
                                    Body = htmlEmail,
                                    SharedUrl = string.Empty,
                                    ReferenceNumber = $"{wo.Number}",
                                });
                            }
                        }
                        bool sendBefore = await this.EmailActivityLogRepository.ExistsDapperAsync(this.CompanyId, subject, email, htmlEmail);
                        // bool sendBefore = EmailActivityLogRepository.Exists(e => e.CompanyId == this.CompanyId &&
                        //                                                     e.Subject == subject &&
                        //                                                     e.SentTo == email &&
                        //                                                     e.Body == htmlEmail);
                        if (!sendBefore)
                        {
                            await this.EmailSender.SendEmailAsync(
                                email,
                                subject,
                                plainTextMessage: plainEmail,
                                htmlMessage: htmlEmail,
                                isExternal: isExternal);

                            // Register Activity
                            EmailActivityLogRepository.Add(new EmailActivityLog
                            {
                                CompanyId = this.CompanyId,
                                Subject = subject,
                                SentTo = email,
                                Body = htmlEmail,
                                SharedUrl = string.Empty,
                                ReferenceNumber = $"{wo.Number}",
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.EmailSender.SendEmailAsync("axzesllc@gmail.com", "Error Parsing Contacts", plainTextMessage: ex.Message);
                    }
                }

                var pushNotificationFilter = new Func<WorkOrderNotificationTemplate, bool>(
                                                t => t.Type.Equals(NotificationType.PushNotification) &&
                                                t.WorkOrderStatusId == (int)wo.StatusId);

                var paramViewModel = new PushNotificationCreateViewModel
                {
                    WorkOrder = woDetails,
                    Recipients = pushNotificationRecipients,
                    Templates = this.Templates.Where(pushNotificationFilter)
                };

                await this.PushNotificationService.CreateNotification(paramViewModel);
            }
            catch (OneSignalException osEx)
            {
                string plainTextMessage = JsonConvert.SerializeObject(osEx.InnerException);
                await this.EmailSender.SendEmailAsync("axzesllc@gmail.com", osEx.Message, plainTextMessage: plainTextMessage);
            }
            catch (SqlException sEx)
            {
                string plainTextMessage = JsonConvert.SerializeObject(sEx);
                await this.EmailSender.SendEmailAsync("axzesllc@gmail.com", sEx.Message, plainTextMessage: plainTextMessage);
            }
            catch (Exception ex)
            {
                await this.EmailSender.SendEmailAsync("axzesllc@gmail.com", "Error Sending Emails", plainTextMessage: ex.Message);
            }
        }

        private string GetContentFromTemplate(string template, WorkOrderEmailDetailsViewModel vm)
        {
            if (string.IsNullOrEmpty(template) || vm == null)
            {
                return string.Empty;
            }

            var result = template.Replace("[WONumber]", vm.WONumber.ToString())
                                 .Replace("[BuildingName]", vm.BuildingName)
                                 .Replace("[AssignedFullName]", vm.AssignedFullName)
                                 .Replace("[Description]", vm.Description)
                                 .Replace("[RequesterEmail]", vm.RequesterEmail)
                                 .Replace("[RequesterFullName]", vm.RequesterFullName)
                                 .Replace("[WOFullUrl]", $"{this.AppBaseUrl}work-orders/{vm.Guid}") // TODO: Use a constant
                                 .Replace("[ExternalWOFullUrl]", $"{this.AppBaseUrl}external-work-orders/{vm.Guid}") // TODO: Use a constant
                                 .Replace("[EmployeeWhoClosedWO]", vm.EmployeeWhoClosedWO)
                                 .Replace("[Sig]", Signature);
            return result;
        }

        private string GetLinkContentFromTemplate(string template, WorkOrderWithExpirationViewModel vm)
        {
            if (string.IsNullOrEmpty(template) || vm == null)
            {
                return string.Empty;
            }

            var result = template.Replace("[WONumber]", vm.Number.ToString())
                                 .Replace("[BuildingName]", vm.BuildingName)
                                 .Replace("[WOFullUrl]", $"{this.AppBaseUrl}work-orders/{vm.Guid}");
            return result;
        }

        private async Task<WorkOrderSummaryViewModel> SendSummaryNotificationAsync(WorkOrderEmployeeContactViewModel contact, IEnumerable<WorkOrderWithExpirationViewModel> workOrders)
        {
            var result = new WorkOrderSummaryViewModel();
            // Filters only WO that belongs to "employee"
            var contactWorkOrders = workOrders.Where(wo => wo.EmployeeIds.Contains(contact.EmployeeId));
            if (!contactWorkOrders.Any())
            {
                // Sending Emails letting them know that all is on time
                await this.EmailSender.SendEmailAsync(
                        contact.Email,
                        "Work Orders Summary",
                        plainTextMessage: "Nothing due!",
                        htmlMessage: "Nothing due!");

                // Register Activity
                EmailActivityLogRepository.Add(new EmailActivityLog()
                {
                    CompanyId = this.CompanyId,
                    Subject = "Work Orders Summary",
                    SentTo = contact.Email,
                    Body = "Nothing due!",
                    SharedUrl = string.Empty,
                    Cc = new List<EmailLogEntry>()
                });

                return result;
            }
            // Build email body
            var dueTodayList = contactWorkOrders.Where(wo => wo.DueToday == 1);
            var pastDueList = contactWorkOrders.Where(wo => wo.IsExpired == 1);

            string emailTemplate = string.Empty;
            string innerLinks = string.Empty;

            // Due Today
            if (dueTodayList.Any())
            {
                emailTemplate = WorkOrdersDueTodayEmailTemplate;
                foreach (var due in dueTodayList)
                {
                    innerLinks += this.GetLinkContentFromTemplate(WorkOrderUrlTemplate, due);
                }

                // var contacts = await this.WORepository.GetSupervisorsAndOperationsManagersDapperAsync(this.CompanyId);

                emailTemplate = emailTemplate.Replace("[WOLinks]", innerLinks);
            }

            // Past Due
            if (pastDueList.Any())
            {
                innerLinks = string.Empty;
                emailTemplate += WorkOrdersPastDueEmailTemplate;
                foreach (var past in pastDueList)
                {
                    innerLinks += this.GetLinkContentFromTemplate(WorkOrderUrlTemplate, past);
                }

                emailTemplate = emailTemplate.Replace("[WOLinks]", innerLinks);
            }

            emailTemplate += $"<p>Thank you,<br />{Signature}</p>";

            result.DueTodayTotal = dueTodayList.Count();
            result.PastDueTotal = pastDueList.Count();

            await this.EmailSender.SendEmailAsync(
                        contact.Email,
                        "Work Orders Summary",
                        plainTextMessage: emailTemplate,
                        htmlMessage: emailTemplate);

            // Register Activity
            EmailActivityLogRepository.Add(new EmailActivityLog()
            {
                CompanyId = this.CompanyId,
                Subject = "Work Orders Summary",
                SentTo = contact.Email,
                Body = emailTemplate,
                SharedUrl = innerLinks,
                Cc = new List<EmailLogEntry>()
            });

            return result;
        }

        public async Task<WorkOrderSummaryViewModel> SendSummaryNotificationsAsync()
        {
            var result = new WorkOrderSummaryViewModel();
            try
            {
                var woList = await this.WORepository.GetWorkOrderWithExpirationDapperAsync(this.CompanyId, this.TimezonOffset);

                if (woList == null || !woList.Any())
                {
                    return result;
                }

                var templates = this.Templates.Where(t => t.Type.Equals(NotificationType.ScheduledPushNotification));

                await this.PushNotificationService.CreateScheduledNotifications(templates);

                var contacts = await this.WORepository.GetSupervisorsAndOperationsManagersDapperAsync(this.CompanyId);

                if (contacts == null || !contacts.Any())
                {
                    return result;
                }

                // Send Emails
                // Dict to store seen employees
                var seen = new HashSet<int>();
                var innerResult = new WorkOrderSummaryViewModel();

                foreach (var contact in contacts)
                {
                    if (seen.Contains(contact.EmployeeId))
                    {
                        continue;
                    }
                    seen.Add(contact.EmployeeId);

                    innerResult = await this.SendSummaryNotificationAsync(contact, woList);
                    result.DueTodayTotal += innerResult.DueTodayTotal;
                    result.PastDueTotal += innerResult.PastDueTotal;
                }


            }
            catch (Exception ex)
            {
                await this.EmailSender.SendEmailAsync(
                            "axzesllc@gmail.com",
                            "Error Sending Emails (Supervisors and Operations Managers)",
                            plainTextMessage: ex.Message);
            }
            return result;
        }

        #region Signature
        // Don't continue beyond this point
        public static string Signature = @"
            <small>MG Capital Maintenance Inc, Customer Service Department</small>
            <br>
            <small>110 Pheasant Wood Ct Suite D Morrisville, NC. &nbsp;27560</small>
            <br>
            <small>C:</small><small>919 337 2304 &nbsp;</small>
            <small>O:</small><small>&nbsp;919 461 8573 &nbsp;</small>
            <small>F</small><small>: 919 467 0837</small>
            <small>Hours</small><small>:Mon-Fri, 8am-5pm </small>
            <br>
            <small>
            Print this email only if you must. &nbsp;MG Capital &nbsp;is a Member of the US Green Building Council.
            </small>
            <br>
            <img src='http://mgcapitalmain.com/wordpress/wp-content/uploads/2018/07/img1.jpg' alt='Logo 1'>
            <img src='http://mgcapitalmain.com/wordpress/wp-content/uploads/2018/07/img2.jpg' alt='Logo 2'>
        ";
        #endregion


        #region Summary
        // Don't see this code
        public static string WorkOrderUrlTemplate = "<li><a href='[WOFullUrl]'>[BuildingName]-[WONumber]</a></li>";

        public static string WorkOrdersDueTodayEmailTemplate = @"
            <p>The following Work Orders are due today:<br /><br /><ul>[WOLinks]</ul><br /><br /></p>
        ";

        public static string WorkOrdersPastDueEmailTemplate = @"
            <p>The following Work Orders are currently Past Due:<br /><br /><ul>[WOLinks]</ul><br /><br /></p>
        ";
        #endregion
    }
}

