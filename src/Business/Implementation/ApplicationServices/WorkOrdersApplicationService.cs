using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderBillingReport;
using MGCap.Domain.ViewModels.WorkOrderTask;
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
    public class WorkOrdersApplicationService : BaseSessionApplicationService<WorkOrder, int>, IWorkOrdersApplicationService
    {
        public IWorkOrderNotificationsApplicationService WONotificationsService { get; private set; }
        public IWorkOrderNotesRepository WONotesRepository { get; private set; }
        public IWorkOrderTasksRepository WOTasksRepository { get; private set; }

        public IWorkOrderEmployeesRepository WOEmployeesRespository { get; set; }

        public IWorkOrderAttachmentRepository WOAttachmentRepository { get; private set; }

        public IEmployeesRepository EmployeesRepository { get; private set; }

        public IWorkOrderStatusLogRepository WOStatusLogRepository { get; private set; }

        public IWorkOrderActivityLogRepository WOActivityLogRepository { get; private set; }

        private IPDFGeneratorApplicationService PDFGeneratorApplicationService { get; set; }

        private IInspectionItemTicketsRepository InspectionItemTicketsRepository { get; set; }

        private IInspectionsApplicationService InspectionsApplicationService { get; set; }

        private IWorkOrderScheduleSettingsRepository WorkOrderScheduleSettingsRepository { get; set; }

        public new IWorkOrdersRepository Repository => base.Repository as IWorkOrdersRepository;
        /// <summary>
        ///     All WO that were created/updated at some point in the lifetime 
        ///     of <see cref="WorkOrdersApplicationService"/>
        /// </summary>
        public HashSet<WorkOrder> ChangedWorkOrders { get; set; }
        /// <summary>
        ///     All WO that were used for notification at some point in the lifecycle 
        ///     of <see cref="WorkOrdersApplicationService"/>
        /// </summary>
        public HashSet<int> NotifiedWorkOrders { get; set; }

        private readonly IMapper Mapper;

        public WorkOrdersApplicationService(
            IWorkOrdersRepository repository,
            IWorkOrderNotesRepository woNotesRepository,
            IWorkOrderTasksRepository woTasksRepository,
            IWorkOrderEmployeesRepository woEmployeesRepository,
            IWorkOrderAttachmentRepository woAttachmentRepository,
            IEmployeesRepository employeesRepository,
            IWorkOrderStatusLogRepository woStatusLogRepository,
            IWorkOrderActivityLogRepository woActivityLogRepository,
            IWorkOrderNotificationsApplicationService woNotificationsService,
            IPDFGeneratorApplicationService pDFGeneratorApplicationService,
            IInspectionItemTicketsRepository inspectionItemTicketsRepository,
            IInspectionsApplicationService inspectionsApplicationService,
            IWorkOrderScheduleSettingsRepository workOrderScheduleSettingsRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) : base(repository, httpContextAccessor)
        {
            this.WONotificationsService = woNotificationsService;
            this.WONotesRepository = woNotesRepository;
            this.WOTasksRepository = woTasksRepository;
            this.WOAttachmentRepository = woAttachmentRepository;
            this.WOStatusLogRepository = woStatusLogRepository;
            this.WOActivityLogRepository = woActivityLogRepository;
            this.EmployeesRepository = employeesRepository;
            this.WOEmployeesRespository = woEmployeesRepository;

            this.ChangedWorkOrders = new HashSet<WorkOrder>();
            this.NotifiedWorkOrders = new HashSet<int>();

            this.PDFGeneratorApplicationService = pDFGeneratorApplicationService;
            this.InspectionItemTicketsRepository = inspectionItemTicketsRepository;
            this.InspectionsApplicationService = inspectionsApplicationService;

            this.WorkOrderScheduleSettingsRepository = workOrderScheduleSettingsRepository;

            this.Mapper = mapper;
        }

        public async Task<DataSource<WorkOrderGridViewModel>> ReadAllDapperAsync(DataSourceRequestWOReadAll request, int? administratorId = null, int? statusId = null, int? buildingId = null, int? typeId = null, bool unscheduled = false)
        {
            // Assigning timezone offset
            request.TimezoneOffset = this.TimezonOffset;
            // Just to make sure logged employee Id gets passed
            if (!request.LoggedEmployeId.HasValue)
            {
                request.LoggedEmployeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            }
            return await this.Repository.ReadAllDapperAsync(request, this.CompanyId, administratorId, statusId, buildingId, typeId, unscheduled);
        }

        public async Task<DataSource<WorkOrderGridViewModel>> ReadAllAppDapperAsync(DataSourceRequestWOReadAll request, int? administratorId = null, int? statusId = null, int? buildingId = null, int? supervisorId = null, int? operationsManagerId = null, int? number = null, int? typeId = null)
        {
            // Assigning timezone offset
            request.TimezoneOffset = this.TimezonOffset;
            // Just to make sure logged employee Id gets passed
            if (!request.LoggedEmployeId.HasValue)
            {
                request.LoggedEmployeId = await this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
            }
            return await this.Repository.ReadAllAppDapperAsync(request, this.CompanyId, administratorId, statusId, buildingId, supervisorId, operationsManagerId, number, typeId);
        }

        public Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null)
        {
            throw new NotImplementedException();
        }

        public Task<WorkOrder> SingleOrDefaultDapperAsync(int id)
        {
            return this.Repository.SingleOrDefaultDapperAsync(id);
        }

        public Task<DataSource<WorkOrderDashboardViewModel>> GetDashboardDataDapperAsync(int? employeeId = null)
        {
            var timezoneOffset = this.TimezonOffset;
            return this.Repository.GetDashboardDataDapperAsync(this.CompanyId, timezoneOffset, employeeId);
        }

        public async Task<WorkOrderUpdateViewModel> GetFullWorkOrderDapperAsync(int workOrderId = -1, Guid? workOrderGuid = null)
        {
            var result = await this.Repository.GetFullWorkOrderDapperAsync(workOrderId, workOrderGuid);
            result.Employees = await this.EmployeesRepository.ReadAllWorkOrderEmployeesAsync(workOrderId);
            return result;
        }

        #region Employees
        public async Task AssignEmployee(int workOrderId, int employeeId, int? type = 1)
        {
            var obj = new WorkOrderEmployee
            {
                WorkOrderId = workOrderId,
                EmployeeId = employeeId,
                Type = (WorkOrderEmployeeType)type
            };

            await this.WOEmployeesRespository.AddAsync(obj);
        }

        public async Task UnassignEmployee(int workOrderId, int employeeId)
        {
            await this.WOEmployeesRespository.RemoveAsync(b => b.WorkOrderId.Equals(workOrderId) && b.EmployeeId.Equals(employeeId));
        }

        public async Task<IEnumerable<int>> GetEmployeesIdsAsync(int workOrderId, WorkOrderEmployeeType type)
        {

            return await this.Repository.GetEmployeesIdsDapperAsync(workOrderId, type);
        }
        #endregion

        #region Attachments

        public async Task<DataSource<WorkOrderAttachmentBaseViewModel>> ReadAllAttachemntsAsync(DataSourceRequest request, int workOrderId)
        {
            return await this.WOAttachmentRepository.ReadAllDapperAsync(request, workOrderId);
        }

        public async Task<WorkOrderAttachment> AddAttachmentAsync(WorkOrderAttachment obj)
        {
            return await this.WOAttachmentRepository.AddAsync(obj);
        }

        public async Task<WorkOrderAttachment> UpdateAttachmentsAsync(WorkOrderAttachment obj)
        {
            return await this.WOAttachmentRepository.UpdateAsync(obj);
        }

        public async Task<WorkOrderAttachment> GetAttachmentAsync(Func<WorkOrderAttachment, bool> filter)
        {
            return await this.WOAttachmentRepository.SingleOrDefaultAsync(filter);
        }

        public Task RemoveAttachmentsAsync(int objId)
        {
            return this.WOAttachmentRepository.RemoveAsync(objId);
        }

        public async Task<IEnumerable<WorkOrderAttachment>> CheckAttachmentsAsync(IEnumerable<WorkOrderAttachmentUpdateViewModel> obj, int workOrderId)
        {
 
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);

            // "obj" is a list of Attachments that comes from the request
            // The DB is queried to fetch existing wo attachments
            var DefaultAttachments = await this.WOAttachmentRepository.ReadAllDapperAsync(new DataSourceRequest(), workOrderId);

            // Delete Attachments that not exists in the request
            var AttachmentsToDelete = DefaultAttachments.Payload.Where(_ => !obj.Any(a => a.ID == _.ID)).ToList();
            foreach (WorkOrderAttachmentBaseViewModel item in AttachmentsToDelete.ToList())
            {
                await this.WOAttachmentRepository.RemoveAsync(ent => ent.ID == item.ID);
            }

            // Create new Attachments
            var AttachmentsToCreate = this.Mapper.Map<IEnumerable<WorkOrderAttachmentUpdateViewModel>, IEnumerable<WorkOrderAttachment>>(obj.Where(_ => _.ID < 0));
            if (AttachmentsToCreate.Any() == false)
            {
               return new List<WorkOrderAttachment>();
            }
            foreach (var objAttachment in AttachmentsToCreate)
            {
                var newObjAttachment = new WorkOrderAttachment
                {
                    WorkOrderId = workOrderId,
                    EmployeeId = employeeId,
                    BlobName = objAttachment.BlobName,
                    FullUrl = objAttachment.FullUrl,
                    ImageTakenDate = objAttachment.ImageTakenDate,
                    Description = objAttachment.Description
                };

                await this.AddAttachmentAsync(newObjAttachment);
            }

            return new List<WorkOrderAttachment>();
        }

        #endregion Attachments

        public Task<DataSource<WorkOrderNoteBaseViewModel>> ReadAllNotesDapperAsync(DataSourceRequest request, int workOrderId)
        {
            // Assigning timezone offset
            request.TimezoneOffset = this.TimezonOffset;
            return this.WONotesRepository.ReadAllDapperAsync(request, workOrderId);
        }

        public Task<WorkOrderNote> AddNoteAsync(WorkOrderNote obj)
        {
            return this.WONotesRepository.AddAsync(obj);
        }

        public Task<WorkOrderNote> UpdateNoteAsync(WorkOrderNote obj)
        {
            return this.WONotesRepository.UpdateAsync(obj);
        }

        public Task RemoveNoteAsync(int objId)
        {
            return this.WONotesRepository.RemoveAsync(objId);
        }

        public Task<DataSource<WorkOrderTaskBaseViewModel>> ReadAllTasksDapperAsync(DataSourceRequest request, int workOrderId)
        {
            return this.WOTasksRepository.ReadAllDapperAsync(request, workOrderId);
        }

        public Task<WorkOrderTask> AddTaskAsync(WorkOrderTask obj)
        {
            return this.WOTasksRepository.AddAsync(obj);
        }

        public Task<WorkOrderTask> UpdateTaskAsync(WorkOrderTask obj)
        {
            return this.WOTasksRepository.UpdateAsync(obj);
        }

        public Task RemoveTaskAsync(int objId)
        {
            return this.WOTasksRepository.RemoveAsync(objId);
        }

        public Task<WorkOrderNote> GetNoteAsync(Func<WorkOrderNote, bool> filter)
        {
            return this.WONotesRepository.SingleOrDefaultAsync(filter);
        }

        public Task<WorkOrderTask> GetTaskAsync(Func<WorkOrderTask, bool> filter)
        {
            return this.WOTasksRepository.SingleOrDefaultAsync(filter);
        }

        /// <summary>
        ///     Gets the logged employee's ID
        /// </summary>
        /// <returns></returns>
        private Task<int> GetCurrentEmployeeIdAsync()
        {
            return this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapperAsync(this.UserEmail, this.CompanyId);
        }

        public async Task<bool> CheckAssignedEmployeeAsync(WorkOrder obj)
        {
            var isChecked = await this.IsCheckedAsync(obj.ID, obj.StatusId);
            if (isChecked)
            {
                this.ChangeWorkOrderStatus(obj, WorkOrderStatus.Active);
            }
            return isChecked;
        }

        public async Task<bool> CheckAssignedEmployeeAsync(int workOrderId, WorkOrderStatus workOrderStatusId)
        {
            var isChecked = await this.IsCheckedAsync(workOrderId, workOrderStatusId);
            if (isChecked)
            {
                var dbContext = this.DbContext as MGCapDbContext;
                var obj = await dbContext.WorkOrders.FirstOrDefaultAsync(el => el.ID == workOrderId);
                this.ChangeWorkOrderStatus(obj, WorkOrderStatus.Active);
            }
            return isChecked;
        }

        /// <summary>
        ///     Checks if the logged employee is a work order employee
        ///     and if work order is on stand by status
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="workOrderStatusId"></param>
        /// <returns></returns>
        private async Task<bool> IsCheckedAsync(int workOrderId, WorkOrderStatus workOrderStatusId)
        {
            var currentEmployeeId = await this.GetCurrentEmployeeIdAsync();
            IEnumerable<int> empIds = await this.Repository.GetEmployeesIdsDapperAsync(workOrderId);
            return empIds != null && empIds.Contains(currentEmployeeId) && workOrderStatusId == WorkOrderStatus.StandBy;
        }

        private void ChangeWorkOrderStatus(WorkOrder obj, WorkOrderStatus status)
        {
            if (obj != null)
            {
                // Using repository contextart
                // due to all operations done in a regular update
                var dbContext = this.DbContext as MGCapDbContext;
                obj.StatusId = status;
                dbContext.WorkOrders.Update(obj);
            }
        }

        public Task<DataSource<WOSourcesListBoxViewModel>> ReadAllWOSourceCboDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllWOSourceCboDapperAsync(request);
        }

        /// <summary>
        ///     Indicates if the WO can be closed,
        ///     in other words if Day(NOW) >= Day(DueDate)
        /// </summary>
        /// <param name="wo"></param>
        /// <returns>If the WO can be closed</returns>
        private bool IsCloseable(WorkOrder wo)
        {
            return wo.DueDate.HasValue ? DateTime.UtcNow.Date >= wo.DueDate.Value.Date : false;
        }

        private void LogWorkOrderActivity(EntityEntry entry)
        {
            var employeeId = this.EmployeesRepository.GetEmployeeIdByEmailAndCompanyIdDapper(this.UserEmail, this.CompanyId);
            var entityId = -1;
            var entityType = WorkOrderActivityLogEntityType.Undefined;

            if (entry.Entity is AuditableEntity<int> entity)
            {
                entityId = entity.ID;

                // Case for types
                // See https://stackoverflow.com/questions/298976/is-there-a-better-alternative-than-this-to-switch-on-type
                if (entity is WorkOrder)
                {
                    entityType = WorkOrderActivityLogEntityType.WorkOrder;
                }
                else if (entity is WorkOrderTask)
                {
                    entityType = WorkOrderActivityLogEntityType.WorkOrderTask;
                }
                else if (entity is WorkOrderAttachment) // Was added for attachments
                {
                    entityType = WorkOrderActivityLogEntityType.WorkOrderAttachment;
                }
            }


            var unwantedFields = new List<string> { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "IsExpired" };
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
                        // Rename the WO attachment property from Description to Attachment
                        if (entry?.Entity is WorkOrderAttachment entityatt)
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
                if (entityType == WorkOrderActivityLogEntityType.WorkOrderAttachment)
                {
                    entityType = WorkOrderActivityLogEntityType.WorkOrder;
                    string fileName = string.Empty;

                    if (entry?.Entity is WorkOrderAttachment entityatt)
                    {
                        entityId = entityatt.WorkOrderId;
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
                var log = new WorkOrderActivityLog
                {
                    EntityId = entityId,
                    EntityType = entityType,
                    ChangeLog = changeLog,
                    EmployeeId = employeeId
                };

                this.WOActivityLogRepository.Add(log);
            }
        }


        protected override void UpdateAuditableEntities()
        {

                var workOrderEntries = this.DbContext
                     .ChangeTracker
                     .Entries()
                     .Where(x => (x.Entity is WorkOrder || x.Entity is WorkOrderTask || x.Entity is WorkOrderAttachment) &&
                            (x.State == EntityState.Added ||
                             x.State == EntityState.Modified ||
                             x.State == EntityState.Deleted)) // (Deleted) Was added for attachments
                      .ToList(); // Gets a new instance since we are going to modify enumeration

            foreach (var entry in workOrderEntries)
            {
                // Activity log related operations
                if (entry.State == EntityState.Modified)
                {
                    this.LogWorkOrderActivity(entry);
                }
                else if (entry.State == EntityState.Added || entry.State == EntityState.Deleted && entry?.Entity is WorkOrderAttachment) // (Added-Deleted) Was added for attachments
                {
                    this.LogWorkOrderActivity(entry);
                }

                if (entry?.Entity is WorkOrder entity)
                {

                    // Only status change related operations
                    if (this.WOStatusLogRepository.HasChangedStatus(entity))
                    {
                        // Checks if the work order its set to 'close'
                        // And can be closed
                        if (entity.StatusId == WorkOrderStatus.Closed && !this.IsCloseable(entity))
                        {
                            throw new InvalidOperationException($"This Work Order cannot be closed because it is not due yet");
                        }
                        // Add new log instance
                        var log = new WorkOrderStatusLogEntry
                        {
                            StatusId = entity.StatusId,
                            WorkOrderId = entity.ID,
                        };

                        this.WOStatusLogRepository.Add(log);

                        // This only works for "updated" entities
                        // new one will have temporary IDs
                        if (!this.NotifiedWorkOrders.Contains(entity.ID))
                        {
                            this.ChangedWorkOrders.Add(entity);
                        }
                    }
                }
            }

            // Here should comes all log and notifications engines
            // in order to keep all functionality in one place


            base.UpdateAuditableEntities();
        }

        public async Task SendNotificationsAsync()
        {
            foreach (var wo in this.ChangedWorkOrders)
            {
                if (this.NotifiedWorkOrders.Contains(wo.ID))
                {
                    continue;
                }
                this.NotifiedWorkOrders.Add(wo.ID);
                await this.WONotificationsService.SendNotificationsAsync(wo);
            }
            this.ChangedWorkOrders = new HashSet<WorkOrder>();
        }

        #region SaveChanges
        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public new int SaveChanges()
        {
            this.SendNotificationsAsync();
            var result = base.SaveChanges();
            return result;
        }

        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <param name="updateAuditableFields">Flag to choose whenever to update or not all Auditable Fields</param>
        /// <returns>The number of state entries written to the DB</returns>
        public new int SaveChanges(bool updateAuditableFields = true)
        {
            this.SendNotificationsAsync();
            var result = base.SaveChanges(updateAuditableFields);
            return result;
        }

        /// <summary>
        ///     Asynchronously saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public new async Task<int> SaveChangesAsync()
        {
            await this.SendNotificationsAsync();
            var result = await base.SaveChangesAsync();
            return result;
        }

        /// <summary>
        ///      Asynchronously saves all changes made in the Context to the Database.
        /// </summary>
        /// <param name="updateAuditableFields">Flag to choose whenever to update or not all Auditable Fields</param>
        /// <returns>The number of state entries written to the DB</returns>
        public async Task<int> SaveChangesAsync(bool updateAuditableFields = true, bool sendNotifications = true)
        {
            if (sendNotifications)
            {
                await this.SendNotificationsAsync();
            }

            var result = await base.SaveChangesAsync(updateAuditableFields);
            return result;
        }
        #endregion


        // AT BOTTOM TO AVOID MERGE CONFLICTS
        public Task<WorkOrderSource> GetWOSourceDapperAsync(WorkOrderSourceCode code)
        {
            return this.Repository.GetWOSourceDapperAsync(code);
        }

        public Task<DataSource<WorkOrderDailyReportViewModel>> DailyReportByOperationsManagerDapperAsync(DataSourceRequestWOReadAll request, int? operationsManagerId = null)
        {
            return this.Repository.DailyReportByOperationsManagerDapperAsync(request, this.CompanyId, operationsManagerId);
        }

        public Task<DataSource<WorkOrderBillingReportGridViewModel>> ReadBillingReportDapperAsync(DataSourceRequestBillingReport request)
        {
            // request.TimezoneOffset = this.TimezonOffset;
            return this.Repository.ReadBillingReportDapperAsync(request, this.CompanyId);
        }

        public async Task<string> GetBillingDocumentReportUrl(IEnumerable<WorkOrderBillingReportGridViewModel> data)
        {
            var ids = data.Select(w => w.WorkOrderId);
            var workOrders = await this.Repository.ReadAllByIDs(ids);

            List<WorkOrderReportViewModel> payloadPdf = new List<WorkOrderReportViewModel>();

            foreach (var workOrder in workOrders)
            {
                var detailWorkOrder = data?.FirstOrDefault(w => w.WorkOrderId == workOrder.ID);

                List<WorkOrderTaskDetailsViewModel> tasks = new List<WorkOrderTaskDetailsViewModel>();
                foreach (var task in workOrder.Tasks)
                {
                    var detailTask = data?.FirstOrDefault(t => t.ID == task.ID);

                    if (detailTask == null)
                    {
                        continue;
                    }

                    tasks.Add(new WorkOrderTaskDetailsViewModel()
                    {
                        Description = task.Description,
                        ServiceName = detailTask.ServiceName,
                        UnitPrice = task.UnitPrice,
                        UnitFactor = detailTask.UnitFactor,
                        ServicePrice = detailTask.ServicePrice,
                        Note = task.GeneralNote.RemoveExtraNewLineCharacters()
                    });
                }

                payloadPdf.Add(new WorkOrderReportViewModel()
                {
                    Id = workOrder.ID,
                    Number = workOrder.Number,
                    Description = workOrder.Description,
                    BuildingName = detailWorkOrder.BuildingName,
                    WorkOrderCreatedDate = detailWorkOrder.WorkOrderCreatedDate,
                    WorkOrderCompletedDate = detailWorkOrder.WorkOrderCompletedDate,
                    Tasks = tasks.AsEnumerable()
                });
            }

            // var tasks = await this.WOTasksRepository.ReadAllByWOIdsDapperAsync(ids);
            var jsonData = JsonConvert.SerializeObject(new { Payload = payloadPdf });

            var result = await this.PDFGeneratorApplicationService.GetDocumentUrl("54862", jsonData);

            return result;
        }

        public void MarkWorkOrderAsChanged(WorkOrder obj)
        {
            this.ChangedWorkOrders.Add(obj);
        }

        public Building GetBuilding(int buildingId)
        {
            return this.Repository.GetBuilding(buildingId);
        }

        #region Inspection
        public async Task<bool> ValidateExistingInspectionReferenced(int workOrderId)
        {
            var inspectionItemTicketReference = await this.InspectionItemTicketsRepository
                .SingleOrDefaultAsync(i => i.entityId == workOrderId && i.DestinationType == TicketDestinationType.WorkOrder);

            if (inspectionItemTicketReference != null)
            {
                var item = await this.InspectionsApplicationService.CloseInspectionItemAsync(inspectionItemTicketReference.InspectionItemId);
            }
            else
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Calendar
        public async Task<WorkOrder> UpdateWorkOrderTaskSummary(WorkOrderTaskSummaryUpdateViewModel wo)
        {
            var workOrder = await this.Repository.SingleOrDefaultAsync(w => w.ID == wo.ID);
            workOrder.ScheduleDate = wo.ScheduleDate;
            workOrder.ScheduleDateConfirmed = wo.ScheduleDateConfirmed;
            workOrder.ClientApproved = wo.ClientApproved;

            var result = await this.Repository.UpdateAsync(workOrder);
            return result;
        }
        #endregion

        #region Work Order Schedule Setting
        public Task<WorkOrderScheduleSetting> AddScheduleSettings(WorkOrderScheduleSetting scheduleSetting)
        {
            return this.WorkOrderScheduleSettingsRepository.AddAsync(scheduleSetting);
        }

        public Task<IEnumerable<string>> ReadAllWorkOrderNumberFromSequence(int workOrderScheduleSettingId)
        {
            return this.WorkOrderScheduleSettingsRepository.ReadAllWorkOrderNumberFromSequence(workOrderScheduleSettingId);
        }
        #endregion

        #region Employees
        public Task UnassignEmployeesByWorkOrderIdAsync(int workOrderId)
        {
            return this.Repository.UnassignEmployeesByWorkOrderIdAsync(workOrderId);
        }
        #endregion

        #region Tasks
        public Task<IEnumerable<WorkOrderTaskGridViewModel>> ReadAllTasksAsync(int workOrderId)
        {
            return this.WOTasksRepository.ReadAllDapperAsync(workOrderId);
        }

        public Task<IEnumerable<WorkOrderTaskUpdateViewModel>> ReadAllUpdateTasksAsync(int workOrderId)
        {
            return this.WOTasksRepository.ReadAllUpdateDapperAsync(workOrderId);
        }

        public Task<WorkOrderTaskDetailsViewModel> GetWorkOrderTaskAsync(int id)
        {
            return this.WOTasksRepository.GetAsync(id);
        }
        #endregion
    }
}
