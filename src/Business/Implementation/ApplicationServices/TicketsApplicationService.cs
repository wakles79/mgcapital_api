using AutoMapper;
using Dapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Freshdesk;
using MGCap.Domain.ViewModels.PushNotifications;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.TicketActivityLog;
using MGCap.Domain.ViewModels.CompanySettings;
//using MGCap.Domain.ViewModels.TicketStatusLog; // MG-15
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Auth.OAuth2;
using MGCap.Domain.ViewModels.GMailApi;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class TicketsApplicationService : BaseEntityWithAttachmentsService<Ticket, TicketAttachment, int>, ITicketsApplicationService
    {
        private readonly IEmployeesRepository EmployeesRepository;

        private readonly ICustomerUserRepository CustomerUserRepository;

        private readonly IBuildingsRepository BuildingsRepository;

        private readonly IPushNotificationService PushNotificationService;

        private readonly IEmailSender EmailSender;

        private readonly IEmailActivityLogRepository EmailActivityLogRepository;

        private readonly IInspectionItemTicketsRepository InspectionItemTicketsRepository;

        private readonly IInspectionsApplicationService InspectionsApplicationService;

        private readonly ITicketActivityLogRepository TicketActivityLogRepository;

        private readonly ICleaningReportItemsRepository CleaningReportItemsRepository;

        private readonly ITagRepository TagsRepository;

        private readonly IWorkOrdersRepository WorkOrdersRepository;

        private readonly ITicketEmailHistoryRepository EmailHistoryRepository;

        private readonly IFreshdeskApplicationService FreshdeskApplicationService;

        private readonly IGMailApiService GMailApiService;

        private readonly IConvertedTicketsRepository ConvertedTicketsRepository;

        public ITicketAttachmentRepository TicketAttachmentRepository { get; private set; }

        private readonly IMapper Mapper; // MG-15

        private readonly ICompanySettingsApplicationService CompanySettingsService;

        private bool GmailEnabled { get; set; }

        private ulong LastHistoryId { get; set; }

        private readonly CompanySettingsDetailViewModel CompanySettings;

        public TicketsApplicationService(
            IAzureStorage azureStorage,
            ITicketsRepository repository,
            IBaseDapperRepository baseDapperRepository,
            ITicketAttachmentRepository ticketAttachmentRepository,
            IEmployeesRepository employeesRepository,
            ICustomerUserRepository customerUserRepository,
            IBuildingsRepository buildingsRepository,
            IPushNotificationService pushNotificationService,
            IEmailActivityLogRepository emailActivityLogRepository,
            IEmailSender emailSender,
            IInspectionItemTicketsRepository inspectionItemTicketsRepository,
            IInspectionsApplicationService inspectionsApplicationService,
            ITicketActivityLogRepository ticketActivityLogRepository,
            ICleaningReportItemsRepository cleaningReportItemsRepository,
            ITagRepository tagsRepository,
            IWorkOrdersRepository workOrdersRepository,
            IHttpContextAccessor httpContextAccessor,
            IFreshdeskApplicationService freshdeskApplicationService,
            IGMailApiService gmailApiService,
            IConvertedTicketsRepository convertedTicketsRepository,
            ICompanySettingsApplicationService companySettingsService,
            IMapper mapper,
            ITicketEmailHistoryRepository emailHistoryRepository) : base(azureStorage, repository, ticketAttachmentRepository, httpContextAccessor)
        {
            this.BaseDapperRepository = baseDapperRepository;
            this.EmployeesRepository = employeesRepository;
            this.CustomerUserRepository = customerUserRepository;

            BuildingsRepository = buildingsRepository;
            PushNotificationService = pushNotificationService;
            EmailSender = emailSender;
            this.EmailActivityLogRepository = emailActivityLogRepository;
            this.InspectionItemTicketsRepository = inspectionItemTicketsRepository;
            this.InspectionsApplicationService = inspectionsApplicationService;
            this.TicketActivityLogRepository = ticketActivityLogRepository;
            this.CleaningReportItemsRepository = cleaningReportItemsRepository;
            this.TagsRepository = tagsRepository;
            this.WorkOrdersRepository = workOrdersRepository;

            this.FreshdeskApplicationService = freshdeskApplicationService;
            this.GMailApiService = gmailApiService;
            this.ConvertedTicketsRepository = convertedTicketsRepository;
            this.CompanySettingsService = companySettingsService;
            var settings = this.CompanySettingsService.GetCompanySettings();
            this.CompanySettings = settings.Result;
            //this.GMailApiService.UserEmail = this.CompanySettings.GmailNotificationsEmail;
            this.EmailHistoryRepository = emailHistoryRepository;
        }

        private IBaseDapperRepository BaseDapperRepository { get; set; }

        public new ITicketsRepository Repository => base.Repository as ITicketsRepository;

        public new ITicketAttachmentRepository AttachmentsRepository => base.AttachmentsRepository as ITicketAttachmentRepository;


        #region Overrides

        public async Task<Ticket> AddAsync(Ticket obj, bool sendNotifications = true)
        {
            if (obj.UserId.HasValue && (string.IsNullOrEmpty(obj.RequesterEmail) || string.IsNullOrEmpty(obj.RequesterFullName)))
            {
                switch (obj.UserType)
                {
                    case UserType.Employee:
                        var employee = await EmployeesRepository.SingleOrDefaultDapperAsync(obj.UserId.Value, this.CompanyId);
                        obj.RequesterFullName = employee.FullName;
                        obj.RequesterEmail = employee.Email;
                        break;
                    case UserType.Customer:
                        var customerUser = await CustomerUserRepository.SingleOrDefaultByIdDapperAsync(obj.UserId.Value, this.CompanyId);
                        obj.RequesterFullName = customerUser.FullName;
                        obj.RequesterEmail = customerUser.Email;
                        break;
                }
            }

            obj = await base.AddAsync(obj);
            await SaveChangesAsync(true, false);
            var result = await this.GetTicketDetailsDapperAsync(obj.ID);
            obj.Number = result.Number;

            var recipients = await EmployeesRepository.ReadAllOfficeStaffOrMastersDapperAsync(this.CompanyId);

            if (sendNotifications)
            {
                // create push notification
                var pushNotificationVM = new PushNotificationTicketCreateViewModel
                {
                    Requester = obj.RequesterFullName,
                    TicketNumber = obj.Number,
                    BuildingName = BuildingsRepository.SingleOrDefault(obj.BuildingId ?? 0)?.Name ?? "",
                    Recipients = recipients?.Select(c => c.Email)?.ToList()
                };

                // DO NOT AWAIT THIS TAKS, NEVER NEVER NEVER
                PushNotificationService.CreateNewTicketNotification(pushNotificationVM);

                // If Ticket was created from an entity outside MG capital
                if (obj.Source == TicketSource.WorkOrderForm)
                {
                    await this.SendTicketCreationEmailAsync(
                        obj, recipients.Where(c => c.SendNotifications).Select(c => c.Email)
                        );
                }
            }
            return obj;
        }

        public async Task<Ticket> AddAsyncExternal(Ticket obj, bool sendNotifications = true)
        {
            if (obj.UserId.HasValue && (string.IsNullOrEmpty(obj.RequesterEmail) || string.IsNullOrEmpty(obj.RequesterFullName)))
            {
                switch (obj.UserType)
                {
                    case UserType.Employee:
                        var employee = await EmployeesRepository.SingleOrDefaultDapperAsync(obj.UserId.Value, this.CompanyId);
                        obj.RequesterFullName = employee.FullName;
                        obj.RequesterEmail = employee.Email;
                        break;
                    case UserType.Customer:
                        var customerUser = await CustomerUserRepository.SingleOrDefaultByIdDapperAsync(obj.UserId.Value, this.CompanyId);
                        obj.RequesterFullName = customerUser.FullName;
                        obj.RequesterEmail = customerUser.Email;
                        break;
                }
            }

            obj = await base.AddAsync(obj);
            await SaveChangesAsyncExternal();
            var result = await this.GetTicketDetailsDapperAsync(obj.ID);
            obj.Number = result.Number;

            var recipients = await EmployeesRepository.ReadAllOfficeStaffOrMastersDapperAsync(this.CompanyId);

            if (sendNotifications)
            {
                // create push notification
                var pushNotificationVM = new PushNotificationTicketCreateViewModel
                {
                    Requester = obj.RequesterFullName,
                    TicketNumber = obj.Number,
                    BuildingName = BuildingsRepository.SingleOrDefault(obj.BuildingId ?? 0)?.Name ?? "",
                    Recipients = recipients?.Select(c => c.Email)?.ToList()
                };

                // DO NOT AWAIT THIS TAKS, NEVER NEVER NEVER
                PushNotificationService.CreateNewTicketNotification(pushNotificationVM);

                // If Ticket was created from an entity outside MG capital
                if (obj.Source == TicketSource.WorkOrderForm)
                {
                    await this.SendTicketCreationEmailAsync(
                        obj, recipients.Where(c => c.SendNotifications).Select(c => c.Email)
                        );
                }
            }
            return obj;
        }

        /// <summary>
        /// Sends a notification to all office staffs and masters about the ticket creation.
        /// </summary>
        /// <returns></returns>
        /// <param name="obj">Object.</param>
        /// <param name="recipients">Recipients.</param>
        private async Task SendTicketCreationEmailAsync(Ticket obj, IEnumerable<string> recipients)
        {
            if (recipients?.Any() == false)
            {
                return;
            }

            obj.Data.TryGetValue("Location", out string location);

            string body = $@"
                <p>A new Ticket has been generated through {obj.Source.ToString().SplitCamelCase()} needs your attention.</p> 
                <br/>
                <p>Ticket # {obj.Number}</p>
                <br/>
                <p>Name: {obj.RequesterFullName}</p>
                <p>Email: {obj.RequesterEmail}</p>
                <p>Phone: {obj.RequesterPhone}</p>
                <br/>
                <p>Location: {location}</p>
                <p>Description: {obj.Description}</p>
            ";

            foreach (var email in recipients)
            {
                try
                {
                    bool sendBeforeExternal = await this.EmailActivityLogRepository.ExistsDapperAsync(this.CompanyId, $"New Ticket # {obj.Number}", email, body);
                    // bool sendBeforeExternal = EmailActivityLogRepository.Exists(e => e.CompanyId == this.CompanyId &&
                    //                                                         e.Subject == $"New Ticket # {obj.Number}" &&
                    //                                                         e.SentTo == email &&
                    //                                                         e.Body == body);
                    if (!sendBeforeExternal)
                    {
                        await this.EmailSender.SendEmailAsync(
                        email,
                        $"New Ticket # {obj.Number}",
                        plainTextMessage: body,
                        htmlMessage: body);

                        // Register Activity
                        await EmailActivityLogRepository.AddAsync(new EmailActivityLog()
                        {
                            CompanyId = this.CompanyId,
                            Subject = $"New Ticket # {obj.Number}",
                            SentTo = email,
                            Body = body,
                            SharedUrl = string.Empty,
                            Cc = new List<EmailLogEntry>()
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Nothing
                }
            }
        }

        #endregion

        public async Task<DataSource<TicketGridViewModel>> ReadAllDapperAsync(DataSourceRequestTicket request)
        {
            int? employeeId = null;
            if (request.OnlyAssigned)
            {
                // Gets logged employee's id
                employeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            }
            return await this.Repository.ReadAllDapperAsync(request, this.CompanyId, employeeId);
        }

        public async Task<Ticket> UpdateEntityReferenceAsync(Ticket obj, bool saveActivity = true, bool sequenceConverted = false, string textLog = "")
        {
            // TODO: Improve this
            var tableNames = new Dictionary<TicketDestinationType, string>
            {
                [TicketDestinationType.WorkOrder] = "WorkOrders",
                [TicketDestinationType.CleaningItem] = "CleaningReportItems",
                [TicketDestinationType.FindingItem] = "CleaningReportItems",

            };
            if (tableNames.TryGetValue(obj.DestinationType, out string column))
            {
                try
                {
                    string query = $"SELECT TOP 1 ID FROM {column} WHERE ID = @entityId";
                    var pars = new DynamicParameters();
                    pars.Add("@entityId", obj.DestinationEntityId);

                    var result = await this.BaseDapperRepository.QuerySingleOrDefaultAsync<int>(query, pars);

                    if (result == 0)
                    {
                        throw new NotSupportedException($"Destination {obj.DestinationType} with ID {obj.DestinationEntityId} doesn't exist.");
                    }

                    // Register 
                    ConvertedTicket convertedTicket = new ConvertedTicket()
                    {
                        TicketId = obj.ID,
                        DestinationType = obj.DestinationType,
                        ConvertedDate = DateTime.Now,
                        DestinationEntityId = obj.DestinationEntityId.Value
                    };
                    await this.ConvertedTicketsRepository.AddAsync(convertedTicket);

                    // Add extra info
                    if (obj.DestinationType == TicketDestinationType.FindingItem
                        || obj.DestinationType == TicketDestinationType.CleaningItem)
                    {
                        string cleaningQuery = @"SELECT TOP 1 C.ID 
                                                FROM CleaningReports AS C 
                                                    INNER JOIN CleaningReportItems AS CI ON CI.CleaningReportId = C.ID
                                                WHERE CI.ID = @entityId";

                        result = await this.BaseDapperRepository.QuerySingleOrDefaultAsync<int>(cleaningQuery, pars);

                        if (result == 0)
                        {
                            throw new NotSupportedException($"CLeaning Report doesn't exist.");
                        }

                        if (obj.Data == null)
                        {
                            obj.Data = new Dictionary<string, string>();
                        }

                        obj.Data["cleaningReportId"] = result.ToString();

                    }

                    var inspectionItemTicketReference = await this.InspectionItemTicketsRepository.SingleOrDefaultAsync(i => i.TicketId == obj.ID);
                    if (inspectionItemTicketReference != null)
                    {
                        inspectionItemTicketReference.DestinationType = obj.DestinationType;
                        inspectionItemTicketReference.entityId = obj.DestinationEntityId;
                        await this.InspectionItemTicketsRepository.UpdateAsync(inspectionItemTicketReference);

                        // Close the inspection item if the destination is equals to Cleaning Item
                        if (inspectionItemTicketReference.DestinationType == TicketDestinationType.CleaningItem || inspectionItemTicketReference.DestinationType == TicketDestinationType.FindingItem)
                        {
                            //var inspectionItem = await this.InspectionItemsRepository.SingleOrDefaultAsync(i => i.ID == inspectionItemTicketReference.InspectionItemId);                            
                            //    inspectionItem.Status = (int)InspectionItemStatus.Closed;
                            //    await this.InspectionItemsRepository.UpdateAsync(inspectionItem);

                            //// validate inspection items
                            //var items = await this.InspectionItemsRepository.ReadAllByInspectionDapperAsync(new DataSourceRequest() { PageSize = 999 }, inspectionItem.InspectionId);
                            //int completedItems = items.Payload.Count(i => i.Status == InspectionItemStatus.Closed);
                            //if (completedItems == items.Payload.Count(i => i.ID != inspectionItem.ID))
                            //{
                            //    var inspection = await this.InspectionsRepository.SingleOrDefaultAsync(i => i.ID == inspectionItem.ID);
                            //    inspection.Status = InspectionStatus.Closed;
                            //    await this.InspectionsRepository.UpdateAsync(inspection);
                            //}

                            var item = await this.InspectionsApplicationService.CloseInspectionItemAsync(inspectionItemTicketReference.InspectionItemId);
                        }
                    }

                    // Get current employee
                    var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

                    if (saveActivity)
                    {
                        // Create Activity
                        var changeSummary = new TicketChangeSummaryEntry()
                        {
                            Title = "Ticket Converted"
                        };
                        if (obj.DestinationType == TicketDestinationType.WorkOrder)
                        {
                            var workOrder = await this.WorkOrdersRepository.SingleOrDefaultAsync(obj.DestinationEntityId.Value);
                            if (sequenceConverted)
                            {

                                changeSummary.EntityId = workOrder.WorkOrderScheduleSettingId.Value;
                                changeSummary.Summary = string.IsNullOrEmpty(textLog) ? $"Ticket converted to Work Order sequence" : textLog;
                            }
                            else
                            {

                                changeSummary.EntityId = obj.DestinationEntityId.Value;
                                changeSummary.Summary = $"Ticket converted to <a href=' ((link)) ' target='_blank'>Work Order #{workOrder.Number}</a>.";
                            }
                            changeSummary.EntityType = TicketReferenceEntityType.WorkOrder;
                        }
                        else if (obj.DestinationType == TicketDestinationType.CleaningItem)
                        {
                            var cleaningItem = await this.CleaningReportItemsRepository.SingleOrDefaultAsync(obj.DestinationEntityId.Value);
                            changeSummary.Summary = "Ticket converted to <a href=' ((link)) ' target='_blank'>Cleaning Item</a>.";
                            changeSummary.EntityType = TicketReferenceEntityType.CleaningReportItem;
                            changeSummary.EntityId = cleaningItem.CleaningReportId;
                        }
                        else if (obj.DestinationType == TicketDestinationType.FindingItem)
                        {
                            var cleaningItem = await this.CleaningReportItemsRepository.SingleOrDefaultAsync(obj.DestinationEntityId.Value);
                            changeSummary.Summary = "Ticket converted to <a href=' ((link)) ' target='_blank'>Finding Item</a>.";
                            changeSummary.EntityType = TicketReferenceEntityType.CleaningReportItem;
                            changeSummary.EntityId = cleaningItem.CleaningReportId;
                        }
                        else if (obj.DestinationType == TicketDestinationType.Undefined)
                        {
                            changeSummary.Summary = "Ticket converted to Undefined";
                            changeSummary.EntityType = TicketReferenceEntityType.None;
                            changeSummary.EntityId = 0;
                        }
                        // Register Activity
                        TicketActivityLog newLog = new TicketActivityLog()
                        {
                            EmployeeId = employeeId,
                            TicketId = obj.ID,
                            ActivityType = sequenceConverted ? TicketActivityType.TicketConvertedWorkOrderSequence : TicketActivityType.TicketConverted,
                            ChangeLog = new List<ChangeLogEntry>(),
                            ChangeSummary = new List<TicketChangeSummaryEntry>() { changeSummary }
                        };
                        await this.TicketActivityLogRepository.AddAsync(newLog);
                    }

                    return await this.UpdateAsync(obj);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            throw new NotSupportedException($"Destination {obj.DestinationType} doesn't match with any entity.");
        }

        public async Task<TicketDetailsViewModel> GetTicketDetailsDapperAsync(int id = -1, Guid? guid = null)
        {
            var result = await this.Repository.GetTicketDetailsDapperAsync(id, guid);
            if (result != null)
            {
                result.Tags = await this.TagsRepository.ReadAllTicketTags(result.ID);
            }
            return result;
        }

        public Task<DataSource<TicketToMergeGridViewModel>> ReadAllToMergeAsync(int paramType, string value)
        {
            return this.Repository.ReadAllToMergeAsync(this.CompanyId, paramType, value);
        }

        public async Task MergeTicketsAsync(TicketMergeViewModel mergeViewModel)
        {
            // get primary ticket
            var parentTicket = await this.Repository.SingleOrDefaultAsync(mergeViewModel.TicketId);

            // validate if exist
            if (parentTicket == null)
            {
                throw new Exception("The primary ticket is invalid");
            }

            // get tickets that will be merged
            var tickets = new List<Ticket>();
            foreach (var ticketId in mergeViewModel.TicketsId)
            {
                var ticketToMerge = await this.Repository.SingleOrDefaultAsync(ticketId);
                if (ticketToMerge != null)
                {
                    tickets.Add(ticketToMerge);
                }
            }

            if (tickets.Count() != mergeViewModel.TicketsId.Count())
            {
                throw new Exception("Missing tickets to be merged");
            }

            // get current employee
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

            var parentSummary = new List<TicketChangeSummaryEntry>();
            var activityLog = new List<TicketActivityLog>();

            foreach (var ticket in tickets)
            {
                ticket.Status = TicketStatus.Resolved;
                ticket.ParentId = parentTicket.ID;

                if (ticket.FreshdeskTicketId.HasValue)
                {
                    await this.FreshdeskApplicationService.UpdateTicketStatus(ticket.ID, ticket.Status);
                }

                // Register activity on children ticket
                var changeSummary = new TicketChangeSummaryEntry()
                {
                    Title = "Ticket Merged",
                    Summary = $"This ticket is resolved and merged into ticket Number <a href=' ((link)) ' target='_blank'> {parentTicket.Number} </a>",
                    EntityType = TicketReferenceEntityType.Ticket,
                    EntityId = parentTicket.ID
                };

                activityLog.Add(new TicketActivityLog()
                {
                    EmployeeId = employeeId,
                    TicketId = ticket.ID,
                    ActivityType = TicketActivityType.TicketMerged,
                    ChangeLog = new List<ChangeLogEntry>(),
                    ChangeSummary = new List<TicketChangeSummaryEntry>() { changeSummary }
                });

                // Register summary activity on parent ticket
                var parentChangeSummary = new TicketChangeSummaryEntry()
                {
                    Title = "Ticket Merged",
                    Summary = $"Ticket with Number <a href=' ((link)) ' target='_blank'> {ticket.Number} </a> is merged into this ticket.",
                    EntityType = TicketReferenceEntityType.Ticket,
                    EntityId = ticket.ID
                };
                parentSummary.Add(parentChangeSummary);
            }

            TicketActivityLog newLog = new TicketActivityLog()
            {
                EmployeeId = employeeId,
                TicketId = parentTicket.ID,
                ActivityType = TicketActivityType.TicketMerged,
                ChangeLog = new List<ChangeLogEntry>(),
                ChangeSummary = parentSummary
            };
            activityLog.Add(newLog);

            await this.Repository.UpdateRangeAsync(tickets);
            await this.TicketActivityLogRepository.AddRangeAsync(activityLog);
        }

        public async Task ForwardTicket(TicketForwardViewModel vm)
        {
            Ticket ticket = await this.Repository.SingleOrDefaultAsync(vm.TicketId);
            if (ticket == null)
            {
                throw new Exception("Missing Ticket");
            }

            TicketReplyViewModel reply = JsonConvert.DeserializeObject<TicketReplyViewModel>(vm.StrReply);

            string signature = await this.FreshdeskApplicationService.GetAgentEmailSignature(); // FreshDesk
            int divSpaces = 5;
            int index = reply.Body.IndexOf("class=\"quoted-text\"");
            index = index - divSpaces;
            // FreshDesk
            if (index >= 0)
            {
                reply.Body = reply.Body.Insert(index, $"<br>{signature}<br>");
            }
            else
            {
                reply.Body += $"<br>{signature}";
            }

            Dictionary<string, byte[]> attachments = new Dictionary<string, byte[]>();
            if (vm.Attachments != null)
            {
                foreach (IFormFile attachment in vm.Attachments)
                {
                    MemoryStream ms = new MemoryStream();
                    attachment.CopyTo(ms);
                    attachments.Add(attachment.FileName, ms.ToArray());
                    ms.Close();
                }
            }

            string fromDisplay = string.Empty;
            // Gets logged employee's id
            var employeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);

            if (employeeId > 0)
            {
                var employee = await this.EmployeesRepository.SingleOrDefaultDapperAsync(employeeId, this.CompanyId);
                if (employee != null)
                {
                    // Brace yourself, a lot of null-conditional operators are coming
                    fromDisplay = employee
                                    ?.FullName
                                    ?.RemoveDuplicatedSpaces();
                }
            }

            // GMail
            //this.GMailApiService.SendEmail(vm.To, "test@mgcapitalmain.com", $"MgCap Ticket #{ticket.Number}", reply.Body, true, reply.CcEmails, reply.BccEmails);


            await this.EmailSender.SendEmailAsyncWithAttachments(
                vm.To,
                $"MgCap Ticket #{ticket.Number}",
                plainTextMessage: reply.Body,
                htmlMessage: reply.Body,
                fromDisplay: fromDisplay,
                replyTo: this.UserEmail,
                cc: reply.CcEmails,
                attachments: attachments
            );

            try
            {
                string ccText = reply.CcEmails.Count() > 0 ? string.Join("<br/>", reply.CcEmails) : "";

                string summary = $"To: {vm.To} <br/> CC: {ccText} <br/><br/> {reply.Body}";

                TicketActivityLog newLog = new TicketActivityLog()
                {
                    EmployeeId = employeeId,
                    TicketId = vm.TicketId,
                    ActivityType = TicketActivityType.Forwarded,
                    ChangeLog = new List<ChangeLogEntry>(),
                    ChangeSummary = new List<TicketChangeSummaryEntry>() {
                    new TicketChangeSummaryEntry()
                    {
                        Title = $"{fromDisplay} forwarded",
                        Summary = summary,
                        EntityType = TicketReferenceEntityType.None,
                        EntityId = 0
                    }
                }
                };
                await this.TicketActivityLogRepository.AddAsync(newLog);
            }
            catch (Exception ex)
            {
                // Nothing
            }
        }

        public async Task UpdateAssignedTicketEmployee(int ticketId, int employeeId)
        {
            var ticket = await this.Repository.SingleOrDefaultAsync(ticketId);
            if (ticket != null)
            {
                ticket.AssignedEmployeeId = employeeId;

                await this.Repository.UpdateAsync(ticket);
                await this.RegisterEmployeeAssignment(ticketId, employeeId);
            }
        }

        public async Task<bool> AssignCurrentEmployeeToTicket(int ticketId)
        {
            var ticket = await this.Repository.SingleOrDefaultAsync(ticketId);
            if (ticket != null)
            {
                if (!ticket.AssignedEmployeeId.HasValue)
                {
                    // Gets logged employee's id
                    var loggedEmployeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
                    if (loggedEmployeeId > 0)
                    {
                        ticket.AssignedEmployeeId = loggedEmployeeId;
                        await this.Repository.UpdateAsync(ticket);
                        await this.RegisterEmployeeAssignment(ticketId, loggedEmployeeId);

                        return true;
                    }
                }
            }
            return false;
        }

        public async Task RegisterEmployeeAssignment(int ticketId, int employeeId)
        {
            // Gets logged employee's id
            var loggedEmployeeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);

            var employee = await this.EmployeesRepository.SingleOrDefaultDapperAsync(employeeId, this.CompanyId);
            if (employee != null)
            {
                TicketActivityLog newLog = new TicketActivityLog()
                {
                    EmployeeId = loggedEmployeeId,
                    TicketId = ticketId,
                    ActivityType = TicketActivityType.AssignedEmployee,
                    ChangeLog = new List<ChangeLogEntry>(),
                    ChangeSummary = new List<TicketChangeSummaryEntry>() {
                    new TicketChangeSummaryEntry()
                    {
                        Title = $"Assigned Ticket",
                        Summary = $"This ticket was assigned to {employee?.FullName?.RemoveDuplicatedSpaces()}",
                        EntityType = employee== null ? TicketReferenceEntityType.None : TicketReferenceEntityType.Employee,
                        EntityId = employee== null ? 0 : employee.ID
                    }
                }
                };
                await this.TicketActivityLogRepository.AddAsync(newLog);
            }
        }

        #region Attachments

        public async Task<DataSource<TicketAttachmentBaseViewModel>> ReadAllAttachemntsAsync(DataSourceRequest request, int ticketId)
        {
            return await this.AttachmentsRepository.ReadAllDapperAsync(request, ticketId);
        }

        public Task<int> GetPendingTicketsCountDapperAsync()
        {
            return this.Repository.GetPendingTicketsCountDapperAsync(this.CompanyId);
        }

        public async Task<TicketAttachment> AddTicketAttachmentAsync(TicketAttachmentCreateViewModel vm)
        {
            Stream stream = vm.File.OpenReadStream();
            var addedFileResult = await this.AzureStorage.UploadImageAsync(stream, contentType: vm.File.ContentType);
            stream.Close();
            TicketAttachment ticketAttachment = new TicketAttachment()
            {
                BlobName = addedFileResult.Item1,
                Description = vm.File.FileName,
                FullUrl = addedFileResult.Item2,
                TicketId = vm.TicketId,
                ID = 0
            };

            var result = await this.AttachmentsRepository.AddAsync(ticketAttachment);
            return result;
        }
        #endregion Attachments

        #region Inspection Item
        public async Task<bool> CloseReferencedInspectionItem(int ticketId)
        {
            try
            {
                var inspectionItemTicketReference = await this.InspectionItemTicketsRepository.SingleOrDefaultAsync(i => i.TicketId == ticketId);
                if (inspectionItemTicketReference != null)
                {
                    //var inspectionItem = await this.InspectionItemsRepository.SingleOrDefaultAsync(i => i.ID == inspectionItemTicketReference.InspectionItemId);
                    //if (inspectionItem != null)
                    //{
                    //    inspectionItem.Status = (int)InspectionItemStatus.Closed;
                    //    await this.InspectionItemsRepository.UpdateAsync(inspectionItem);
                    //}

                    var item = await this.InspectionsApplicationService.CloseInspectionItemAsync(inspectionItemTicketReference.InspectionItemId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region External
        public Task<Ticket> AddExternalAsync(TicketFreshdeskCreateViewModel ticketVm)
        {
            // If Gmail is not enabled, process Freshdesk Ticket.
            if (!this.CompanySettings.GmailEnabled)
            {
                var ticket = new Ticket()
                {
                    ID = 0,
                    Source = TicketSource.Email,
                    Status = TicketStatus.Draft,
                    DestinationType = TicketDestinationType.Undefined,
                    Description = ticketVm.Description,
                    FullAddress = "",
                    BuildingId = null,
                    UserId = null,
                    UserType = UserType.Employee,
                    RequesterFullName = ticketVm.RequesterFullName,
                    RequesterEmail = ticketVm.RequesterEmail,
                    RequesterPhone = "",
                    FreshdeskTicketId = ticketVm.TicketId,
                    NewRequesterResponse = false,
                    PendingReview = true
                };

                // This is a bad idea
                ticket.CompanyId = this.CompanyId;

                return this.AddAsync(ticket);
            }
            else
            {
                // Don't process FreshDesk Ticket and send Empty Ticket as result.
                // This should be changed later with a process that saves this "unprocessed" Tickets, for debugging purposes only
                var tcs = new TaskCompletionSource<Ticket>();
                tcs.SetResult(new Ticket());
                return tcs.Task;
            }

        }

        public async Task GMailAddExternalAsync(GMailRequesterResponseViewModel ticketVm)
        {
            // Verify ig gmail is enabled
            if (!CompanySettings.GmailEnabled)
            {
                return;
            }
            var jsonBack = this.GMailApiService.Base64Decode(ticketVm.Message.Data);
            var msgdata = JsonConvert.DeserializeObject<MessageData>(jsonBack);

            // Default historyId recived from gmail push
            var historyId = msgdata.HistoryId;
            // Recover historyId from last Email store in database
            var lastGmailTicket = await this.EmailHistoryRepository.GetLastHistoryIdDapperAsync();

            // Check HistoryId is grate than the minimum setup on company setting
            if (this.CompanySettings.LastHistoryId > 0 && this.CompanySettings.LastHistoryId > historyId)
            {
                historyId = (ulong)this.CompanySettings.LastHistoryId;
            }
            if (this.CompanySettings.LastHistoryId > 0 && this.CompanySettings.LastHistoryId > lastGmailTicket)
            {
                lastGmailTicket = this.CompanySettings.LastHistoryId ?? 0;
            }

            await LoadGmailTicketFromHistoryId(historyId);
            await LoadGmailTicketFromHistoryId(lastGmailTicket);
        }

        private async Task LoadGmailTicketFromHistoryId(ulong historyId)
        {
            // Retrieve history list and read every History Object
            var history = await this.GMailApiService.GetHistory(historyId);
            if (history.History == null)
            {
                return;
            }
            foreach (var h in history.History)
            {
                foreach (var message in h.Messages)
                {
                    if (this.Repository == null)
                    {
                        continue;
                    }
                        
                    // Recover message
                    var msg = this.GMailApiService.GetMessageContent(message.Id);
                    if (msg == null)
                    {
                        continue;
                    }
                        
                    // Verify if this Email has been prossecced
                    var emailHistory = await this.EmailHistoryRepository
                        .FirstOrDefaultDapperAsync(msg.Id);

                    var msgHistoryId = msg.HistoryId ?? 0;
                    if (emailHistory != null && msg.HistoryId != null)
                    {
                        //if (emailHistory.HistoryId < msgHistoryId)
                        //{
                        //    emailHistory.HistoryId = msgHistoryId;
                        //    await this.EmailHistoryRepository
                        //        .UpdateHistoryIdAsync(emailHistory.ID, msgHistoryId);
                        //}
                        continue;
                    }

                    var ticket = await this.Repository.FirstOrDefaultDapperAsync(msg.ThreadId, msg.Id);
                    int? ticketId = null;
                    if (ticket != null)
                    {
                        ticketId = ticket.ID;
                    }
                    var messageData = await this.GMailApiService.GetEmailData(msg, ticketId);
                        
                    // Inserts new EmailTicketHistory log
                    await this.EmailHistoryRepository.AddDapperAsync(
                        new TicketEmailHistory
                        {
                            HistoryId = ((decimal)(h.Id ?? 0)), 
                            MessageId = msg.Id, 
                            ThreadId = msg.ThreadId, 
                            RawMessage = (msg.Snippet ?? "" + messageData.Body),
                            Timestamp = ((decimal)(msg.InternalDate ?? 0))
                        });
                        
                    // Verify if ticket already exists
                    if (ticket != null)
                    {
                        // If ticket already exists, verify if it was updated
                        if (msg.HistoryId > ticket.HistoryId)
                        {
                            // If message Id is different than thread Id, it means it is a response email
                            if (msg.ThreadId != msg.Id)
                            {

                                if (messageData?.From != null && !messageData.From.Contains(this.CompanySettings.GmailEmail))
                                {
                                    // Updates Ticket to "draft" and remove any snooze date
                                    await this.GMailRegisterRequesterResponseAsync(ticket.ID, msgHistoryId);
                                    // Update last history id
                                }
                            }
                        }

                        continue;
                    }

                    // Avoids creating a ticket from a reply of an email sent by
                    // the user
                    if (messageData?.From != null && messageData.From.Contains(this.CompanySettings.GmailEmail))
                    {
                        continue;
                    }
                    var newTicket = new Ticket
                    {
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        CreatedBy = this.UserEmail,
                        UpdatedBy = this.UserEmail,
                        Source = TicketSource.Email,
                        Status = TicketStatus.Draft,
                        DestinationType = TicketDestinationType.Undefined,
                        Description = messageData.Subject,
                        FullAddress = "",
                        BuildingId = null,
                        UserId = null,
                        UserType = UserType.Employee,
                        RequesterFullName = messageData.FromName,
                        RequesterEmail = messageData.From,
                        RequesterPhone = "",
                        FreshdeskTicketId = 0,
                        NewRequesterResponse = false,
                        PendingReview = true,
                        HistoryId = msgHistoryId,
                        MessageId = msg.Id,
                        CompanyId = this.CompanyId
                    };
                    ticketId = await this.Repository.AddDapperV2Async(newTicket);
                    // Ensures attachments are always created
                    await this.GMailApiService.GetEmailData(msg, ticketId);
                }
            }
        }

        public async Task<Ticket> RegisterRequesterResponse(FreshdeskRequesterResponseViewModel vm)
        {
            var ticket = await this.Repository.SingleOrDefaultAsync(T => T.FreshdeskTicketId.ToString().Equals(vm.FreshdeskTicketId));
            if (ticket != null)
            {
                //bool setOpen = (ticket.Status == TicketStatus.Resolved) || (ticket.Status == TicketStatus.Converted);
                bool setOpen = true;
                await this.Repository.UpdateByRequesterResponseAsync(ticket.ID, setOpen);
                return ticket;
            }
            else
            {
                return null;
            }
        }

        public Task GMailRegisterRequesterResponseAsync(int ticketId, ulong historyId)
        {
            return this.Repository.GMailUpdateByRequesterResponseAsync(ticketId, historyId);
        }
        #endregion External

        #region Activity Log
        public Task<IEnumerable<TicketActivityLogGridViewModel>> ReadAllTicketAcitivityLogAsync(int ticketId, IEnumerable<int> activityTypes = null)
        {
            return this.TicketActivityLogRepository.ReadAllFromTicket(ticketId, activityTypes);
        }

        public async Task<TicketActivityLog> ExistWorkOrderSequenceLog(int ticketId, int workOrderScheduleSettingId)
        {
            try
            {
                var activityLog = await this.TicketActivityLogRepository
                    .ReadAllAsync(a => a.TicketId == ticketId && a.ActivityType == TicketActivityType.TicketConvertedWorkOrderSequence);

                var log = activityLog.Where(a => a.ChangeSummary.FirstOrDefault(s => s.EntityId == workOrderScheduleSettingId) != null).FirstOrDefault();

                return log;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task RemoveActivityLog(int id)
        {
            await this.TicketActivityLogRepository.RemoveAsync(id);
        }
        #endregion

        #region Freshdesk
        public async Task<TicketAttachment> CopyFreshdeskAttachmentToTicket(TicketCopyFreshdeskImageViewModel viewModel)
        {
            Stream stream;

            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                var aBytes = webClient.DownloadData(viewModel.Url);
                stream = new MemoryStream(aBytes);
            }

            var addedFileResult = await this.AzureStorage.UploadImageAsync(stream, contentType: viewModel.FileType);
            TicketAttachment ticketAttachment = new TicketAttachment()
            {
                BlobName = addedFileResult.Item1,
                Description = viewModel.FileName,
                FullUrl = addedFileResult.Item2,
                TicketId = viewModel.TicketId,
                ID = 0
            };

            var result = await this.AttachmentsRepository.AddAsync(ticketAttachment);
            return result;
        }
        #endregion

        #region MG-15
        public async Task<int> SaveChangesAsync(bool updateAuditableFields = true, bool sendNotifications = true)
        {
            // Ask if this will send notifications
            //if (sendNotifications)
            //{
            //    await this.SendNotificationsAsync(); 
            //}

            var result = await base.SaveChangesAsync(updateAuditableFields);
            return result;
        }
        private void LogTicketActivity(EntityEntry entry)
        {
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);
            var entityId = -1;
            var entityType = TicketActivityLogEntityType.Undefined;
            //var activityType = TicketActivityType.None;

            if (entry.Entity is AuditableEntity<int> entity)
            {
                entityId = entity.ID;

                // Case for types
                // See https://stackoverflow.com/questions/298976/is-there-a-better-alternative-than-this-to-switch-on-type
                if (entity is Ticket)
                {
                    entityType = TicketActivityLogEntityType.Ticket;
                }
                //else if (entity is TicketTask)
                //{
                //    entityType = TicketActivityLogEntityType.TicketTask;
                //} It seems there isn't ticket tasks yet.
                else if (entity is TicketAttachment) // Was added for attachments
                {
                    entityType = TicketActivityLogEntityType.TicketAttachment;
                }
            }

            var unwantedFields = new List<string> { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "IsExpired", "Data" };
            var modifiedProperties = entry.Properties
                                            ?.Where(p => p.IsModified && !unwantedFields.Contains(p.Metadata.Name))
                                            ?.ToList() ?? new List<PropertyEntry>();

            var changeLog = new List<ChangeLogEntry>();
            try
            {
                foreach (var prop in modifiedProperties)
                {
                    // HACK: To ensure we get only the modified properties
                    if (prop.OriginalValue == null && prop.CurrentValue == null)
                    {
                        // Both nulls means no change
                        continue;
                    }
                    var equalsFlag = prop.OriginalValue?.Equals(prop.CurrentValue) ?? false;
                    if (equalsFlag == false)
                    {
                        string propertyName = prop.Metadata.Name;
                        // Ensuring we don't display anything (.+)Id like
                        // Aviding regex

                        // Was added for attachments
                        // Rename the Ticket attachment property from Description to Attachment
                        if (entry?.Entity is TicketAttachment entityatt)
                        {
                            propertyName = "Attachment";
                        }

                        if (propertyName.EndsWith("Id"))
                        {
                            propertyName = propertyName.Substring(0, propertyName.Length - 2);
                        }
                        changeLog.Add(new ChangeLogEntry
                        {
                            PropertyName = propertyName,
                            OriginalValue = prop.OriginalValue?.ToString(),
                            CurrentValue = prop.CurrentValue?.ToString()
                        });
                    }
                }

                // New section to capture the changes that were made in the attachments since they do not count as a modification
                if (entityType == TicketActivityLogEntityType.TicketAttachment)
                {
                    entityType = TicketActivityLogEntityType.Ticket;
                    string fileName = string.Empty;

                    if (entry?.Entity is TicketAttachment entityatt)
                    {
                        entityId = entityatt.TicketId;
                        fileName = entityatt.Description;
                    }

                    if (entry.State == EntityState.Deleted || entry.State == EntityState.Added)
                    {

                        changeLog.Add(new ChangeLogEntry
                        {
                            PropertyName = "Attachment",
                            OriginalValue = fileName,
                            CurrentValue = entry.State.ToString()
                        });
                    }

                }

            }
            catch (Exception)
            {
                // Fail silently
                // TODO: record exception DB log
            }


            // Add new log instance
            if (changeLog.Any())
            {
                var log = new TicketActivityLog
                {
                    TicketId = entityId,
                    //ActivityType = entityType, MG-15
                    ChangeLog = changeLog,
                    EmployeeId = employeeId
                };

                this.TicketActivityLogRepository.Add(log);
            }
        }
        protected override void UpdateAuditableEntities()
        {

            var ticketEntries = this.DbContext
                 .ChangeTracker
                 .Entries()
                 .Where(x => (x.Entity is Ticket || x.Entity is TicketAttachment) &&
                        (x.State == EntityState.Added ||
                         x.State == EntityState.Modified ||
                         x.State == EntityState.Deleted)) // (Deleted) Was added for attachments
                  .ToList(); // Gets a new instance since we are going to modify enumeration

            foreach (var entry in ticketEntries)
            {
                if (entry.State == EntityState.Modified)
                {
                    this.LogTicketActivity(entry);
                }
                else if (entry.State == EntityState.Added || entry.State == EntityState.Deleted && entry?.Entity is WorkOrderAttachment) // (Added-Deleted) Was added for attachments
                {
                    this.LogTicketActivity(entry);
                }

                if (entry?.Entity is Ticket entity)
                {

                    // Only status change related operations
                    //if (this.TicketStatusLogRepository.HasChangedStatus(entity))
                    //{
                    //    // Checks if the work order its set to 'close'
                    //    // And can be closed
                    //    if (entity.StatusId == TicketStatus.Closed && !this.IsCloseable(entity))
                    //    {
                    //        throw new InvalidOperationException($"This Ticket cannot be closed because it is not due yet");
                    //    }
                    //    // Add new log instance
                    //    var log = new TicketStatusLogEntry
                    //    {
                    //        StatusId = entity.StatusId,
                    //        TicketId = entity.ID,
                    //    };

                    //    this.TicketStatusLogRepository.Add(log);

                    //    // This only works for "updated" entities
                    //    // new one will have temporary IDs
                    //    if (!this.NotifiedTickets.Contains(entity.ID))
                    //    {
                    //        this.ChangedTickets.Add(entity);
                    //    }
                    //}
                }
            }

            // Here should comes all log and notifications engines
            // in order to keep all functionality in one place

            base.UpdateAuditableEntities();
        }

        protected void UpdateAuditableEntitiesExternal()
        {

            var ticketEntries = this.DbContext
                 .ChangeTracker
                 .Entries()
                 .Where(x => (x.Entity is Ticket) &&
                        (x.State == EntityState.Added ||
                         x.State == EntityState.Modified ||
                         x.State == EntityState.Deleted)) // (Deleted) Was added for attachments
                  .ToList(); // Gets a new instance since we are going to modify enumeration

            foreach (var entry in ticketEntries)
            {
                if (entry.State == EntityState.Modified)
                {
                    this.LogTicketActivity(entry);
                }
                else if (entry.State == EntityState.Added || entry.State == EntityState.Deleted && entry?.Entity is WorkOrderAttachment) // (Added-Deleted) Was added for attachments
                {
                    this.LogTicketActivity(entry);
                }

                if (entry?.Entity is Ticket entity)
                {
                }
            }

            // Here should comes all log and notifications engines
            // in order to keep all functionality in one place
            base.UpdateAuditableEntities();
        }
        public async Task<IEnumerable<TicketAttachment>> CheckAttachmentsAsync(IEnumerable<TicketAttachmentUpdateViewModel> obj, int workOrderId)
        {

            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

            // "obj" is a list of Attachments that comes from the request
            // The DB is queried to fetch existing wo attachments
            var DefaultAttachments = await this.TicketAttachmentRepository.ReadAllDapperAsync(new DataSourceRequest(), workOrderId);

            // Delete Attachments that not exists in the request
            var AttachmentsToDelete = DefaultAttachments.Payload.Where(_ => !obj.Any(a => a.ID == _.ID)).ToList();
            foreach (TicketAttachmentBaseViewModel item in AttachmentsToDelete.ToList())
            {
                await this.TicketAttachmentRepository.RemoveAsync(ent => ent.ID == item.ID);
            }

            // Create new Attachments
            var AttachmentsToCreate = this.Mapper.Map<IEnumerable<TicketAttachmentUpdateViewModel>, IEnumerable<TicketAttachment>>(obj.Where(_ => _.ID < 0));
            if (AttachmentsToCreate.Any() == false)
            {
                return new List<TicketAttachment>();
            }
            foreach (var objAttachment in AttachmentsToCreate)
            {
                var newObjAttachment = new TicketAttachment
                {
                    TicketId = workOrderId,
                    BlobName = objAttachment.BlobName,
                    FullUrl = objAttachment.FullUrl,
                    CreatedDate = objAttachment.CreatedDate,
                    Description = objAttachment.Description
                };

                await this.AddAttachmentAsync(newObjAttachment);
            }

            return new List<TicketAttachment>();
        }
        #endregion MG-15

        #region MG-115
        public async Task<int> SaveChangesAsyncExternal()
        {
            UpdateAuditableEntitiesExternal();
            var result = await base.SaveChangesAsync(false);
            return result;
        }
        #endregion MG-115
    }
}
