using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderBillingReport;
using MGCap.Domain.ViewModels.WorkOrderScheduleSetting;
using MGCap.Domain.ViewModels.WorkOrderTask;
using MGCap.Presentation.Filters;

using MGCap.Presentation.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace MGCap.Presentation.Controllers
{
    public class WorkOrdersController : BaseEntityController<WorkOrder, int>
    {
        public new IWorkOrdersApplicationService AppService => base.AppService as IWorkOrdersApplicationService;
        private ITicketsApplicationService _TicketsApplicationService;

        public IBuildingsApplicationService _buildingAppService { get; set; }

        protected readonly IAzureStorage _azureStorage;

        public WorkOrdersController(
            IEmployeesApplicationService employeeAppService,
            IWorkOrdersApplicationService appService,
            IBuildingsApplicationService buildingAppService,
            ITicketsApplicationService ticketsApplicationService,
            IAzureStorage azureStorage,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
            _azureStorage = azureStorage;
            this._buildingAppService = buildingAppService;
            this._TicketsApplicationService = ticketsApplicationService;
        }

        protected async Task<ActionResult<WorkOrderUpdateViewModel>> GetWorkOrder(int workOrderId)
        {
            try
            {
                var result = await this.AppService.GetFullWorkOrderDapperAsync(workOrderId, null);
                if (result == null)
                {
                    return NoContent();
                }

                var isChecked = await this.AppService.CheckAssignedEmployeeAsync(result.ID, result.StatusId);
                if (isChecked)
                {
                    await this.AppService.SaveChangesAsync();
                }

                result.HasChangedStatus = isChecked;
                return new JsonResult(result);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

        #region WO
        [HttpGet]
        [PermissionsFilter("ReadWorkOrders")]
        public async Task<ActionResult<DataSource<WorkOrderGridViewModel>>> ReadAll(DataSourceRequestWOReadAll request, int? administratorId, int? statusId, int? buildingId, int? typeId, bool unscheduled = false)
        {
            var wosVM = await this.AppService.ReadAllDapperAsync(request, administratorId, statusId, buildingId, typeId, unscheduled);
            return new JsonResult(wosVM);
        }

        /// <summary>
        /// Return all Work Order for mobile App
        /// </summary>
        [HttpGet]
        [PermissionsFilter("ReadWorkOrders")]
        public async Task<ActionResult<DataSource<WorkOrderGridViewModel>>> ReadAllApp(DataSourceRequestWOReadAll request, int? administratorId, int? statusId, int? buildingId, int? supervisorId, int? operationsManagerId, int? number, int? typeId)
        {
            var wosVM = await this.AppService.ReadAllAppDapperAsync(request, administratorId, statusId, buildingId, supervisorId, operationsManagerId, number, typeId);
            return new JsonResult(wosVM);
        }

        [HttpGet]
        [PermissionsFilter("ReadWorkOrders")]
        public async Task<ActionResult<WorkOrderUpdateViewModel>> Get(int id)
        {
            try
            {
                var result = await this.AppService.GetFullWorkOrderDapperAsync(id, null);
                if (result == null)
                {
                    return NoContent();
                }

                var isChecked = await this.AppService.CheckAssignedEmployeeAsync(result.ID, result.StatusId);
                if (isChecked)
                {
                    await this.AppService.SaveChangesAsync();
                }

                result.HasChangedStatus = isChecked;
                return new JsonResult(result);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<WorkOrderUpdateViewModel>> PublicGet(Guid guid)
        {
            try
            {
                var result = await this.AppService.GetFullWorkOrderDapperAsync(workOrderGuid: guid);
                if (result == null)
                {
                    return NoContent();
                }

                var tasks = await this.AppService.ReadAllUpdateTasksAsync(result.ID);
                if (tasks.Any())
                {
                    result.Tasks = tasks;
                }

                return new JsonResult(result);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        /// <summary>
        /// Gets all related information needed for cloning a given Work Order.
        /// </summary>
        /// <returns>The Work Order to be cloned from.</returns>
        /// <param name="id">Work Order's Identifier.</param>
        [HttpGet]
        [PermissionsFilter("CloneWorkOrders")]
        public async Task<ActionResult<WorkOrderCreateViewModel>> GetForCloning(int id)
        {
            try
            {
                var woUpdateVM = await this.AppService.GetFullWorkOrderDapperAsync(id, null);
                if (woUpdateVM == null)
                {
                    return NoContent();
                }

                var woCreateVM = this.Mapper.Map<WorkOrderUpdateViewModel, WorkOrderCreateViewModel>(woUpdateVM);

                woCreateVM.StatusId = WorkOrderStatus.StandBy;
                woCreateVM.WorkOrderSourceId = 16;
                woCreateVM.OriginWorkOrderId = woUpdateVM.OriginWorkOrderId ?? woUpdateVM.ID;
                woCreateVM.KeepCloningReference = true;
                woCreateVM.ClosingNotes = string.Empty;
                woCreateVM.FollowUpOnClosingNotes = null;

                woCreateVM.Notes = woUpdateVM.Notes.Select(n => new WorkOrderNoteCreateViewModel
                {
                    EmployeeId = n.EmployeeId,
                    EmployeeEmail = n.EmployeeEmail,
                    EmployeeFullName = n.EmployeeFullName,
                    Note = n.Note
                });

                woCreateVM.Tasks = woUpdateVM.Tasks.Select(t => new WorkOrderTaskCreateViewModel
                {
                    Description = t.Description,
                    DiscountPercentage = t.DiscountPercentage,
                    IsComplete = t.IsComplete,
                    Quantity = t.Quantity,
                    ServiceId = t.ServiceId,
                    UnitPrice = t.UnitPrice
                }).ToList();

                return new JsonResult(woCreateVM);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

        [HttpPost]
        [PermissionsFilter("AddWorkOrders", "AddWorkOrderFromCalendar")]
        public async Task<IActionResult> Add([FromBody] WorkOrderCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                // always send notifications
                vm.SendNotifications = true;

                // cant create wo
                if (!vm.ScheduleDate.HasValue && !vm.DueDate.HasValue)
                {
                    return this.BadRequest(this.ModelState);
                }

                // mark all tasks as "complete" when WO is closed (REMOVED BY NOW)
                //if (vm.StatusId.Equals((int)WorkOrderStatus.Closed))
                //{
                //    for (int i = 0; i < vm.Tasks.Count; ++i)
                //    {
                //        vm.Tasks[i].IsComplete = true;
                //    }
                //}

                if (vm.KeepCloningReference == false)
                {
                    vm.OriginWorkOrderId = null;
                }

                // Ticket #388
                if (vm.Unscheduled)
                {
                    vm.DueDate = null;
                    vm.StatusId = 0;
                }

                // Ticket MG32
                if (!vm.Priority.HasValue)
                {
                    vm.Priority = WorkOrderPriority.Low;
                }

                // Gets source from DB and assign source Id to work order
                var source = await this.AppService.GetWOSourceDapperAsync(vm.SourceCode);
                vm.WorkOrderSourceId = source.ID;

                var obj = this.Mapper.Map<WorkOrderCreateViewModel, WorkOrder>(vm);

                //  prevent foreign key error
                foreach (var task in obj.Tasks)
                {
                    task.ServiceId = task.ServiceId == 0 ? null : task.ServiceId;
                    task.WorkOrderServiceCategoryId = task.WorkOrderServiceCategoryId == 0 ? null : task.WorkOrderServiceCategoryId;
                    task.WorkOrderServiceId = task.WorkOrderServiceId == 0 ? null : task.WorkOrderServiceId;
                }

                // Set standby if is set to draft
                obj.StatusId = obj.StatusId == WorkOrderStatus.Draft ? WorkOrderStatus.StandBy : obj.StatusId;

                await this.AppService.AddAsync(obj);
                await this.AppService.SaveChangesAsync(sendNotifications: false);

                // get employees by buildingId
                var employeesByBuilding = await this._buildingAppService.GetEmployeesByBuildingId(vm.BuildingId);

                // assign the building employees to work order
                var taskList = new List<Task>();
                foreach (EmployeeBuildingViewModel employee in employeesByBuilding)
                {
                    taskList.Add(this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type));
                }
                // user selected employees
                foreach (var employee in vm.AssignedEmployees)
                {
                    taskList.Add(this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type));
                }

                await Task.WhenAll(taskList);
                await this.AppService.SaveChangesAsync(sendNotifications: false);

                if (vm.SendNotifications)
                {
                    await this.AppService.SendNotificationsAsync();
                    // to save log 
                    await this.AppService.SaveChangesAsync(sendNotifications: false);
                }

                var isChecked = await this.AppService.CheckAssignedEmployeeAsync(obj);
                if (isChecked)
                {
                    await this.AppService.SaveChangesAsync(sendNotifications: vm.SendNotifications);
                }

                var result = this.Mapper.Map<WorkOrder, WorkOrderUpdateViewModel>(obj);
                return new JsonResult(result);
            }
            catch (InvalidOperationException ioEx)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed, ioEx.Message);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [PermissionsFilter("ConvertTickets")]
        public async Task<IActionResult> AddFromTicket([FromBody] WorkOrderCreateFromTicketViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                // cant create wo
                if (!vm.ScheduleDate.HasValue && !vm.DueDate.HasValue)
                {
                    return this.BadRequest(this.ModelState);
                }

                var ticket = await this._TicketsApplicationService.SingleOrDefaultAsync(vm.TicketId);
                if (ticket == null)
                {
                    return StatusCode(
                        (int)HttpStatusCode.BadRequest,
                        "Error finding ticket"
                    );
                }

                // mark all tasks as "complete" when WO is closed (REMOVED BY NOW)
                //if (vm.StatusId.Equals((int)WorkOrderStatus.Closed))
                //{
                //    for (int i = 0; i < vm.Tasks.Count; ++i)
                //    {
                //        vm.Tasks[i].IsComplete = true;
                //    }
                //}

                if (vm.KeepCloningReference == false)
                {
                    vm.OriginWorkOrderId = null;
                }

                // Ticket #388
                if (vm.Unscheduled)
                {
                    vm.DueDate = null;
                    vm.StatusId = 0;
                }

                // Ticket MG32
                if (!vm.Priority.HasValue)
                {
                    vm.Priority = WorkOrderPriority.Low;
                }

                // Gets source from DB and assign source Id to work order
                var source = await this.AppService.GetWOSourceDapperAsync(vm.SourceCode);
                vm.WorkOrderSourceId = source.ID;

                var obj = this.Mapper.Map<WorkOrderCreateViewModel, WorkOrder>(vm);

                //  prevent foreign key error
                foreach (var task in obj.Tasks)
                {
                    task.ServiceId = task.ServiceId == 0 ? null : task.ServiceId;
                    task.WorkOrderServiceCategoryId = task.WorkOrderServiceCategoryId == 0 ? null : task.WorkOrderServiceCategoryId;
                    task.WorkOrderServiceId = task.WorkOrderServiceId == 0 ? null : task.WorkOrderServiceId;
                }

                // Set standby if is set to draft
                obj.StatusId = obj.StatusId == WorkOrderStatus.Draft ? WorkOrderStatus.StandBy : obj.StatusId;

                await this.AppService.AddAsync(obj);
                await this.AppService.SaveChangesAsync(sendNotifications: vm.SendNotifications);

                // get employees by buildingId
                var employeesByBuilding = await this._buildingAppService.GetEmployeesByBuildingId(vm.BuildingId);

                // assign the building employees to work order
                var taskList = new List<Task>();
                foreach (EmployeeBuildingViewModel employee in employeesByBuilding)
                {
                    taskList.Add(this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type));
                }
                // user selected employees
                foreach (var employee in vm.AssignedEmployees)
                {
                    taskList.Add(this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type));
                }

                await Task.WhenAll(taskList);
                await this.AppService.SaveChangesAsync(sendNotifications: vm.SendNotifications);

                var isChecked = await this.AppService.CheckAssignedEmployeeAsync(obj);
                if (isChecked)
                {
                    await this.AppService.SaveChangesAsync(sendNotifications: vm.SendNotifications);
                }

                // Convert Ticket 
                ticket.DestinationType = TicketDestinationType.WorkOrder;
                ticket.DestinationEntityId = obj.ID;

                ticket.PendingReview = false;
                ticket.NewRequesterResponse = false;

                bool saveActivity = true;
                bool sequenceConverted = false;
                string textLog = string.Empty;
                if (obj.WorkOrderScheduleSettingId.HasValue)
                {
                    sequenceConverted = true;
                    var existSequeceLog = await this._TicketsApplicationService.ExistWorkOrderSequenceLog(ticket.ID, obj.WorkOrderScheduleSettingId.Value);

                    IEnumerable<string> numbers = await this.AppService.ReadAllWorkOrderNumberFromSequence(obj.WorkOrderScheduleSettingId.Value);

                    if (numbers.Any())
                    {
                        List<string> numbersList = numbers.ToList();

                        if (numbers.Count() <= 3)
                        {
                            textLog = "Ticket converted to " + string.Join(", ", numbersList);
                        }
                        else if (numbers.Count() > 3)
                        {
                            string initNumbers = string.Join(", ", numbersList.GetRange(0, 2));
                            string endNumbers = numbersList[numbers.Count() - 1];
                            textLog = $"Ticket converted to {initNumbers}... {endNumbers}";
                        }
                    }

                    // Remove existing log
                    if (existSequeceLog != null)
                    {
                        await this._TicketsApplicationService.RemoveActivityLog(existSequeceLog.ID);
                        await this._TicketsApplicationService.SaveChangesAsync();
                    }
                }

                ticket = await this._TicketsApplicationService.UpdateEntityReferenceAsync(ticket, saveActivity: saveActivity, sequenceConverted: sequenceConverted, textLog: textLog);
                await this._TicketsApplicationService.SaveChangesAsync();
                // Convert Ticket 

                var result = this.Mapper.Map<WorkOrder, WorkOrderUpdateViewModel>(obj);
                return new JsonResult(result);
            }
            catch (InvalidOperationException ioEx)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed, ioEx.Message);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        ///     Edit a work order by id
        /// </summary>
        /// <param id="id"></param>
        /// <returns>All fields that will be editables of the work order</returns>
        //GET: api/workorders/update/<id>
        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadWorkOrders", "ReadWorkOrderFromCalendar")]
        public async Task<ActionResult<WorkOrderUpdateViewModel>> Update(int id)
        {
            return await this.GetWorkOrder(id);
        }

        [HttpPut]
        [PermissionsFilter("UpdateWorkOrders", "UpdateWorkOrderFromCalendar")]
        public async Task<IActionResult> Update([FromBody] WorkOrderUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                WorkOrder obj = await this.AppService.SingleOrDefaultDapperAsync(vm.ID);

                // no update tasks
                vm.UpdateTasks = false;

                // set draft when due date is null
                if (!vm.DueDate.HasValue)
                {
                    vm.StatusId = WorkOrderStatus.Draft;
                }

                // set status to stand by
                if (obj.StatusId == 0 && vm.DueDate.HasValue)
                {
                    vm.StatusId = WorkOrderStatus.StandBy;
                }

                // Ticket MG32
                if (!vm.Priority.HasValue)
                {
                    vm.Priority = WorkOrderPriority.Low;
                }

                vm.CalendarItemFrequencyId = obj.CalendarItemFrequencyId;
                vm.WorkOrderScheduleSettingId = obj.WorkOrderScheduleSettingId;

                if (obj == null)
                {
                    return this.NoContent();
                }

                int completedTasks = 0;
                int countTasks = 0;
                if (vm.UpdateTasks)
                {
                    completedTasks = vm.Tasks.Count(task => task.IsComplete == true);
                    countTasks = vm.Tasks.Count();
                }
                else
                {
                    var workOrderTask = await this.AppService.ReadAllTasksAsync(vm.ID);
                    completedTasks = workOrderTask.Count(task => task.IsComplete == true);
                    countTasks = workOrderTask.Count();
                }

                if (vm.StatusId == WorkOrderStatus.Closed)
                {
                    if (!vm.IsCloseable)
                    {
                        return StatusCode(
                            (int)HttpStatusCode.PreconditionFailed,
                            "This Work Order cannot be closed because it is not due yet");
                    }
                    if (completedTasks != countTasks)
                    {
                        return StatusCode(
                                (int)HttpStatusCode.PreconditionFailed,
                                "This Work Order cannot be closed because there are uncompleted tasks");
                    }
                    if (obj.StatusId == WorkOrderStatus.Closed)
                    {
                        return StatusCode(
                                (int)HttpStatusCode.PreconditionFailed,
                                "This Work Order is already closed");
                    }
                }

                //delete and assign workOrder Employees

                var employeesByWorkOrder = await this.AppService.GetEmployeesIdsAsync(vm.ID, WorkOrderEmployeeType.Any);

                foreach (int employeeWorkOrderId in employeesByWorkOrder)
                {
                    await this.AppService.UnassignEmployee(vm.ID, employeeWorkOrderId);
                }

                await AppService.SaveChangesAsync();

                // get supervisors by buildingId
                var employeesByBuilding = await this._buildingAppService.GetEmployeesByBuildingId(vm.BuildingId);

                // assign the supervisors or operations manager to work order        
                foreach (EmployeeBuildingViewModel employee in employeesByBuilding)
                {
                    await this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type);
                }
                // user selected employees
                foreach (var employee in vm.AssignedEmployees)
                {
                    await this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type);
                }

                await this.AppService.SaveChangesAsync();
                // HACK: Copying Task and notes in memory for futher
                // usage

                var prevTasks = new Dictionary<int, WorkOrderTask>();
                var prevNotes = new Dictionary<int, WorkOrderNote>();

                // Removes Tasks
                if (vm.UpdateTasks)
                {
                    foreach (var task in obj.Tasks)
                    {
                        prevTasks[task.ID] = this.Mapper.Map<WorkOrderTask, WorkOrderTask>(task);
                        await this.AppService.RemoveTaskAsync(task.ID);
                    }
                }

                // Removes Notes
                foreach (var note in obj.Notes)
                {
                    prevNotes[note.ID] = this.Mapper.Map<WorkOrderNote, WorkOrderNote>(note);
                    await this.AppService.RemoveNoteAsync(note.ID);
                }

                // Removes Attachments in azure
                IEnumerable<int> atts = vm.Attachments.Select(v => v.ID);
                List<int> toDelete = obj.Attachments.Select(a => a.ID).Except(atts).ToList();
                foreach (var attId in toDelete)
                {
                    // Removes not inlcuded attachments from azure storage
                    var att = obj.Attachments.FirstOrDefault(a => a.ID == attId);
                    await _azureStorage.DeleteImageAsync(blobName: att.BlobName);
                }

                //foreach (var att in obj.Attachments)
                //{
                //    await this.AppService.RemoveAttachmentsAsync(att.ID);
                //}

                // Update Attachments
                foreach (var attach in obj.Attachments)
                {
                    var AttachmentsToUpdate = vm.Attachments.SingleOrDefault(da => da.ID == attach.ID);

                    if (AttachmentsToUpdate == null)
                    {
                        continue;
                    }
                    if (AttachmentsToUpdate?.Description != attach.Description)
                    {
                        attach.Description = AttachmentsToUpdate.Description;
                        await this.AppService.UpdateAttachmentsAsync(attach);
                    }
                }

                var building = obj.Building;

                if (obj.BuildingId != vm.BuildingId)
                {
                    building = this.AppService.GetBuilding(vm.BuildingId);
                }

                // Updates work order in DB
                var woTasks = new List<WorkOrderTask>(obj.Tasks);
                var woAttachments = new List<WorkOrderAttachment>(obj.Attachments);

                this.Mapper.Map(vm, obj);
                obj.Tasks = woTasks;

                if (vm.UpdateTasks)
                {
                    obj.Tasks.Clear();
                }
                obj.Notes.Clear();


                obj.Attachments = woAttachments;

                try
                {
                    if (obj.StatusId == WorkOrderStatus.Closed || obj.StatusId == WorkOrderStatus.Cancelled)
                    {
                        await this.AppService.ValidateExistingInspectionReferenced(obj.ID);
                    }
                }
                catch (Exception ex)
                {

                }

                await this.AppService.UpdateAsync(obj);
                await this.AppService.SaveChangesAsync();

                // Add tasks from WorkOrderUpdateViewModel        
                if (vm.UpdateTasks)
                {
                    foreach (WorkOrderTaskUpdateViewModel objTask in vm.Tasks)
                    {
                        var newTaskObj = new WorkOrderTask
                        {
                            WorkOrderId = vm.ID,
                            // mark as "complete" when status is Closed (REMOVED BY NOW)
                            //IsComplete = vm.StatusId.Equals((int)WorkOrderStatus.Closed) ? true : objTask.IsComplete,
                            IsComplete = objTask.IsComplete,
                            Description = objTask.Description,
                            ServiceId = objTask.ServiceId,
                            UnitPrice = objTask.UnitPrice ?? 0,
                            Quantity = objTask.Quantity ?? 0,
                            DiscountPercentage = objTask.DiscountPercentage ?? 0,
                            Note = objTask.Note
                        };
                        // Updating Task
                        if (prevTasks.TryGetValue(objTask.ID, out WorkOrderTask prevTask))
                        {
                            newTaskObj.CreatedBy = prevTask.CreatedBy;
                            newTaskObj.CreatedDate = prevTask.CreatedDate;
                            newTaskObj.LastCheckedDate = prevTask.LastCheckedDate;
                            // If task was marked as 'completed'
                            // We set a new checked date
                            if (newTaskObj.IsComplete && !prevTask.IsComplete)
                            {
                                newTaskObj.LastCheckedDate = DateTime.UtcNow;
                            }
                        }
                        // Creating a new one
                        else
                        {
                            newTaskObj.LastCheckedDate = newTaskObj.IsComplete ? DateTime.UtcNow : new DateTime();
                        }

                        await this.AppService.AddTaskAsync(newTaskObj);
                    }
                }

                // Add notes from WorkOrderUpdateViewModel        
                foreach (WorkOrderNoteUpdateViewModel objNote in vm.Notes)
                {
                    var newNoteObj = new WorkOrderNote()
                    {
                        WorkOrderId = vm.ID,
                        Note = objNote.Note,
                        EmployeeId = objNote.EmployeeId,
                    };
                    // Updating Note
                    if (prevNotes.TryGetValue(objNote.ID, out WorkOrderNote prevNote))
                    {
                        newNoteObj.CreatedBy = prevNote.CreatedBy;
                        newNoteObj.CreatedDate = prevNote.CreatedDate;
                    }
                    await this.AppService.AddNoteAsync(newNoteObj);
                }

                // Added or deleted Attachments in DB
                var checkAttachments = await this.AppService.CheckAttachmentsAsync(vm.Attachments, obj.ID);

                //foreach (var objAttachment in vm.Attachments)
                //{
                //    var newObjAttachment = new WorkOrderAttachment
                //    {
                //        WorkOrderId = vm.ID,
                //        EmployeeId = objAttachment.EmployeeId,
                //        BlobName = objAttachment.BlobName,
                //        FullUrl = objAttachment.FullUrl,
                //        ImageTakenDate = objAttachment.ImageTakenDate,
                //        Description = objAttachment.Description
                //    };
                //    await this.AppService.AddAttachmentAsync(newObjAttachment);
                //}

                await this.AppService.SaveChangesAsync();

                obj.Building = building;
                var result = this.Mapper.Map<WorkOrder, WorkOrderUpdateViewModel>(obj);

                return new JsonResult(result);
            }

            catch (InvalidOperationException ioEx)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed, ioEx.Message);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [PermissionsFilter("UpdateSnoozeDateWorkOrders")]
        public async Task<IActionResult> UpdateSnoozeDate([FromBody] WorkOrderUpdateViewModel vm)
        {
            try
            {
                WorkOrder obj = await this.AppService.SingleOrDefaultAsync(w => w.ID == vm.ID);

                if (obj == null)
                {
                    return this.NoContent();
                }

                obj.DueDate = vm.SnoozeDate ?? new DateTime();
                obj.SnoozeDate = vm.SnoozeDate;

                await this.AppService.UpdateAsync(obj);

                await this.AppService.SaveChangesAsync();

                return Ok("Due Date Updated");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllWOSourcesCbo(DataSourceRequest request)
        {
            var wosVM = await this.AppService.ReadAllWOSourceCboDapperAsync(request);
            return new JsonResult(wosVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteWorkOrders")]
        public async Task<IActionResult> DeleteWO(int id)
        {
            var obj = this.AppService.SingleOrDefault(wo => wo.ID == id);
            if (obj == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, "Error finding work order");
            }
            try
            {
                await this.AppService.RemoveAsync(obj);
                await this.AppService.SaveChangesAsync();
                return this.Ok();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed, "This work order is being referenced by other entities");
            }
        }

        [HttpPut]
        [PermissionsFilter("CloseWorkOrders")]
        public async Task<IActionResult> ClosedWorkOrder([FromBody] WorkOrderCloseViewModel closeViewModel)
        {
            try
            {
                var obj = await this.AppService.SingleOrDefaultDapperAsync(closeViewModel.WorkOrderId);
                if (obj == null)
                {
                    return this.NotFound(this.ModelState);
                }

                // Cant close because not has a due date assigned
                if (!obj.DueDate.HasValue)
                {
                    throw new InvalidOperationException($"This work order cannot be closed because it has not yet been assigned a due date");
                }

                if (DateTime.UtcNow <= obj.DueDate.Value)
                {
                    throw new InvalidOperationException($"This Work Order can not be closed because it is not due yet");
                }

                if (obj.Tasks.Any(t => t.IsComplete == false))
                {
                    throw new InvalidOperationException($"This Work Order cannot be closed because there are incompleted tasks");
                }

                if (obj.StatusId == WorkOrderStatus.Closed || obj.StatusId == WorkOrderStatus.Cancelled)
                {
                    throw new InvalidOperationException($"This Work Order is already closed");
                }

                obj.FollowUpOnClosingNotes = closeViewModel.FollowUpOnClosingNotes;
                obj.StatusId = WorkOrderStatus.Closed;

                if (string.IsNullOrEmpty(closeViewModel.ClosingNotes) == false)
                {
                    obj.ClosingNotes = closeViewModel.ClosingNotes;
                }

                if (closeViewModel.AdditionalNotes != WorkOrderClosingNotes.None)
                {
                    obj.AdditionalNotes = closeViewModel.AdditionalNotes;
                }

                obj.ClosingNotesOther = closeViewModel.ClosingNotesOther;

                //Complete all tasks of the work order
                /*foreach (WorkOrderTask objTask in obj.Tasks)
                {
                    objTask.IsComplete = true;
                    await this.AppService.UpdateTaskAsync(objTask);
                }*/

                await this.AppService.UpdateAsync(obj);

                // HACK: Mark the work order as "changed"
                this.AppService.MarkWorkOrderAsChanged(obj);

                await this.AppService.SaveChangesAsync();

                return this.Ok();
            }
            catch (InvalidOperationException ioEx)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed, ioEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [PermissionsFilter("ReadWorkOrders")]
        public async Task<JsonResult> GetDashboardData(int employeeId)
        {
            var dashboardData = await this.AppService.GetDashboardDataDapperAsync(employeeId);
            return new JsonResult(dashboardData);
        }

        #endregion

        #region Notes
        [HttpGet]
        public async Task<JsonResult> ReadAllNotes(DataSourceRequest request, int woId)
        {
            var objsVM = await this.AppService.ReadAllNotesDapperAsync(request, woId);
            return new JsonResult(objsVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteWorkOrdersNotes")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var obj = await this.AppService.GetNoteAsync(n => n.ID == id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }

            await this.AppService.RemoveNoteAsync(id);
            await this.AppService.SaveChangesAsync();
            return this.Ok();
        }

        [HttpPost]
        [PermissionsFilter("AddWorkOrdersNotes")]
        public async Task<IActionResult> AddNote([FromBody] WorkOrderNoteCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<WorkOrderNoteCreateViewModel, WorkOrderNote>(vm);
            await this.AppService.AddNoteAsync(obj);
            await this.AppService.SaveChangesAsync();
            return new JsonResult(obj);
        }

        [HttpPut]
        [PermissionsFilter("UpdateWorkOrdersNotes")]
        public async Task<IActionResult> UpdateNote([FromBody] WorkOrderNoteUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = await this.AppService.GetNoteAsync(w => w.ID == vm.ID);
            if (obj == null)
            {
                return this.BadRequest(this.ModelState);
            }
            this.Mapper.Map(vm, obj);
            await this.AppService.UpdateNoteAsync(obj);
            await this.AppService.SaveChangesAsync();
            return new JsonResult(obj);
        }

        #endregion

        #region Tasks
        [HttpGet]
        public async Task<JsonResult> ReadAllTasks(DataSourceRequest request, int woId)
        {
            var objsVM = await this.AppService.ReadAllTasksDapperAsync(request, woId);
            return new JsonResult(objsVM);
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllWorkOrderTasks(int workOrderId)
        {
            var result = await this.AppService.ReadAllTasksAsync(workOrderId);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkOrderTask(int id)
        {
            var result = await this.AppService.GetWorkOrderTaskAsync(id);
            if (result != null)
            {
                return new JsonResult(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpDelete]
        [PermissionsFilter("DeleteWorkOrdersTasks")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var obj = await this.AppService.GetTaskAsync(n => n.ID == id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }

            await this.AppService.RemoveTaskAsync(id);
            await this.AppService.SaveChangesAsync(sendNotifications: false);
            return this.Ok();
        }

        [HttpPost]
        [PermissionsFilter("AddWorkOrdersTasks")]
        public async Task<IActionResult> AddTask([FromBody] WorkOrderTaskCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<WorkOrderTaskCreateViewModel, WorkOrderTask>(vm);
            await this.AppService.AddTaskAsync(obj);
            await this.AppService.SaveChangesAsync(sendNotifications: false);

            var result = await this.AppService.GetWorkOrderTaskAsync(obj.ID);
            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateWorkOrdersTasks")]
        public async Task<IActionResult> UpdateTask([FromBody] WorkOrderTaskUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (vm == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Empty payload");
            }

            var obj = await this.AppService.GetTaskAsync(w => w.ID == vm.ID);
            if (obj == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, "Error finding task");
            }

            //  prevent foreign key error
            vm.ServiceId = vm.ServiceId == 0 ? null : vm.ServiceId;
            vm.WorkOrderServiceCategoryId = vm.WorkOrderServiceCategoryId == 0 ? null : vm.WorkOrderServiceCategoryId;
            vm.WorkOrderServiceId = vm.WorkOrderServiceId == 0 ? null : vm.WorkOrderServiceId;

            // Only if prev was not complete and now it is
            bool isChecked = vm.IsComplete && !obj.IsComplete;
            this.Mapper.Map(vm, obj);
            if (isChecked)
            {
                obj.LastCheckedDate = DateTime.UtcNow;
            }

            var attachments = new List<WorkOrderTaskAttachment>(obj.Attachments);
            obj.Attachments.Clear();
            obj.Attachments = attachments.Where(a => a.ID == 0).ToList();

            await this.AppService.UpdateTaskAsync(obj);
            await this.AppService.SaveChangesAsync(sendNotifications: false);

            var resultVM = this.Mapper.Map<WorkOrderTask, WorkOrderTaskDetailsViewModel>(obj);
            return new JsonResult(resultVM);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> UpdateTask(int id)
        {
            var obj = await this.AppService.GetTaskAsync(w => w.ID == id);
            if (obj == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, "Error finding task");
            }
            var resultVM = this.Mapper.Map<WorkOrderTask, WorkOrderTaskDetailsViewModel>(obj);
            return new JsonResult(resultVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] WorkOrderTaskUpdateCompletedStatusViewModel vm)
        {
            try
            {
                var workOrderTask = await this.AppService.GetTaskAsync(t => t.ID == vm.Id);
                if (workOrderTask != null)
                {
                    workOrderTask.IsComplete = vm.IsComplete;
                    if (workOrderTask.IsComplete)
                    {
                        workOrderTask.LastCheckedDate = DateTime.UtcNow;
                        workOrderTask.QuantityExecuted = vm.QuantityExecuted;
                        workOrderTask.HoursExecuted = vm.HoursExecuted;
                        workOrderTask.CompletedDate = vm.CompletedDate;
                    }
                    else
                    {
                        workOrderTask.CompletedDate = null;
                    }
                    await this.AppService.UpdateTaskAsync(workOrderTask);
                    await this.AppService.SaveChangesAsync(sendNotifications: false);
                    return Ok();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Attachments

        [HttpPost]
        public async Task<IActionResult> UploadAttachments([FromForm] string azureStorageDirectory = "img")
        {
            IFormFileCollection files = Request.Form.Files;

            if (files.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "The provided files collection can not be empty!");
            }

            List<string> notUploaded = new List<string>();
            List<ImageUploadViewModel> uploaded = new List<ImageUploadViewModel>();

            try
            {
                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
                    {
                        Stream imageFile = file.OpenReadStream();
                        if (imageFile != null)
                        {
                            // Tuple<string, string, DateTime>
                            var result = await _azureStorage.UploadImageAsync(imageFile, azureStorageDirectory, contentType: file.ContentType);

                            uploaded.Add(new ImageUploadViewModel
                            {
                                FileName = file.FileName,
                                BlobName = result.Item1,
                                FullUrl = result.Item2,
                                ImageTakenDate = result.Item3
                            });
                        }
                        else
                        {
                            notUploaded.Add(file.FileName);
                        }
                    }
                    else
                    {
                        notUploaded.Add(file.FileName);
                    }
                }

                if (notUploaded.Count.Equals(0))
                {
                    return new JsonResult(uploaded);
                }

                if (notUploaded.Count < files.Count)
                {
                    return StatusCode(
                        (int)HttpStatusCode.PartialContent,
                        string.Format("The following files were not uploaded: {0}", string.Join(", ", notUploaded))
                    );
                }

                return StatusCode((int)HttpStatusCode.NoContent, "Any file was uploaded!");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                if (notUploaded.Count < files.Count)
                {
                    return StatusCode(
                        (int)HttpStatusCode.PartialContent,
                        string.Format("The following images did not get uploaded: {0}", string.Join(", ", notUploaded))
                    );
                }

                return StatusCode((int)HttpStatusCode.NoContent, "Any file was uploaded!");
            }
        }

        [HttpDelete]
        [PermissionsFilter("DeleteWorkOrdersAttachment")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var obj = await this.AppService.GetAttachmentAsync(a => a.ID.Equals(id));
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }

            try
            {
                // TODO: the container may change, take this in consideration
                bool result = await _azureStorage.DeleteImageAsync(blobName: obj.BlobName);
#if DEBUG
                Console.WriteLine($"azure delete result: {result}");
#endif
            }
            catch (Exception)
            {
                // It does not matter, let the file remains in the storage. 
                // The most important thing is to remove it from DB
            }

            await this.AppService.RemoveAttachmentsAsync(id);
            await this.AppService.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [PermissionsFilter("DeleteWorkOrdersAttachment")]
        public async Task<IActionResult> DeleteAttachmentByBlobName(string blobName)
        {
            try
            {
                // TODO: the container may change, take this in consideration
                bool result = await _azureStorage.DeleteImageAsync(blobName: blobName);
#if DEBUG
                Console.WriteLine($"azure delete result: {result}");
#endif
            }
            catch (Exception)
            {
                // It does not matter, let the file remains in the storage. 
                // The most important thing is to remove it from DB
            }

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> ReadDailyReportByOperationsManager(DataSourceRequestWOReadAll request, int? operationsManagerId)
        {
            var woDailyReport = await this.AppService.DailyReportByOperationsManagerDapperAsync(request, operationsManagerId);
            return new JsonResult(woDailyReport);
        }

        #endregion Attachments

        #region Billable Tasks
        [HttpGet]
        [PermissionsFilter("ReadWorkOrderBillableReport")]
        public async Task<ActionResult<DataSource<WorkOrderBillingReportGridViewModel>>> ReadBillingReport(DataSourceRequestBillingReport request)
        {
            try
            {
                var vm = await this.AppService.ReadBillingReportDapperAsync(request);
                return new JsonResult(vm);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpGet]
        [PermissionsFilter("ReadWorkOrderBillableReport")]
        public async Task<FileResult> ReadBillingReportCsv(DataSourceRequestBillingReport request)
        {
            // HACK: Workaround with UTC dates

            // var strTimezoneOffset = "300";
            // Timezone offset received in http headers
            var strTimezoneOffset = this.HttpContext?.Request?.Headers["TimezoneOffset"];
            var timeZoneId = this.HttpContext?.Request?.Headers["TimezoneId"];

            if (string.IsNullOrEmpty(timeZoneId))
            {
                timeZoneId = "America/New_York";
            }

            if (string.IsNullOrEmpty(strTimezoneOffset))
            {
                strTimezoneOffset = "300";
            }

            var timezoneOffset = (int)Math.Round(double.Parse(strTimezoneOffset));

            // HACK: To ensure all records are included within the daterange filter
            request.PageSize = int.MaxValue;
            request.PageNumber = 0;
            var vm = await this.AppService.ReadBillingReportDapperAsync(request);

            if (vm?.Payload?.Any() == true)
            {
                foreach (var item in vm.Payload)
                {
                    // Adjusting dates
                    item.CompletedDate = this.AdjustDateTime(item.EpochTaskCompletedDate, timezoneOffset, timeZoneId);
                    // item.TaskCreatedDate = this.AdjustDateTime(item.TaskCreatedDate, timezoneOffset);
                    item.WorkOrderCreatedDate = this.AdjustDateTime(item.EpochWorkOrderCreatedDate, timezoneOffset, timeZoneId);
                    item.WorkOrderCompletedDate = this.AdjustDateTime(item.EpochWorkOrderCompletedDate, timezoneOffset, timeZoneId);
                }
            }

            var csv = vm.Payload.ToCsv(true, "WorkOrderGuid", "WorkOrderId", "ClonePath", "OriginWorkOrderId", "Count", "ID",
                                        "EpochTaskUpdatedDate", "EpochTaskCompletedDate", "EpochTaskCreatedDate", "EpochWorkOrderCreatedDate",
                                        "BuildingBillingFullName", "BuildingBillingEmail", "CompletedDate", "EpochWorkOrderCompletedDate", "WorkOrderCreatedDate", "WorkOrderCompletedDate",
                                        "ServicePrice", "ServicePriceText", "TaskUnitPrice", "BuildingBillingInfo", "WorkOrderCreatedDateText", "IsTaskChecked", "UnitFactor", "TUnitPrice", "OldVersion");
            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return File(bytes, "text/csv", "report.csv");
        }

        /// <summary>
        /// Adjusts the date time (if applicable) given an offset.
        /// </summary>
        /// <returns>The date time.</returns>
        /// <param name="epoch">Unix timestamp</param>
        /// <param name="timezoneOffset">Timezone offset.</param>
        /// <param name="timezoneId">Timezone identifer.</param>
        private DateTime AdjustDateTime(int epoch, int timezoneOffset, string timezoneId)
        {
            if (epoch <= 0)
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            epoch -= timezoneOffset * 60;
            var dt = epoch.FromEpoch();
            // var ts = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            var ts = TZConvert.GetTimeZoneInfo(timezoneId);
            if (!ts.IsDaylightSavingTime(dt))
            {
                dt = dt.AddHours(-1);
            }
            return dt;
        }

        [HttpGet]
        [PermissionsFilter("ReadWorkOrderBillableReport")]
        public async Task<IActionResult> GetBillingDocumentReportUrl(DataSourceRequestBillingReport request)
        {
            try
            {
                var strTimezoneOffset = this.HttpContext?.Request?.Headers["TimezoneOffset"];
                var timeZoneId = this.HttpContext?.Request?.Headers["TimezoneId"];

                if (string.IsNullOrEmpty(timeZoneId))
                {
                    timeZoneId = "America/New_York";
                }

                if (string.IsNullOrEmpty(strTimezoneOffset))
                {
                    strTimezoneOffset = "300";
                }

                var timezoneOffset = (int)Math.Round(double.Parse(strTimezoneOffset));

                request.PageSize = int.MaxValue;
                request.PageNumber = 0;
                var vm = await this.AppService.ReadBillingReportDapperAsync(request);

                if (vm?.Payload?.Any() == true)
                {
                    foreach (var item in vm.Payload)
                    {
                        // Adjusting dates
                        item.CompletedDate = this.AdjustDateTime(item.EpochTaskCompletedDate, timezoneOffset, timeZoneId);
                        // item.TaskCreatedDate = this.AdjustDateTime(item.TaskCreatedDate, timezoneOffset);
                        item.WorkOrderCreatedDate = this.AdjustDateTime(item.EpochWorkOrderCreatedDate, timezoneOffset, timeZoneId);
                        item.WorkOrderCompletedDate = this.AdjustDateTime(item.EpochWorkOrderCompletedDate, timezoneOffset, timeZoneId);
                    }
                }

                var base64 = await this.AppService.GetBillingDocumentReportUrl(vm.Payload);
                return new JsonResult(base64);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();

            }
        }
        #endregion

        #region PreCalendar
        [HttpPost]
        [PermissionsFilter("AddWorkOrderFromPreCalendar")]
        public async Task<ActionResult> AddWorkOrdenFromPreCalendar([FromBody] WorkOrderFromPreCalendarCreateViewModel wo)
        {
            if (!this.ModelState.IsValid || wo == null)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                WorkOrderCreateViewModel vm = new WorkOrderCreateViewModel()
                {
                    BuildingId = wo.BuildingId,
                    Location = wo.Location,
                    AdministratorId = wo.AdministratorId,
                    Priority = wo.Priority,
                    SendRequesterNotifications = wo.SendRequesterNotifications,
                    SendPropertyManagersNotifications = wo.SendPropertyManagersNotifications,
                    StatusId = wo.StatusId,
                    Number = wo.Number,
                    Description = wo.Description,
                    DueDate = wo.DueDate,
                    SnoozeDate = wo.SnoozeDate,
                    Type = wo.Type,
                    WorkOrderSourceId = wo.WorkOrderSourceId,
                    BillingName = wo.BillingName,
                    BillingEmail = wo.BillingEmail,
                    BillingNote = wo.BillingNote,
                    ClosingNotes = wo.ClosingNotes,
                    AdditionalNotes = wo.AdditionalNotes,
                    ClosingNotesOther = wo.ClosingNotesOther,
                    OriginWorkOrderId = wo.OriginWorkOrderId,
                    ClonePath = wo.ClonePath,
                    OriginWorkOrderNumber = wo.OriginWorkOrderNumber,
                    Guid = wo.Guid,

                    Attachments = wo.Attachments,
                    Notes = wo.Notes,
                    Tasks = wo.Tasks,
                    RequesterEmail = wo.RequesterEmail,
                    RequesterFullName = wo.RequesterFullName

                };

                for (int i = 0; i < wo.Quantity; i++)
                {

                    var obj = this.Mapper.Map<WorkOrderCreateViewModel, WorkOrder>(vm);

                    await this.AppService.AddAsync(obj);
                    await this.AppService.SaveChangesAsync(sendNotifications: false);

                    // get employees by buildingId
                    var employeesByBuilding = await this._buildingAppService.GetEmployeesByBuildingId(vm.BuildingId);

                    // assign the building employees to work order        
                    foreach (EmployeeBuildingViewModel employee in employeesByBuilding)
                    {
                        await this.AppService.AssignEmployee(obj.ID, employee.ID, (int)employee.Type);
                    }

                    await this.AppService.SaveChangesAsync(sendNotifications: false);

                    var isChecked = await this.AppService.CheckAssignedEmployeeAsync(obj);
                    if (isChecked)
                    {
                        await this.AppService.SaveChangesAsync();
                    }

                    // ++Date

                    string option = wo.Periodicity.ToString();

                    switch (option)
                    {
                        case "Monthly":
                            vm.SnoozeDate = vm.DueDate.Value.AddMonths(1);
                            vm.DueDate = vm.DueDate.Value.AddMonths(1);
                            break;
                        case "Weekly":
                            vm.SnoozeDate = vm.DueDate.Value.AddDays(7);
                            vm.DueDate = vm.DueDate.Value.AddDays(7);
                            break;
                        case "BiWeekly":
                            vm.SnoozeDate = vm.DueDate.Value.AddDays(14);
                            vm.DueDate = vm.DueDate.Value.AddDays(14);
                            break;
                        default:
                            break;
                    }
                }

                return Ok();
            }
            catch (InvalidOperationException ioEx)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed, ioEx.Message);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        #endregion

        #region Calendar
        [HttpPut]
        [PermissionsFilter("UpdateWorkOrderFromCalendar")]
        public async Task<IActionResult> UpdateWorkOrderTaskSummary([FromBody] WorkOrderTaskSummaryUpdateViewModel wo)
        {
            if (wo == null || !this.ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var result = await this.AppService.UpdateWorkOrderTaskSummary(wo);

                await this.AppService.SaveChangesAsync(sendNotifications: false);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Schedule Settings
        [HttpPost]
        public async Task<IActionResult> AddScheduleSettings([FromBody] WorkOrderScheduleSettingCreateViewModel vm)
        {
            if (vm == null || !this.ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var obj = this.Mapper.Map<WorkOrderScheduleSettingCreateViewModel, WorkOrderScheduleSetting>(vm);
                var result = await this.AppService.AddScheduleSettings(obj);
                await this.AppService.SaveChangesAsync(sendNotifications: false);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
