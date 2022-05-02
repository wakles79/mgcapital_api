using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.InspectionActivityLog;
using MGCap.Domain.ViewModels.InspectionItem;
using MGCap.Domain.ViewModels.InspectionItemTask;
using MGCap.Domain.ViewModels.PushNotifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class InspectionsApplicationService : BaseSessionApplicationService<Inspection, int>, IInspectionsApplicationService
    {
        public new IInspectionsRepository Repository => base.Repository as IInspectionsRepository;
        private readonly IInspectionItemAttachmentsRepository InspectionItemAttachmentsRepository;
        private readonly IInspectionItemsRepository InspectionItemsRepository;
        private readonly IEmployeesRepository EmployeesRepository;
        private readonly IPDFGeneratorApplicationService PDFGeneratorApplicationService;
        private readonly IEmailSender EmailSender;
        private readonly IEmailActivityLogRepository EmailActivityLogRepository;
        private readonly IInspectionActivityLogRepository InspectionActivityLogRepository;
        private readonly IInspectionItemTicketsRepository InspectionItemTicketRepository;
        private readonly IInspectionItemTasksRepository InspectionItemTasksRepository;
        private readonly IPushNotificationService PushNotificationService;
        private readonly IBuildingsRepository BuildingsRepository;
        private readonly IInspectionNotesRepository InspectionNotesRepository;
        private readonly IInspectionItemNotesRepository InspectionItemNotesRepository;

        public InspectionsApplicationService(
            IInspectionsRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IInspectionItemAttachmentsRepository inspectionItemAttachmentsRepository,
            IInspectionItemsRepository inspectionItemsRepository,
            IEmployeesRepository employeesRepository,
            IPDFGeneratorApplicationService pDFGeneratorApplicationService,
            IEmailSender emailSender,
            IEmailActivityLogRepository emailActivityLogRepository,
            IInspectionActivityLogRepository inspectionActivityLogRepository,
            IInspectionItemTicketsRepository inspectionItemTicketRepository,
            IInspectionItemTasksRepository inspectionItemTasksRepository,
            IPushNotificationService pushNotificationService,
            IBuildingsRepository buildingsRepository,
            IInspectionNotesRepository inspectionNotesRepository,
            IInspectionItemNotesRepository inspectionItemNotesRepository
        ) : base(repository, httpContextAccessor)
        {
            this.InspectionItemsRepository = inspectionItemsRepository;
            this.InspectionItemAttachmentsRepository = inspectionItemAttachmentsRepository;
            this.EmployeesRepository = employeesRepository;
            this.PDFGeneratorApplicationService = pDFGeneratorApplicationService;
            this.EmailSender = emailSender;
            this.EmailActivityLogRepository = emailActivityLogRepository;
            this.InspectionActivityLogRepository = inspectionActivityLogRepository;
            this.InspectionItemTicketRepository = inspectionItemTicketRepository;
            this.InspectionItemTasksRepository = inspectionItemTasksRepository;
            this.PushNotificationService = pushNotificationService;
            this.BuildingsRepository = buildingsRepository;
            this.InspectionNotesRepository = inspectionNotesRepository;
            this.InspectionItemNotesRepository = inspectionItemNotesRepository;
        }


        public async Task<DataSource<InspectionGridViewModel>> ReadAllDapperAsync(DataSourceRequestInspection request, int? status = -1, int? buildingId = null, int? employeeId = null)
        {
            request.TimezoneOffset = this.TimezonOffset;

            if (!request.LoggedEmployeId.HasValue)
            {
                request.LoggedEmployeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            }

            if (!request.ShowSnoozed.HasValue)
                request.ShowSnoozed = false;

            return await this.Repository.ReadAllDapperAsync(request, this.CompanyId, status, buildingId, employeeId);
        }

        public async Task<InspectionItem> AddInspectionItemAsync(InspectionItem inspectionItem)
        {
            var item = await InspectionItemsRepository.AddAsync(inspectionItem);

            if (item != null)
            {
                var itemLog = new List<ItemLogEntry>();
                itemLog.Add(new ItemLogEntry()
                {
                    ActivityType = "Added",
                    ItemType = 0,
                    Value = $"#{item.Number}, {item.Description}"
                });

                this.RegisterLogActivity(item.InspectionId,
                    InspectionActivityType.ItemUpdated,
                    itemLog: itemLog);
            }

            if (item.Number == 1)
            {
                var inspection = await this.Repository.SingleOrDefaultAsync(i => i.ID == item.InspectionId);
                if (inspection != null)
                {
                    inspection.Status = Domain.Enums.InspectionStatus.Walkthrough;
                    await this.Repository.UpdateAsync(inspection);
                }

            }

            return item;
        }

        public async Task<InspectionItem> UpdateInspectionItemAsync(InspectionItem inspectionItem)
        {
            var item = await this.InspectionItemsRepository.UpdateAsync(inspectionItem);
            if (item != null)
            {
                var itemLog = new List<ItemLogEntry>();
                itemLog.Add(new ItemLogEntry()
                {
                    ActivityType = "Updated",
                    ItemType = 0,
                    Value = $"#{item.Number}, {item.Description}"
                });

                this.RegisterLogActivity(item.InspectionId,
                    InspectionActivityType.ItemUpdated,
                    itemLog: itemLog);
            }
            return item;
        }

        //public async Task<InspectionItem> GetInspectionItemByIdAsync(int id)
        //{
        //    return await this.InspectionItemsRepository.SingleOrDefaultAsync(i => i.ID == id);
        //}

        public async Task<InspectionItem> CloseInspectionItemAsync(int id)
        {
            var inspectionItem = await this.GetInspectionItemByIdAsync(id);
            if (inspectionItem == null)
            {
                return null;
            }

            inspectionItem.Status = (int)InspectionItemStatus.Closed;

            await this.UpdateInspectionItemAsync(inspectionItem);

            var items = await this.InspectionItemsRepository.ReadAllByInspectionDapperAsync(new DataSourceRequest() { PageSize = 999 }, inspectionItem.InspectionId);
            int completedItems = items.Payload.Count(i => i.Status == InspectionItemStatus.Closed);
            if (completedItems == items.Payload.Count(i => i.ID != inspectionItem.ID))
            {
                var inspection = await this.Repository.SingleOrDefaultAsync(i => i.ID == inspectionItem.InspectionId);
                inspection.Status = InspectionStatus.Closed;
                if (inspection.CloseDate == null)
                {
                    inspection.CloseDate = DateTime.Now;
                }
                await this.UpdateAsync(inspection);
                // await this.SendNotificationToManagersAsync(inspection);
            }

            return inspectionItem;
        }

        public async Task<DataSource<InspectionItemGridViewModel>> ReadAllInspectionItemDapperAsync(DataSourceRequest request, int inspectionItemId)
        {
            return await this.InspectionItemsRepository.ReadAllByInspectionDapperAsync(request, inspectionItemId);
        }

        public async Task<DataSource<InspectionItemAttachmentBaseViewModel>> ReadAllAttachemntsAsync(DataSourceRequest request, int inspectionId)
        {
            return await this.InspectionItemAttachmentsRepository.ReadAllDapperAsync(request, inspectionId);
        }

        public async Task<InspectionItemAttachment> AddAttachmentAsync(InspectionItemAttachment obj)
        {
            return await this.InspectionItemAttachmentsRepository.AddAsync(obj);
        }

        public async Task<InspectionItemAttachment> UpdateAttachmentsAsync(InspectionItemAttachment obj)
        {
            return await this.InspectionItemAttachmentsRepository.UpdateAsync(obj);
        }

        public async Task<InspectionItemAttachment> GetAttachmentAsync(Func<InspectionItemAttachment, bool> filter)
        {
            return await this.InspectionItemAttachmentsRepository.SingleOrDefaultAsync(filter);
        }

        public Task RemoveAttachmentsAsync(int objId)
        {
            return this.InspectionItemAttachmentsRepository.RemoveAsync(objId);
        }

        public Task<InspectionReportDetailViewModel> GetInspectionDetailsDapperAsync(int? inspectionId, Guid? guid)
        {
            return this.Repository.GetInspectionDetailsDapperAsync(inspectionId, guid);
        }

        public async Task<InspectionItem> GetInspectionItemByIdAsync(int id)
        {
            return await this.InspectionItemsRepository.SingleOrDefaultAsync(i => i.ID == id);
        }

        public async Task<InspectionItem> SingleOrDefaultItemAsync(Func<InspectionItem, bool> filter)
        {
            return await InspectionItemsRepository.SingleOrDefaultAsync(filter);
        }

        public Task DeleteItemAsync(InspectionItem itemObj)
        {
            var itemLog = new List<ItemLogEntry>();
            itemLog.Add(new ItemLogEntry()
            {
                ActivityType = "Removed",
                ItemType = 0,
                Value = $"#{itemObj.Number}, {itemObj.Description}"
            });

            this.RegisterLogActivity(itemObj.InspectionId,
                InspectionActivityType.ItemUpdated,
                itemLog: itemLog);

            return this.InspectionItemsRepository.RemoveAsync(itemObj);

        }

        public async Task<InspectionItemUpdateViewModel> GetInspectionItemUpdateByIdAsync(int id)
        {
            return await InspectionItemsRepository.GetInspectionItemDapperAsync(id);
        }

        public async Task<string> GetInspectionReportUrl(int? id, Guid? guid)
        {
            try
            {
                var inspectionDetail = await this.Repository.GetInspectionDetailsDapperAsync(id, guid);

                var jInspectionDetail = JsonConvert.SerializeObject(inspectionDetail);

                var joBody = JObject.Parse(jInspectionDetail);
                joBody.Add("publicurl", $"{this.AppBaseUrl}inspections/inspection-detail/{inspectionDetail.Guid}");
                string url = await this.PDFGeneratorApplicationService.GetDocumentUrl("56950", joBody.ToString());

                return url;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Tasks
        public Task RemoveTasksAsync(int objId)
        {
            return this.InspectionItemTasksRepository.RemoveAsync(objId);
        }

        public async Task<InspectionItemTask> AddTaskAsync(InspectionItemTask obj)
        {
            return await this.InspectionItemTasksRepository.AddAsync(obj);
        }

        public async Task<InspectionItemTask> UpdateCompletedStatusTaskAsync(int id, bool isCompleted)
        {
            var task = await this.InspectionItemTasksRepository.SingleOrDefaultAsync(t => t.ID == id);
            if (task != null)
            {
                task.IsComplete = isCompleted;
                await this.InspectionItemTasksRepository.SaveChangesAsync();
            }
            return task;
        }
        #endregion

        #region Email
        public async Task SendInspectionReportByEmail(InspectionReportDetailViewModel vm, IEnumerable<InspectionAdditionalRecipientViewModel> additionalRecipients, bool commentResponse = false)
        {
            string template = commentResponse ? InspectionCommentResponseEmailTemplate : InspectionEmailTemplate;
            var emailBody = this.GetLinkContentFromTemplate(vm, template + Signature);

            try
            {
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

                IEnumerable<string> cc = null;
                List<EmailLogEntry> emailLogEntries = new List<EmailLogEntry>();
                emailLogEntries.Add(new EmailLogEntry()
                {
                    Name = vm.EmployeeName,
                    Email = vm.EmployeeEmail
                });

                if (additionalRecipients != null && additionalRecipients.Any())
                {
                    cc = additionalRecipients.Select(ar => ar.Email);
                    foreach (var ar in additionalRecipients)
                    {
                        emailLogEntries.Add(new EmailLogEntry()
                        {
                            Email = ar.Email,
                            Name = ar.FullName
                        });
                    }
                }

                await this.EmailSender.SendEmailAsync(
                    vm.EmployeeEmail,
                    "Inspection Details",
                    plainTextMessage: emailBody,
                    htmlMessage: emailBody,
                    fromDisplay: fromDisplay,
                    replyTo: this.UserEmail,
                    cc: cc);

                // Register Inspection Activity
                this.RegisterLogActivity(
                    vm.ID,
                    InspectionActivityType.EmailSent,
                    emailLog: emailLogEntries);

                // Register Activity
                await EmailActivityLogRepository.AddAsync(new EmailActivityLog()
                {
                    CompanyId = this.CompanyId,
                    Subject = "Inspection Details",
                    SentTo = vm.EmployeeEmail,
                    Body = emailBody,
                    SharedUrl = $"{this.AppBaseUrl}inspections/inspection-detail/{vm.Guid}",
                    Cc = emailLogEntries
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SendNotificationToManagersAsync(Inspection inspection)
        {
            string emailTemplate = InspectionEmailStatusNotificationTemplate;
            string status = inspection.Status.ToString().SplitCamelCase();

            string emailBody = emailTemplate
                        .Replace("[Number]", inspection.Number.ToString())
                        .Replace("[NewStatus]", status)
                        .Replace("[Link]", $"{this.AppBaseUrl}inspections/inspection-detail/{inspection.Guid}")
                        .Replace("[Sig]", Signature);

            string emailSubject = $"Inspection #{inspection.Number} [{status}]";

            try
            {
                // Gets managers contacts
                IEnumerable<EmailLogEntry> emailLogEntries = await this.Repository.GetManagersEmailsAsync(inspection.ID);

                var emailsTasks = new List<Task>();
                var activityTasks = new List<Task<EmailActivityLog>>();

                if (emailLogEntries.Any())
                {
                    var seen = new HashSet<string>();
                    var sent = new List<EmailLogEntry>();

                    foreach (var contact in emailLogEntries)
                    {
                        if (seen.Contains(contact.Email))
                        {
                            continue;
                        }

                        sent.Add(contact);
                        seen.Add(contact.Email);

                        // Adds email tasks
                        emailsTasks.Add(
                            this.EmailSender.SendEmailAsync(
                            contact.Email,
                            emailSubject,
                            plainTextMessage: emailBody,
                            htmlMessage: emailBody,
                            replyTo: this.UserEmail)
                            );

                        // Adds email activity log task
                        activityTasks.Add(
                            EmailActivityLogRepository.AddAsync(new EmailActivityLog
                            {
                                CompanyId = this.CompanyId,
                                Subject = emailSubject,
                                SentTo = contact.Email,
                                Body = emailBody,
                                SharedUrl = $"{this.AppBaseUrl}inspections/inspection-detail/{inspection.Guid}",
                            })
                            );

                    }

                    await Task.WhenAll(emailsTasks);
                    await Task.WhenAll(activityTasks);

                    // Register Inspection Activity
                    this.RegisterLogActivity(
                        inspection.ID,
                        InspectionActivityType.EmailSent,
                        emailLog: sent);

                    // create push notification
                    var pushNotificationVM = new PushNotificationInspectionCreateViewModel
                    {
                        BuildingName = this.BuildingsRepository.SingleOrDefault(inspection.BuildingId)?.Name ?? "",
                        Recipients = seen,
                        InspectionNumber = inspection.Number,
                        InspectionStatus = inspection.Status
                    };

                    // DO NOT AWAIT THIS TAKS, NEVER NEVER NEVER
                    PushNotificationService.CreateNewInspectionNotification(pushNotificationVM);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetLinkContentFromTemplate(InspectionReportDetailViewModel vm, string template)
        {
            string content = string.Empty;

            if (string.IsNullOrEmpty(template) || vm == null)
                return content;

            content = template.Replace("[Number]", vm.Number.ToString())
                                 .Replace("[Link]", $"{this.AppBaseUrl}inspections/inspection-detail/{vm.Guid}");

            return content;
        }

        protected static string InspectionEmailTemplate = @"<p>Hello!</p><br/>
            <br/><p>The inspection number [Number] is ready to be viewed. <br/>
            <br/>
            <a href='[Link]'>Click here to view the inspection.</a></p><br>
            <p>Thank you, <br/>
            </p>";

        protected static string InspectionEmailStatusNotificationTemplate = @"<p>Hello!</p>
            <p>The inspection number [Number] has a change of status to <b> [NewStatus] </b>.</p>
            <p><a href='[Link]'>Click here to view the inspection.</a></p>
            <p>Thank you,</p>
            [Sig]";

        protected static string InspectionCommentResponseEmailTemplate = @"<p>Hello!</p><br/>
            <p>MG Capital has responded to your comment.<br/>
            <br/>
            <a href='[Link]'>Click here to view your proposal.</a></p><br>
            <p>Thank you, <br/>
            </p>";

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
            <img src='http://mgcapitalmain.com/wordpress/wp-content/uploads/2018/07/img2.jpg' alt='Logo 2'>";
        #endregion

        #region Log
        public async Task<DataSource<InspectionActivityLogGridViewModel>> GetAllActivityLogDapperAsync(DataSourceRequest request, int inspectionId)
        {
            return await this.InspectionActivityLogRepository.ReadAllDapperAsync(request, inspectionId);
        }

        private InspectionActivityLog RegisterLogActivity(
            int inspectionId,
            InspectionActivityType activityType,
            IList<ChangeLogEntry> changeLog = null,
            IList<ItemLogEntry> itemLog = null,
            IList<EmailLogEntry> emailLog = null)
        {
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

            InspectionActivityLog newLog = new InspectionActivityLog()
            {
                EmployeeId = employeeId,
                InspectionId = inspectionId,
                ActivityType = activityType,
                ChangeLog = changeLog,
                ItemLog = itemLog,
                EmailLog = emailLog
            };

            return this.InspectionActivityLogRepository.Add(newLog);
        }

        private void LogInspectionActivity(EntityEntry entry)
        {
            int inspectionId = -1;

            if (entry.Entity is AuditableEntity<int> entity)
                inspectionId = entity.ID;

            var unwantedFields = new List<string> { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            var modifiedProperties = entry.Properties
                                            ?.Where(p => p.IsModified && !unwantedFields.Contains(p.Metadata.Name))
                                            ?.ToList() ?? new List<PropertyEntry>();

            var changeLog = new List<ChangeLogEntry>();
            try
            {
                foreach (var property in modifiedProperties)
                {
                    if (property.OriginalValue == null && property.CurrentValue == null)
                        continue;

                    var equalsFlag = property.OriginalValue?.Equals(property.CurrentValue) ?? false;
                    if (equalsFlag == false)
                    {
                        string propertyName = property.Metadata.Name;

                        // Checks for status changes
                        if (propertyName == "Status")
                        {
                            var triggerableStatuses = new List<InspectionStatus> {
                                InspectionStatus.WalkthroughComplete,
                                InspectionStatus.Active,
                                InspectionStatus.Closed
                            };

                            if (triggerableStatuses.Contains((InspectionStatus)property.CurrentValue))
                            {
                                // HACK: Awaits an async method from a sync one
                                var task = Task.Run(async () => await this.SendNotificationToManagersAsync(entry.Entity as Inspection));
                                var result = task.Result;
                            }
                        }
                        // Ensures we don't display anything (.+)Id like
                        if (propertyName.EndsWith("Id"))
                        {
                            propertyName = propertyName.Substring(0, propertyName.Length - 2);
                        }
                        changeLog.Add(new ChangeLogEntry
                        {
                            PropertyName = propertyName,
                            OriginalValue = property.OriginalValue?.ToString(),
                            CurrentValue = property.CurrentValue?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }

            // Add new log instance
            if (changeLog.Any())
            {
                this.RegisterLogActivity(
                    inspectionId,
                    InspectionActivityType.FieldUpdated,
                    changeLog: changeLog);
            }
        }

        protected override void UpdateAuditableEntities()
        {
            var inspectionEntries = this.DbContext
                                .ChangeTracker
                                .Entries()
                                .Where(x => (x.Entity is Inspection) &&
                                       (x.State == EntityState.Added ||
                                        x.State == EntityState.Modified))
                                 .ToList(); // Gets a new instance since we are going to modify enumeration

            // Here should comes all log and notifications engines
            // in order to keep all functionality in one place
            foreach (var entry in inspectionEntries)
            {
                // Activity log related operations
                if (entry.State == EntityState.Modified)
                {
                    this.LogInspectionActivity(entry);
                }
            }

            base.UpdateAuditableEntities();
        }
        #endregion

        #region Ticket
        public async Task<InspectionItemTicket> AddTicketFromInspectionItemAsync(InspectionItemTicket inspectionItemTicket)
        {
            var inspectionItem = await this.InspectionItemsRepository.SingleOrDefaultAsync(i => i.ID == inspectionItemTicket.InspectionItemId);
            var itemLog = new List<ItemLogEntry>();
            itemLog.Add(new ItemLogEntry()
            {
                ActivityType = "Converted",
                ItemType = 0,
                Value = $"Item #{inspectionItem.Number} to Ticket"
            });

            this.RegisterLogActivity(inspectionItem.InspectionId,
                InspectionActivityType.ItemUpdated,
                itemLog: itemLog);

            return await this.InspectionItemTicketRepository.AddAsync(inspectionItemTicket);
        }
        #endregion

        #region Notes
        public Task<DataSource<InspectionNoteGridViewModel>> ReadAllNotesDapperAsync(DataSourceRequest request, int inspectionID)
        {
            // Assigning timezone offset
            request.TimezoneOffset = this.TimezonOffset;
            return this.InspectionNotesRepository.ReadAllDapperAsync(request, inspectionID);
        }

        public Task<InspectionNote> GetNoteAsync(Func<InspectionNote, bool> filter)
        {
            return this.InspectionNotesRepository.SingleOrDefaultAsync(filter);
        }

        public Task RemoveNoteAsync(int objId)
        {
            return this.InspectionNotesRepository.RemoveAsync(objId);
        }

        public Task<InspectionNote> AddNoteAsync(InspectionNote obj)
        {
            return this.InspectionNotesRepository.AddAsync(obj);
        }

        public Task<InspectionNote> UpdateNoteAsync(InspectionNote obj)
        {
            return this.InspectionNotesRepository.UpdateAsync(obj);
        }

        public async Task<DataSource<InspectionNoteGridViewModel>> GetAllgetInspectionNotesDapperAsync(DataSourceRequest request, int inspectionId)
        {
            return await this.InspectionNotesRepository.ReadAllDapperAsync(request, inspectionId);
        }

        public async Task<InspectionItemNote> AddItemNoteAsync(InspectionItemNote obj)
        {
            return await this.InspectionItemNotesRepository.AddAsync(obj);
        }

        public Task RemoveItemNoteAsync(int objId)
        {
            return this.InspectionItemNotesRepository.RemoveAsync(objId);
        }

        #endregion
    }
}
