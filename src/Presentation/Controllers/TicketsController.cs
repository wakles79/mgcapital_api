using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Freshdesk;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MGCap.Domain.ViewModels.GMailApi;
using MGCap.Domain.Utils;

namespace MGCap.Presentation.Controllers
{
    public class TicketsController : BaseEntityController<Ticket, int>
    {

        public new ITicketsApplicationService AppService => base.AppService as ITicketsApplicationService;
        private readonly IEmailSender _emailSender;
        private readonly IFreshdeskApplicationService _freshdeskApplicationService;
        private readonly IGMailApiService _gmailApiService;
        IAzureStorage _azureStorage;

        public TicketsController(
            IEmployeesApplicationService employeeAppService,
            ITicketsApplicationService ticketAppService,
            IFreshdeskApplicationService freshdeskApplicationService,
            IEmailSender emailSender,
            IAzureStorage azureStorage,
            IGMailApiService gmailApiService,
            IMapper mapper) : base(employeeAppService, ticketAppService, mapper)//base(employeeAppService, mapper)
        {
            //this.AppService = ticketAppService;
            this._azureStorage = azureStorage;
            this._emailSender = emailSender;
            this._freshdeskApplicationService = freshdeskApplicationService;
            this._gmailApiService = gmailApiService;
        }

