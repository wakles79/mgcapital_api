using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.CleaningReportActivityLog;
using MGCap.Domain.ViewModels.CleaningReportItem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class CleaningReportsApplicationService : BaseSessionApplicationService<CleaningReport, int>, ICleaningReportsApplicationService
    {
        public CleaningReportsApplicationService(
            IEmailSender emailSender,
            ICleaningReportItemAttachmentsRepository cleaningReportItemAttachmentsRepository,
            ICleaningReportItemsRepository cleaningReportItemsRepository,
            ICleaningReportNoteRepository cleaningReportNoteRepository,
            ICleaningReportsRepository repository,
            IEmployeesRepository employeesRepository,
            ICleaningReportActivityLogRepository cleaningReportActivityLogRepository,
            IEmailActivityLogRepository emailActivityLogRepository,
            IPDFGeneratorApplicationService pDFGeneratorApplicationService,
            IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            CleaningReportItemRepository = cleaningReportItemsRepository;
            CleaningReportNoteRepository = cleaningReportNoteRepository;
            CleaningReportItemAttachmentsRepository = cleaningReportItemAttachmentsRepository;
            EmailSender = emailSender;
            EmployeesRepository = employeesRepository;
            CleaningReportActivityLogRepository = cleaningReportActivityLogRepository;
            this.EmailActivityLogRepository = emailActivityLogRepository;
            this.PDFGeneratorApplicationService = pDFGeneratorApplicationService;
        }

        public new ICleaningReportsRepository Repository => base.Repository as ICleaningReportsRepository;

        private readonly ICleaningReportItemsRepository CleaningReportItemRepository;

        private readonly ICleaningReportNoteRepository CleaningReportNoteRepository;

        private readonly ICleaningReportItemAttachmentsRepository CleaningReportItemAttachmentsRepository;

        private readonly IEmailSender EmailSender;

        private readonly IEmployeesRepository EmployeesRepository;

        private readonly ICleaningReportActivityLogRepository CleaningReportActivityLogRepository;

        private readonly IEmailActivityLogRepository EmailActivityLogRepository;

        private readonly IPDFGeneratorApplicationService PDFGeneratorApplicationService;

        public Task<DataSourceCleaningReport> ReadAllDapperAsync(DataSourceRequest request, int? contactId = null, int? employeeId = null, int? statusId = null, int? commentDirection = null)
        {
            return Repository.ReadAllDapperAsync(request, CompanyId, contactId, statusId, employeeId, commentDirection);
        }

        public Task<DataSource<CleaningReportListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null)
        {
            return Repository.ReadAllCboDapperAsync(request, CompanyId, id);
        }

        public async Task<CleaningReportDetailsViewModel> GetCleaningReportDetailsDapperAsync(int cleaningReportId = -1, Guid? guid = null)
        {
            return await Repository.GetCleaningReportDetailsDapperAsync(cleaningReportId, guid);
        }

        #region CleaningReportItems

        public async Task<CleaningReportItem> AddCleaningReportItemAsync(CleaningReportItem obj)
        {
            var cleaningReport = await CleaningReportItemRepository.AddAsync(obj);

            if (cleaningReport != null)
            {
                var itemLog = new List<ItemLogEntry>();
                itemLog.Add(new ItemLogEntry()
                {
                    ActivityType = "Added",
                    ItemType = (int)cleaningReport.Type,
                    Value = cleaningReport.Observances
                });

                this.RegisterLogActivity(
                    obj.CleaningReportId,
                    CleaningReportActivityType.ItemUpdated,
                    itemLog: itemLog);
            }

            return cleaningReport;
        }

        public async Task<IEnumerable<CleaningReportItemGridViewModel>> GetCleaningReportItemsDapper(int cleaningReportId, int? type = null)
        {
            return await CleaningReportItemRepository.GetCleaningReportItemsDapper(CompanyId, cleaningReportId, type);
        }

        public async Task<CleaningReportItem> SingleOrDefaultItemAsync(Func<CleaningReportItem, bool> filter)
        {
            return await CleaningReportItemRepository.SingleOrDefaultAsync(filter);
        }

        public async Task<CleaningReportItem> UpdateItemAsync(CleaningReportItem obj)
        {
            return await CleaningReportItemRepository.UpdateAsync(obj);
        }

        public async Task DeleteItemAsync(CleaningReportItem obj)
        {
            var deletedItem = CleaningReportItemRepository.RemoveAsync(obj);
            if (deletedItem != null)
            {
                var itemLog = new List<ItemLogEntry>();
                itemLog.Add(new ItemLogEntry()
                {
                    ActivityType = "Deleted",
                    ItemType = (int)obj.Type,
                    Value = obj.Observances
                });

                this.RegisterLogActivity(
                    obj.CleaningReportId,
                    CleaningReportActivityType.ItemUpdated,
                    itemLog: itemLog);
            }
        }

        public async Task<CleaningReportItemAttachment> GetAttachmentAsync(Func<CleaningReportItemAttachment, bool> filter)
        {
            return await this.CleaningReportItemAttachmentsRepository.SingleOrDefaultAsync(filter);
        }

        public Task RemoveAttachmentsAsync(int objId)
        {
            return this.CleaningReportItemAttachmentsRepository.RemoveAsync(objId);
        }

        public async Task<CleaningReportItemAttachment> AddAttachmentAsync(CleaningReportItemAttachment obj)
        {
            return await this.CleaningReportItemAttachmentsRepository.AddAsync(obj);
        }

        public async Task<CleaningReportItemUpdateViewModel> GetCleaningReportItemAsync(int id)
        {
            return await this.CleaningReportItemRepository.GetCleaningReportItemDapperAsync(id);
        }
        #endregion

        #region Email
        public async Task SendCleaningReport(CleaningReportDetailsViewModel reportVM, IEnumerable<CleaningReportAdditionalRecipientViewModel> additionalRecipients, bool commentResponse = false)
        {
            string template = commentResponse ? CleaningReportCommentResponseEmailTemplate : CleaningReportEmailTemplate;
            var body = GetLinkContentFromTemplate(template + Signature, reportVM);

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

                var ccLog = new List<EmailLogEntry>();
                IEnumerable<string> cc = null;
                if (additionalRecipients != null && additionalRecipients.Any())
                {
                    cc = additionalRecipients.Select(ar => ar.Email);
                    foreach (var item in additionalRecipients)
                    {
                        ccLog.Add(new EmailLogEntry()
                        {
                            Email = item.Email,
                            Name = item.FullName
                        });
                    }
                }

                string base64 = await this.GetReportPDFBase64(reportVM.ID);
                byte[] bytePDF = Convert.FromBase64String(base64);

                Dictionary<string, byte[]> attachment = new Dictionary<string, byte[]>();
                attachment.Add("Cleaning Report.pdf", bytePDF);

                bool sendBeforeExternal = await this.EmailActivityLogRepository.ExistsDapperAsync(this.CompanyId, "Cleaning Report", reportVM.ToEmail, body);
                // bool sendBeforeExternal = EmailActivityLogRepository.Exists(e => e.CompanyId == this.CompanyId &&
                //                                 e.Subject == "Cleaning Report" &&
                //                                 e.SentTo == reportVM.ToEmail &&
                //                                 e.Body == body);
                if (!sendBeforeExternal)
                {
                    await this.EmailSender.SendEmailAsyncWithAttachments(
                    reportVM.ToEmail,
                    "Cleaning Report",
                    plainTextMessage: body,
                    htmlMessage: body,
                    fromDisplay: fromDisplay,
                    replyTo: this.UserEmail,
                    cc: cc,
                    attachments: attachment);

                    this.RegisterLogActivity(
                        reportVM.ID,
                        CleaningReportActivityType.EmailSent,
                        emailLog: ccLog);

                    // Register Activity
                    await EmailActivityLogRepository.AddAsync(new EmailActivityLog()
                    {
                        CompanyId = this.CompanyId,
                        Subject = "Cleaning Report",
                        SentTo = reportVM.ToEmail,
                        Body = body,
                        SharedUrl = $"{this.AppBaseUrl}reports/cleaning-report/{reportVM.Guid}",
                        Cc = ccLog
                    });
                }

                //Increment submitted value
                var reportObj = await SingleOrDefaultAsync(ent => ent.ID == reportVM.ID);
                reportObj.Submitted++;
                await UpdateAsync(reportObj);
            }
            catch (Exception exception)
            {
                await this.EmailSender.SendEmailAsync(
                            "axzesllc@gmail.com",
                            "Error Sending Emails (Cleaning Report)",
                            plainTextMessage: exception.Message);
                throw exception;
            }
        }

        private string GetLinkContentFromTemplate(string template, CleaningReportDetailsViewModel vm)
        {
            if (string.IsNullOrEmpty(template) || vm == null)
            {
                return string.Empty;
            }

            var result = template.Replace("[To]", vm.To.ToString())
                                 .Replace("[ServiceDate]", vm.DateOfService.ToShortDateString())
                                 .Replace("[Link]", $"{this.AppBaseUrl}reports/cleaning-report/{vm.Guid}")
                                 .Replace("[From]", vm.From);
            return result;
        }

        protected static string CleaningReportCommentResponseEmailTemplate = @"<p>Dear [To],</p><br/>
            <p>MG Capital has responded to your comment.<br/>
            <br/>
            <a href='[Link]'>Click here to view your report.</a></p><br>
            <p>Thank you, <br/>
            [From]</p>";

        public static string CleaningReportEmailTemplate = @" <p>Dear [To],</p><br/>
            <p>Your cleaning report for [ServiceDate] is ready to be viewed. <br/>
            <br/>
            <a href='[Link]'>Click here to view your report.</a></p><br>
            <p>Thank you, <br/>
            [From]</p>";

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

        #region Cleaning Report Notes

        public async Task<CleaningReportNote> AddCleaningReportNoteAsync(CleaningReportNote obj)
        {
            if (obj.Direction.Equals(CleaningReportNoteDirection.Outgoing))
            {
                obj.SenderId = await EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            }

            obj.BeforeCreate(this.UserEmail, this.CompanyId);

            return await CleaningReportNoteRepository.AddAsync(obj);
        }

        #endregion

        #region Log
        private List<ChangeLogEntry> CompareChangesFromCleaningReport(CleaningReport oldModel, CleaningReport newModel)
        {
            List<ChangeLogEntry> changeLogs = new List<ChangeLogEntry>();
            var oldType = oldModel.GetType();
            foreach (var oProperty in oldType.GetProperties())
            {
                var oldValue = oProperty.GetValue(oldModel, null);
                var newValue = oProperty.GetValue(newModel, null);

                if (!object.Equals(oldValue, newValue))
                    changeLogs.Add(new ChangeLogEntry()
                    {
                        PropertyName = oProperty.Name,
                        OriginalValue = oldValue.ToString(),
                        CurrentValue = newValue.ToString()
                    });
            }
            return changeLogs;
        }

        private CleaningReportActivityLog RegisterLogActivity(
            int cleaningReportId,
            CleaningReportActivityType activityType,
            IList<ChangeLogEntry> changeLog = null,
            IList<EmailLogEntry> emailLog = null,
            IList<ItemLogEntry> itemLog = null)
        {
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

            var logActivity = new CleaningReportActivityLog()
            {
                EmployeeId = employeeId,
                CleaningReportId = cleaningReportId,
                ActivityType = activityType,
                ChangeLog = changeLog,
                EmailLog = emailLog,
                ItemLog = itemLog
            };

            return CleaningReportActivityLogRepository.Add(logActivity);
        }

        private void LogCleaningReportActivity(EntityEntry entry)
        {
            int cleaningReportId = -1;

            if (entry.Entity is AuditableEntity<int> entity)
                cleaningReportId = entity.ID;

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
                        // Ensuring we don't display anything (.+)Id like
                        // Aviding regex
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
                    cleaningReportId,
                    CleaningReportActivityType.FieldUpdated,
                    changeLog: changeLog);
            }
        }

        protected override void UpdateAuditableEntities()
        {
            var cleaningReportEntries = this.DbContext
                                 .ChangeTracker
                                 .Entries()
                                 .Where(x => (x.Entity is CleaningReport) &&
                                        (x.State == EntityState.Added ||
                                         x.State == EntityState.Modified))
                                  .ToList(); // Gets a new instance since we are going to modify enumeration

            // Here should comes all log and notifications engines
            // in order to keep all functionality in one place
            foreach (var entry in cleaningReportEntries)
            {
                // Activity log related operations
                if (entry.State == EntityState.Modified)
                {
                    this.LogCleaningReportActivity(entry);
                }
            }

            base.UpdateAuditableEntities();
        }

        public async Task<DataSource<CleaningReportActivityLogGridViewModel>> GetAllActivityLogDapperAsync(DataSourceRequest request, int cleaningReportId)
        {
            return await CleaningReportActivityLogRepository.ReadAllDapperAsync(request, cleaningReportId);
        }

        public async Task<string> GetReportPDFBase64(int cleaningReportId)
        {
            string result = string.Empty;
            try
            {
                var cleaningReport = await this.Repository.GetCleaningReportDetailsDapperAsync(cleaningReportId);

                var jsonCleaningReport = JsonConvert.SerializeObject(cleaningReport);

                JObject joCleaningReport = JObject.Parse(jsonCleaningReport);
                joCleaningReport["DateOfService"] = cleaningReport.DateOfService.ToString("yyyy-MM-dd");
                joCleaningReport.Add("publicurl", $"{this.AppBaseUrl}reports/cleaning-report/{cleaningReport.Guid}");

                result = await this.PDFGeneratorApplicationService.GetBase64Document("54273", joCleaningReport.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        #endregion
    }
}