        #region Ticket
        // GET: api/tickets/readall
        [HttpGet]
        [PermissionsFilter("ReadTickets")]
        public async Task<IActionResult> ReadAll(DataSourceRequestTicket request)
        {
            var dataSource = await this.AppService.ReadAllDapperAsync(request);
            return new JsonResult(dataSource);
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllCboToMerge(int paramType, string value)
        {
            try
            {
                var result = await this.AppService.ReadAllToMergeAsync(paramType, value);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> MergeTickets([FromBody] TicketMergeViewModel mergeViewModel)
        {
            if (!this.ModelState.IsValid || mergeViewModel == null)
            {
                return BadRequest(this.ModelState);
            }

            try
            {
                await this.AppService.MergeTicketsAsync(mergeViewModel);
                await this.AppService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new ticket
        /// </summary>
        /// <param name="vm">TicketCreateViewModel vm</param>
        /// <returns>The 'Guid' and 'ID' of the created ticket</returns>
        [HttpPost]
        [PermissionsFilter("AddTickets", "AddTicketFromInspectionItem", "AddTicketFromCalendar")]
        public async Task<ActionResult> Add([FromBody] TicketCreateViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                if (vm == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, $"Empty payload.");
                }

                try
                {
                    var obj = this.Mapper.Map<TicketCreateViewModel, Ticket>(vm);

                    // HACK: In case location comes as a plain field
                    if (!string.IsNullOrEmpty(vm.Location))
                    {
                        obj.Data["Location"] = vm.Location;
                    }

                    obj.PendingReview = true;
                    var createdTicket = await this.AppService.AddAsync(obj);
                    await this.AppService.SaveChangesAsync(false);

                    #region Gmail
                    var msgId = await this._gmailApiService.AddFromTicket(obj);
                    if (msgId != null)
                    {
                        createdTicket.MessageId = msgId;
                        await this.AppService.UpdateAsync(createdTicket);
                        await this.AppService.SaveChangesAsync();
                    }
                    #endregion Gmail

                    #region Freshdesk
                    //var fdTicket = await this._freshdeskApplicationService.AddFromTicket(obj);
                    //if (fdTicket != null)
                    //{
                    //    long fdTicketId = fdTicket.Id;
                    //    if (fdTicketId <= int.MaxValue)
                    //    {
                    //        createdTicket.FreshdeskTicketId = System.Convert.ToInt32(fdTicketId);
                    //        await this.AppService.UpdateAsync(createdTicket);
                    //        await this.AppService.SaveChangesAsync();
                    //    }
                    //}
                    #endregion Freshdesk

                    return new JsonResult(new
                    {
                        guid = obj.Guid.ToString(),
                        id = obj.ID
                    });
                }
                catch (Exception ex)
                {
                    // TODO: Catch proper error with foreign key exceptions
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, ex.Message);
                }
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadTickets")]
        public async Task<ActionResult<TicketUpdateViewModel>> Update(int id)
        {
            try
            {
                bool updated = await this.AppService.AssignCurrentEmployeeToTicket(id);
                if (updated)
                {
                    await this.AppService.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Nothing
            }


            var obj = await AppService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (obj == null)
            {
                return this.NoContent();
            }

            var vm = this.Mapper.Map<Ticket, TicketUpdateViewModel>(obj);

            // HACK: To Add location
            if (obj.Data.ContainsKey("Location"))
            {
                vm.Location = obj.Data["Location"];
            }

            return new JsonResult(vm);
        }

        [HttpPut]
        [PermissionsFilter("UpdateTickets", "UpdateTicketFromCalendar")]
        public async Task<ActionResult<TicketDetailsViewModel>> Update([FromBody] TicketUpdateViewModel vm)
        {
            #region MG-15
            if (this.ModelState.IsValid)
            {
                if (vm == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Empty payload.");
                }

                // Recover object from DataBase
                var dbObj = await AppService.SingleOrDefaultAsync(ent => ent.ID == vm.ID);
                if (dbObj == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Error finding ticket");
                }

                try
                {
                    //// Verify if there are changes
                    //bool hasChanges = false;
                    //// Verify Assigned Employee change
                    //if (vm.AssignedEmployeeId != dbObj.AssignedEmployeeId) hasChanges = true;

                    // Verify Status change
                    bool statusUpdated = vm.Status != dbObj.Status;
                    //if (statusUpdated) hasChanges = true;
                    // Verify Atachments changes


                    // Prevent remove this value
                    vm.FreshdeskTicketId = dbObj.FreshdeskTicketId.HasValue ? dbObj.FreshdeskTicketId : null;
                    vm.ParentId = dbObj.ParentId.HasValue ? dbObj.ParentId : null;

                    if (vm.AssignedEmployeeId != dbObj.AssignedEmployeeId)
                    {
                        if (vm.AssignedEmployeeId.HasValue)
                        {
                            await this.AppService.RegisterEmployeeAssignment(vm.ID, vm.AssignedEmployeeId.Value);
                        }
                    }

                    ////Delete exist attachments in DB
                    //IEnumerable<int> atts = vm.Attachments.Select(v => v.ID);
                    //List<int> toDelete = dbObj.Attachments.Select(a => a.ID).Except(atts).ToList();
                    //while (toDelete.Count > 0)
                    //{
                    //    await this.AppService.RemoveAttachmentsAsync(toDelete.ElementAt(0));
                    //    toDelete.RemoveAt(0);
                    //}
                    // Removes Attachments in azure
                    IEnumerable<int> atts = vm.Attachments.Select(v => v.ID);
                    List<int> toDelete = dbObj.Attachments.Select(a => a.ID).Except(atts).ToList();
                    foreach (var attId in toDelete)
                    {
                        // Removes not inlcuded attachments from azure storage
                        var att = dbObj.Attachments.FirstOrDefault(a => a.ID == attId);
                        await _azureStorage.DeleteImageAsync(blobName: att.BlobName);
                    }

                    foreach (var attach in dbObj.Attachments)
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

                    var ticketAttachments = new List<TicketAttachment>(dbObj.Attachments);

                    this.Mapper.Map(vm, dbObj);

                    dbObj.Attachments = ticketAttachments;

                    //dbObj.Attachments.Clear();

                    //await this.AppService.SaveChangesAsync();
                    // HACK: In case location comes as a plain field
                    if (!string.IsNullOrEmpty(vm.Location))
                    {
                        dbObj.Data["Location"] = vm.Location;
                    }

                    await this.AppService.UpdateAsync(dbObj);
                    await this.AppService.SaveChangesAsync();

                    if (dbObj.FreshdeskTicketId.HasValue)
                    {
                        await this._freshdeskApplicationService.UpdateTicket(dbObj);
                    }

                    await this.AppService.SaveChangesAsync();

                    //var checkAttachments = await this.AppService.CheckAttachmentsAsync(vm.Attachments, dbObj.ID);

                    // Deprecated
                    //Add attachments from TicketUpdateViewModel
                    //foreach (TicketAttachmentUpdateViewModel objAttachment in vm.Attachments)
                    //{
                    //    var newObjAttachment = new TicketAttachment
                    //    {
                    //        TicketId = vm.ID,
                    //        BlobName = objAttachment.BlobName,
                    //        FullUrl = objAttachment.FullUrl,
                    //        Description = objAttachment.Description
                    //    };
                    //    await this.AppService.AddAttachmentAsync(newObjAttachment);
                    //}

                    var result = await this.AppService.GetTicketDetailsDapperAsync(dbObj.ID);
                    // HACK: To Add location
                    if (result.Data.ContainsKey("Location"))
                    {
                        result.Location = result.Data["Location"];
                    }

                    await this.AppService.SaveChangesAsync();

                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, "Error updating ticket");
                }
            }

            return StatusCode((int)HttpStatusCode.BadRequest, "Error updating ticket");
            #endregion MG-15

            #region MG-15 new
            //if (!this.ModelState.IsValid || vm == null)
            //{
            //    return this.BadRequest(this.ModelState);
            //}

            //try
            //{
            //    // Recover object from DB
            //    Ticket obj = await this._ticketAppService.SingleOrDefaultAsync(vm.ID);

            //    // Verify changes
            //    bool hasChanges = false;
            //    IEnumerable<TicketChangeSummaryEntry> changesMade;
            //    if (vm.AssignedEmployeeId != obj.AssignedEmployeeId)
            //    {
            //        hasChanges = true;

            //    }
            //    if (vm.BuildingId != obj.AssignedEmployeeId) hasChanges = true;
            //    if (vm.Description != obj.Description) hasChanges = true;
            //    if (vm.DestinationEntityId != obj.DestinationEntityId) hasChanges = true;
            //    if (vm.DestinationType != obj.DestinationType) hasChanges = true;
            //    if (vm.EpochCreatedDate != obj.EpochCreatedDate) hasChanges = true;

            //    // Validate atachments changes

            //}
            //catch(Exception ex)
            //{

            //}
            #endregion MG-15 new
        }


        /// <summary>
        ///     Adds EntityId and EntityType to Ticket
        ///     Doesn't validate if the EntityId of EntityType exists
        ///     yet
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPut]
        [PermissionsFilter("ConvertTickets")]
        public async Task<ActionResult<TicketDetailsViewModel>> Convert([FromBody] TicketConvertViewModel vm)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (vm == null)
            {
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    "Empty payload."
                );
            }

            var obj = await this.AppService.SingleOrDefaultAsync(vm.TicketId);
            if (obj == null)
            {
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    "Error finding ticket"
                );
            }

            try
            {
                obj.DestinationType = vm.DestinationType;
                obj.DestinationEntityId = vm.DestinationEntityId;
                // obj.Status = TicketStatus.Converted;

                obj.PendingReview = false;
                obj.NewRequesterResponse = false;

                obj = await this.AppService.UpdateEntityReferenceAsync(obj);
                await this.AppService.SaveChangesAsync();

                var result = await this.AppService.GetTicketDetailsDapperAsync(obj.ID);
                return new JsonResult(result);
            }
            catch (NotSupportedException ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.PreconditionFailed, ex.Message);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode(
                    (int)HttpStatusCode.PreconditionFailed,
                    "Error updating ticket");
            }
        }


        [HttpPut]
        [PermissionsFilter("UpdateTickets")]
        public async Task<ActionResult<TicketUpdateViewModel>> UpdateStatus([FromBody] TicketUpdateStatusViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                if (vm == null)
                {
                    return StatusCode(
                        (int)HttpStatusCode.BadRequest,
                        "Empty payload."
                    );
                }
                var objs = await AppService.ReadAllAsync(ent => vm.Id.Contains(ent.ID));
                if (objs == null)
                {
                    return StatusCode(
                        (int)HttpStatusCode.BadRequest,
                        "Error finding tickets"
                    );
                }

                try
                {
                    foreach (var obj in objs)
                    {
                        obj.Status = vm.Status;

                        if (obj.Status == TicketStatus.Resolved)
                        {
                            obj.PendingReview = false;
                            obj.NewRequesterResponse = false;
                        }

                        if (obj.FreshdeskTicketId.HasValue)
                        {
                            await this._freshdeskApplicationService.UpdateTicketStatus(obj.FreshdeskTicketId.Value, obj.Status);
                        }
                    }

                    await this.AppService.UpdateRangeAsync(objs);

                    await this.AppService.SaveChangesAsync();

                    // Returning same input
                    // TODO: Think about a proper response
                    return new JsonResult(vm);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                    return StatusCode(
                        (int)HttpStatusCode.PreconditionFailed,
                        "Error updating tickets");
                }
            }
            return StatusCode(
                        (int)HttpStatusCode.BadRequest,
                        "Error updating tickets");
        }


        [HttpPut]
        [PermissionsFilter("UpdateTickets")]
        public async Task<IActionResult> UpdateSingleStatus([FromBody] TicketUpdateSingleStatusViewModel vm)
        {
            if (!this.ModelState.IsValid)
            {
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    "Error updating ticket");
            }

            if (vm == null)
            {
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    "Empty payload."
                );
            }

            // Not using 'async' version to avoid LINQ includes
            var obj = this.AppService.SingleOrDefault(ent => ent.ID == vm.ID);
            if (obj == null)
            {
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    "Error finding ticket"
                );
            }

            try
            {
                obj.Status = vm.Status;

                if (obj.Status == TicketStatus.Resolved)
                {
                    obj.PendingReview = false;
                    obj.NewRequesterResponse = false;

                    await this.AppService.CloseReferencedInspectionItem(obj.ID);
                }

                await this.AppService.UpdateAsync(obj);
                await this.AppService.SaveChangesAsync();

                if (obj.FreshdeskTicketId.HasValue)
                {
                    await this._freshdeskApplicationService.UpdateTicketStatus(obj.FreshdeskTicketId.Value, obj.Status);
                }

                var result = await this.AppService.GetTicketDetailsDapperAsync(obj.ID);
                return new JsonResult(result);

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode(
                    (int)HttpStatusCode.PreconditionFailed,
                    "Error updating ticket");
            }
        }

        [HttpDelete]
        [PermissionsFilter("DeleteTickets")]
        public async Task<IActionResult> Delete(int id)
        {
            var porObj = await AppService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (porObj == null)
            {
                return StatusCode(
                        (int)HttpStatusCode.NoContent,
                        $"Error finding Ticket"
                        );
            }
            try
            {
                AppService.Remove(porObj.ID);
                await this.AppService.CloseReferencedInspectionItem(porObj.ID);
                await this.AppService.SaveChangesAsync();
            }
            catch (Exception)
            {
                // TODO: Catch proper error with foreign key exceptions
                return StatusCode(
                        (int)HttpStatusCode.PreconditionFailed,
                        $"This Ticket is referenced by one or many entities"
                        );
            }
            return Ok();
        }

        [HttpDelete]
        [PermissionsFilter("DeleteTickets")]
        public async Task<IActionResult> DeleteRange([FromBody] EntityIdCollectionViewModel vm)
        {
            if (vm == null || !vm.Id.Any())
            {
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    $"Empty payload"
                );
            }
            var objs = await AppService.ReadAllAsync(ent => vm.Id.Contains(ent.ID));
            if (objs == null || !objs.Any())
            {
                return StatusCode(
                        (int)HttpStatusCode.NoContent,
                        $"Error finding Tickets"
                        );
            }
            try
            {
                foreach (var ticket in objs)
                {
                    if (ticket.FreshdeskTicketId.HasValue)
                    {
                        await this._freshdeskApplicationService.DeleteTicket(ticket.FreshdeskTicketId.Value.ToString());
                    }
                }

                var idsDeleted = objs.Select(el => el.ID).ToList();
                AppService.RemoveRange(objs);
                await this.AppService.SaveChangesAsync();

                // Building respnse
                return new JsonResult(new { Count = idsDeleted.Count, Id = idsDeleted });
            }
            catch (Exception)
            {
                // TODO: Catch proper error with foreign key exceptions
                return StatusCode(
                        (int)HttpStatusCode.PreconditionFailed,
                        $"Some tickets are being referenced by one or many entities"
                        );
            }
        }

        //GET: api/tickets/get/<id>
        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadTickets", "ReadTicketFromCalendar")]
        public async Task<ActionResult<TicketDetailsViewModel>> Get(int id)
        {
            var objVM = await this.AppService.GetTicketDetailsDapperAsync(id);

            if (objVM == null)
            {
                return StatusCode(
                    (int)HttpStatusCode.NoContent,
                    $"Error finding Ticket"
                );
            }

            // HACK: To Add location
            if (objVM.Data.ContainsKey("Location"))
            {
                objVM.Location = objVM.Data["Location"];
            }

            return new JsonResult(objVM);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TicketDetailsViewModel>> AddExternal([FromBody] TicketCreateViewModel vm)
        {
            var receiver = "axzesllc@gmail.com";
            try
            {

                var strPayload = JsonConvert.SerializeObject(vm);
                this._emailSender.SendEmailAsync(receiver, "MGCapital Create Ticket Attempt", strPayload);
                if (!this.ModelState.IsValid || vm == null)
                {
                    var strModelState = JsonConvert.SerializeObject(this.ModelState);
                    this._emailSender.SendEmailAsync(receiver, "MGCapital Ticket Bad Request", strModelState);
                    return this.BadRequest(this.ModelState);
                }

                var obj = this.Mapper.Map<TicketCreateViewModel, Ticket>(vm);

                // HACK: In case location comes as a plain field
                if (!string.IsNullOrEmpty(vm.Location))
                {
                    obj.Data["Location"] = vm.Location;
                }

                await this.AppService.AddAsyncExternal(obj);
                //await this.AppService.SaveChangesAsyncExternal();

                var result = await this.AppService.GetTicketDetailsDapperAsync(obj.ID);

                var strResult = JsonConvert.SerializeObject(result);
                this._emailSender.SendEmailAsync(receiver, "MGCapital Ticket Success", strResult);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                this._emailSender.SendEmailAsync(receiver, "MGCapital Exception With Ticket External API", plainTextMessage: $"EXCEPTION: {ex.Message}", htmlMessage: $"EXCEPTION: <br> {ex.Message}");
                throw ex;
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> ReadAllCbo(DataSourceRequest request, int? id)
        //{
        //    var porVM = await _ticketAppService.ReadAllCboDapperAsync(request, id);
        //    return new JsonResult(porVM);
        //}

        /// <summary>
        /// Gets the amount of "not deleted" tickets that are pending to review
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Pending()
        {
            int pendingTicketsCount = await this.AppService.GetPendingTicketsCountDapperAsync();
            return pendingTicketsCount;
        }

        [HttpPost]
        public async Task<IActionResult> ForwardTicket([FromForm] TicketForwardViewModel vm)
        {
            try
            {
                await this.AppService.ForwardTicket(vm);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTicketAttachment([FromForm] TicketAttachmentCreateViewModel vm)
        {
            try
            {
                var result = await this.AppService.AddTicketAttachmentAsync(vm);
                await this.AppService.SaveChangesAsync();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region PreCalendar
        [HttpPost]
        [PermissionsFilter("AddTicketFromPreCalendar")]
        public async Task<ActionResult> AddTicketFromPreCalendar([FromBody] TicketFromPreCalendarCreateViewModel ticketVm)
        {
            if (!this.ModelState.IsValid || ticketVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            TicketCreateViewModel ticketCreate = new TicketCreateViewModel()
            {
                Source = ticketVm.Source,
                Status = ticketVm.Status,
                DestinationType = ticketVm.DestinationType,
                DestinationEntityId = ticketVm.DestinationEntityId,
                Description = ticketVm.Description,
                FullAddress = ticketVm.FullAddress,
                BuildingId = ticketVm.BuildingId,
                UserId = ticketVm.UserId,
                UserType = ticketVm.UserType,
                RequesterFullName = ticketVm.RequesterFullName,
                RequesterEmail = ticketVm.RequesterEmail,
                RequesterPhone = ticketVm.RequesterPhone,
                SnoozeDate = ticketVm.SnoozeDate,
                Attachments = ticketVm.Attachments,
                Data = ticketVm.Data,
            };



            for (int i = 0; i < ticketVm.Quantity; i++)
            {
                var ticket = this.Mapper.Map<TicketCreateViewModel, Ticket>(ticketCreate);

                if (!string.IsNullOrEmpty(ticketCreate.Location))
                {
                    ticket.Data["Location"] = ticketCreate.Location;
                }
                var ticketObject = await this.AppService.AddAsync(ticket, false);

                await this.AppService.SaveChangesAsync();

                // ++Date

                DateTime fecha = ticketCreate.SnoozeDate.Value;

                string option = ticketVm.Periodicity.ToString();

                switch (option)
                {
                    case "Monthly":
                        fecha = fecha.AddMonths(1);
                        ticketCreate.SnoozeDate = fecha;
                        break;
                    case "Weekly":
                        fecha = fecha.AddDays(7);
                        ticketCreate.SnoozeDate = fecha;
                        break;
                    case "BiWeekly":
                        fecha = fecha.AddDays(14);
                        ticketCreate.SnoozeDate = fecha;
                        break;
                    default:
                        break;
                }
            }
            return Ok();
        }

        #endregion

        #region Freshdesk
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TicketDetailsViewModel>> GetTicketDetail(int id)
        {
            try
            {
                bool updated = await this.AppService.AssignCurrentEmployeeToTicket(id);
                if (updated)
                {
                    await this.AppService.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Nothing
            }

            try
            {
                var objVM = await this.AppService.GetTicketDetailsDapperAsync(id);

                if (objVM == null)
                {
                    return StatusCode(
                        (int)HttpStatusCode.NoContent,
                        $"Error finding Ticket"
                    );
                }

                // Adds Employee Signature
                var signature = await this.EmployeeApplicationService.GetEmailSignatureAsync();
                if (string.IsNullOrEmpty(signature))
                {
                    // Adds Company Default Signature
                    signature = await this.EmployeeApplicationService.GetCompanyEmailSignatureAsync();
                }
                objVM.EmailSignature = signature;

                // HACK: To Add location
                if (objVM.Data.ContainsKey("Location"))
                {
                    objVM.Location = objVM.Data["Location"];
                }

                if (objVM.FreshdeskTicketId.HasValue)
                {
                    if (objVM.FreshdeskTicketId > 0)
                    {
                        try
                        {
                            var fdTicket = await this._freshdeskApplicationService.GetTicketDetail(objVM.FreshdeskTicketId.Value);
                            objVM.TicketFreshdesk = fdTicket;
                        }
                        catch (Exception ex)
                        {
                            // TODO
                        }
                    }
                }

                if (objVM.TicketFreshdesk == null)
                {
                    objVM.TicketFreshdesk = new TicketFreshdeskSummaryViewModel()
                    {
                        Ticket = null,
                        Conversations = new HashSet<ConversationBaseViewModel>()
                    };
                }

                #region GMail Ticket
                // Repeat process, but with "GMail Ticket"
                if (!string.IsNullOrEmpty(objVM.MessageId))
                {
                    var gmailTicket = this._gmailApiService.GetMessageContent(objVM.MessageId);
                    if (gmailTicket != null)
                    {
                        objVM.GMailTicket = await this._gmailApiService.GetEmailData(gmailTicket, id);
                        DateTime date = DateTime.UtcNow;
                        if (gmailTicket.InternalDate != null)
                        {
                            int milliseconds = (int)(gmailTicket.InternalDate.Value / 1000);
                            date = DateTimeExtensions.FromEpoch(milliseconds);
                        }
                        else
                        {
                            if (!DateTime.TryParse(objVM.GMailTicket.Date, out date))
                            {
                                date = DateTime.UtcNow;
                            }
                        }

                        objVM.TicketFreshdesk = new TicketFreshdeskSummaryViewModel()
                        {
                            Ticket = new FreshdeskTicketBaseViewModel()
                            {
                                CreatedAt = date,
                                Email = objVM.GMailTicket.From,
                                Subject = objVM.GMailTicket.Subject,
                                Description = objVM.GMailTicket.Body,
                                DescriptionText = objVM.GMailTicket.BodyText,
                                Attachments = objVM.GMailTicket.Attachments
                            },
                            Conversations = await this._gmailApiService.GetConversations(objVM.MessageId, id),
                        };
                        objVM.TicketFreshdesk.Ticket.Description = objVM.GMailTicket.Body;
                        objVM.TicketFreshdesk.Ticket.DescriptionText = objVM.GMailTicket.BodyText;
                    }

                }
                #endregion GMail Ticket

                var mergeLog = await this.AppService.ReadAllTicketAcitivityLogAsync(id, new List<int> {
                    (int)TicketActivityType.TicketMerged,
                    (int)TicketActivityType.Forwarded,
                    (int)TicketActivityType.TicketConverted,
                    (int)TicketActivityType.TicketConvertedWorkOrderSequence
                });
                if (mergeLog.Any())
                {
                    List<ConversationBaseViewModel> conversations = objVM.TicketFreshdesk.Conversations.ToList();
                    foreach (var log in mergeLog)
                    {
                        foreach (var summary in log.ChangeSummaryEntries)
                        {
                            conversations.Add(new ConversationBaseViewModel()
                            {
                                CreatedAt = log.EpochCreatedDate.FromEpoch(),
                                UpdatedAt = log.EpochUpdatedDate.FromEpoch(),
                                BodyText = summary.Summary,
                                Body = summary.Summary,
                                SupportEmail = log.EmployeeEmail,
                                Id = 0,
                                Incoming = true,
                                Source = FreshdeskConversationSource.Reply,
                                UserId = 0,
                                FromActivityLog = true,
                                ActivityType = log.ActivityType,
                                ActivityTypeName = log.ActivityTypeName,
                                AppCustomFields = new Dictionary<string, string>()
                                {
                                    {"EntityId",summary.EntityId.ToString()},
                                    {"EntityType",((int)summary.EntityType).ToString() }
                                }
                            });
                        }

                    }
                    objVM.TicketFreshdesk.Conversations = conversations;
                }

                if (objVM.TicketFreshdesk.Conversations != null)
                    objVM.TicketFreshdesk.Conversations = objVM.TicketFreshdesk.Conversations.OrderBy(c => c.CreatedAt).AsEnumerable();

                return new JsonResult(objVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetFreshDeskTicketConversations(int id)
        {
            try
            {
                var result = this._freshdeskApplicationService.ReadAllTicketConversations(id);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReplyTicket([FromForm] FreshdeskTicketReplyAttachedFilesViewModel vm)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return BadRequest();
                }

                // Get Message Base to do this, we need to recover Ticket.MessageID so, 1st we recover ticket
                var ticket = await this.AppService.GetTicketDetailsDapperAsync(vm.TicketId);
                var message = this._gmailApiService.GetMessageContent(ticket.MessageId);
                await this._gmailApiService.ReplyEmail(message, vm, ticket.ID);

                var obj = await this.AppService.SingleOrDefaultAsync(vm.TicketId);
                if (obj != null)
                {
                    obj.NewRequesterResponse = false;
                    obj.PendingReview = false;
                    await this.AppService.UpdateAsync(obj);
                    await this.AppService.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public FileContentResult DownloadFreshdeskImage(string url, string name, string fileType)
        {
            var result = this._freshdeskApplicationService.DownloadImageByte(url);

            return File(result, fileType, name);
        }

        [HttpPost]
        public async Task<IActionResult> CopyFreshdeskImageToTicket([FromBody] TicketCopyFreshdeskImageViewModel viewModel)
        {
            try
            {
                var result = await this.AppService.CopyFreshdeskAttachmentToTicket(viewModel);

                await this.AppService.SaveChangesAsync();

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion
    }
}
