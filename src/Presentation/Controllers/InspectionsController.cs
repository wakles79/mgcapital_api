using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.InspectionItem;
using MGCap.Domain.ViewModels.InspectionItemTicket;
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
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class InspectionsController : BaseEntityController<Inspection, int>
    {

        public new IInspectionsApplicationService AppService => base.AppService as IInspectionsApplicationService;

        protected readonly IAzureStorage _azureStorage;


        public InspectionsController
         (
             IEmployeesApplicationService employeeAppService,
             IInspectionsApplicationService appService,
             IMapper mapper,
            IAzureStorage azureStorage
         ) : base(employeeAppService, appService, mapper)
        {
            _azureStorage = azureStorage;
        }

        #region Inspection
        [HttpGet]
        [PermissionsFilter("ReadInspections")]
        public async Task<ActionResult<DataSource<InspectionGridViewModel>>> ReadAll(DataSourceRequestInspection request, int? status = -1, int? buildingId = null, int? employeeId = null)
        {
            var InspectioVm = await AppService.ReadAllDapperAsync(request, status, buildingId, employeeId);
            return InspectioVm;
        }

        [HttpPost]
        [PermissionsFilter("AddInspections", "AddInspectionFromCalendar")]
        public async Task<ActionResult> Add([FromBody] InspectionCreateViewModel inspectionVm)
        {
            if (!this.ModelState.IsValid || inspectionVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var inspection = this.Mapper.Map<InspectionCreateViewModel, Inspection>(inspectionVm);
            var inspectionObject = await this.AppService.AddAsync(inspection);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Inspection, InspectionCreateViewModel>(inspectionObject);
            return new JsonResult(result);
        }

        [HttpPost]
        [PermissionsFilter("AddInspectionFromPreCalendar")]
        public async Task<ActionResult> AddInspectionFromPreCalendar([FromBody] InspectionFromPreCalendarCreateViewModel inspectionVm)
        {
            if (!this.ModelState.IsValid || inspectionVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            InspectionCreateViewModel inspectionCreate = new InspectionCreateViewModel()
            {
                BuildingId = inspectionVm.BuildingId,
                EmployeeId = inspectionVm.EmployeeId,
                SnoozeDate = inspectionVm.SnoozeDate,
                BeginNotes = inspectionVm.BeginNotes
            };

            for (int i = 0; i < inspectionVm.Quantity; i++)
            {
                var inspection = this.Mapper.Map<InspectionCreateViewModel, Inspection>(inspectionCreate);
                var inspectionObject = await this.AppService.AddAsync(inspection);
                await this.AppService.AddAsync(inspectionObject);

                await this.AppService.SaveChangesAsync();

                // ++Date
                DateTime fecha = inspectionCreate.SnoozeDate.Value;
                // Convert.ToDateTime(inspectionCreate.SnoozeDate);
                string option = inspectionVm.Periodicity.ToString();

                switch (option)
                {
                    case "Monthly":
                        fecha = fecha.AddMonths(1);
                        inspectionCreate.SnoozeDate = fecha;
                        break;
                    case "Weekly":
                        fecha = fecha.AddDays(7);
                        inspectionCreate.SnoozeDate = fecha;
                        break;
                    case "BiWeekly":
                        fecha = fecha.AddDays(14);
                        inspectionCreate.SnoozeDate = fecha;
                        break;
                    default:
                        break;
                }
            }
            return Ok();
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadInspections", "ReadInspectionFromCalendar")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.GetInspectionDetailAsync(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateInspections", "UpdateInspectionFromCalendar")]
        public async Task<IActionResult> Update([FromBody] InspectionUpdateViewModel inspectionVm)
        {
            if (!this.ModelState.IsValid || inspectionVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var inspectionObject = await this.AppService.SingleOrDefaultAsync(p => p.ID == inspectionVm.ID);
            if (inspectionObject == null)
            {
                return this.NoContent();
            }

            inspectionVm.Number = inspectionObject.Number;

            this.Mapper.Map(inspectionVm, inspectionObject);
            await this.AppService.UpdateAsync(inspectionObject);
            await this.AppService.SaveChangesAsync();
            var result = await this.GetInspectionDetailAsync(inspectionObject.ID);
            return new JsonResult(result);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteInspections")]
        public async new Task<IActionResult> Delete(int id)
        {
            var reportObj = await this.AppService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (reportObj == null)
            {
                return BadRequest(this.ModelState);
            }
            try
            {
                this.AppService.Remove(reportObj);
                await this.AppService.SaveChangesAsync();
                return Ok();
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
        [PermissionsFilter("ReadInspections")]
        public async Task<IActionResult> GetInspectionDetails(int id)
        {
            // TODO: Generate functions to get the inspection details
            //var obj = await this.AppService.SingleOrDefaultAsync(b => b.ID == id);
            //var vm = this.Mapper.Map<Inspection, InspectionBaseViewModel>(obj);
            //return new JsonResult(vm);

            var InspectionDetails = await this.AppService.GetInspectionDetailsDapperAsync(id, null);
            return new JsonResult(InspectionDetails);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PublicGetInspectionDetails(Guid guid)
        {
            var InspectionDetails = await this.AppService.GetInspectionDetailsDapperAsync(null, guid);
            return new JsonResult(InspectionDetails);
        }

        [HttpGet]
        public async Task<IActionResult> GetInspectionPdfUrl(int id)
        {
            var result = await this.AppService.GetInspectionReportUrl(id, null);
            return new JsonResult(result);
        }

        [HttpPost]
        [PermissionsFilter("SendInspectionByEmail")]
        public async Task<IActionResult> SendInspectionByEmail([FromBody] InspectionReportSendEmailViewModel vm)
        {
            try
            {
                var inspectionDetail = await this.AppService.GetInspectionDetailsDapperAsync(vm.ID, null);
                if (inspectionDetail == null)
                {
                    return BadRequest("Error finding report");
                }

                if (string.IsNullOrEmpty(inspectionDetail.EmployeeEmail))
                {
                    return BadRequest("Employee doesn't have an email");
                }

                var additionalRecipents = vm.AdditionalRecipients;
                await this.AppService.SendInspectionReportByEmail(inspectionDetail, additionalRecipents);
                // to save the activity log
                await this.AppService.SaveChangesAsync();
                return new JsonResult(inspectionDetail);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return BadRequest("Error Sending Emails");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetActivityLog(DataSourceRequest request, int id)
        {
            var result = await this.AppService.GetAllActivityLogDapperAsync(request, id);
            return new JsonResult(result);
        }

        private async Task<InspectionDetailViewModel> GetInspectionDetailAsync(int id)
        {
            var inspection = await this.AppService.SingleOrDefaultAsync(p => p.ID == id);
            if (inspection == null)
            {
                return null;
            }

            var inspectionDetail = this.Mapper.Map<Inspection, InspectionDetailViewModel>(inspection);
            return inspectionDetail;
        }

        private async Task<IActionResult> GetInspectionReportDetails(int InspectionReportId)
        {
            var InspectionDetailstVM = await this.AppService.GetInspectionDetailsDapperAsync(InspectionReportId, null);
            if (InspectionDetailstVM == null)
            {
                return this.NoContent();
            }
            return new JsonResult(InspectionDetailstVM);
        }
        #endregion

        #region Inspection Items
        [HttpGet]
        public async Task<ActionResult<DataSource<InspectionItemGridViewModel>>> ReadAllInspectionItem(DataSourceRequest request, int inspectionItemId)
        {
            var inspectionServices = await this.AppService.ReadAllInspectionItemDapperAsync(request, inspectionItemId);
            return new JsonResult(inspectionServices);
        }

        [HttpPost]
        [PermissionsFilter("AddInspectionItem")]
        public async Task<ActionResult> AddInspectionItem([FromBody] InspectionItemCreateViewModel InspectionItemVM)
        {
            if (!this.ModelState.IsValid || InspectionItemVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<InspectionItemCreateViewModel, InspectionItem>(InspectionItemVM);
            var InspectionItemObject = await this.AppService.AddInspectionItemAsync(obj);

            await this.AppService.SaveChangesAsync();

            obj.Attachments.Clear();
            obj.Tasks.Clear();
            obj.Notes.Clear();
            var result = this.Mapper.Map<InspectionItem, InspectionItemCreateViewModel>(obj);

            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInspectionItem(int id)
        {
            var result = await this.GetInspectionItemDetailAsync(id);
            if (result == null)
            {
                return NoContent();
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetInspectionItemDetails(int id)
        {

            var result = await this.GetInspectionItemDetailAsync(id);
            if (result == null)
            {
                return NoContent();
            }
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAttachementItem(int id)
        {
            var result = await this.GetInspectionItemDetailAsync(id);
            if (result == null)
            {
                return NoContent();
            }
            return new JsonResult(result);
        }


        [HttpPut]
        [PermissionsFilter("UpdateInspectionItem")]
        public async Task<ActionResult> UpdateInspectionItem([FromBody] InspectionItemUpdateViewModel inspectionItemVm)
        {
            if (!this.ModelState.IsValid || inspectionItemVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var itemObj = await this.AppService.SingleOrDefaultItemAsync(ent => ent.ID == inspectionItemVm.ID);
            if (itemObj == null)
            {
                return NotFound();
            }

            //Delete from storage attachments in DB but not in vm
            IEnumerable<int> atts = inspectionItemVm.Attachments.Select(v => v.ID);
            List<int> toDelete = itemObj.Attachments.Select(a => a.ID).Except(atts).ToList();
            foreach (var attId in toDelete)
            {
                // Removes not inlcuded attachments from azure storage
                var att = itemObj.Attachments.FirstOrDefault(a => a.ID == attId);
                await _azureStorage.DeleteImageAsync(blobName: att.BlobName);
            }

            foreach (var att in itemObj.Tasks)
            {
                await this.AppService.RemoveTasksAsync(att.ID);
            }

            foreach (var att in itemObj.Attachments)
            {
                await this.AppService.RemoveAttachmentsAsync(att.ID);
            }

            foreach (var att in itemObj.Notes)
            {
                await this.AppService.RemoveItemNoteAsync(att.ID);
            }

            this.Mapper.Map(inspectionItemVm, itemObj);

            itemObj.Attachments.Clear();
            itemObj.Tasks.Clear();
            itemObj.Notes.Clear();

            await this.AppService.UpdateInspectionItemAsync(itemObj);
            await this.AppService.SaveChangesAsync();

            foreach (var objAttachment in inspectionItemVm.Attachments)
            {
                var newObjAttachment = new InspectionItemAttachment
                {
                    BlobName = objAttachment.BlobName,
                    FullUrl = objAttachment.FullUrl,
                    ImageTakenDate = objAttachment.ImageTakenDate,
                    Title = objAttachment.Title,
                    InspectionItemId = inspectionItemVm.ID,
                    Description = inspectionItemVm.Description
                };
                await this.AppService.AddAttachmentAsync(newObjAttachment);
            }

            foreach (var objAttachment in inspectionItemVm.Tasks)
            {
                var newObjTask = new InspectionItemTask
                {
                    IsComplete = objAttachment.IsComplete,
                    Description = objAttachment.Description,
                    InspectionItemId = inspectionItemVm.ID,
                };
                await this.AppService.AddTaskAsync(newObjTask);
            }

            foreach (var objAttachment in inspectionItemVm.Notes)
            {
                var newObjNote = new InspectionItemNote
                {
                    Note = objAttachment.Note,
                    EmployeeId = objAttachment.EmployeeId,
                    InspectionItemId = inspectionItemVm.ID,
                };
                await this.AppService.AddItemNoteAsync(newObjNote);
            }

            await this.AppService.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [PermissionsFilter("CloseInspectionItemDirectly")]
        public async Task<ActionResult> CloseInspectionItem(int id)
        {
            try
            {
                var result = await this.AppService.CloseInspectionItemAsync(id);

                await this.AppService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private async Task<InspectionItemUpdateViewModel> GetInspectionItemDetailAsync(int id)
        {
            var inspectionItem = await this.AppService.GetInspectionItemUpdateByIdAsync(id);

            if (inspectionItem == null)
            {
                return null;
            }

            return inspectionItem;
        }

        [HttpDelete("{itemId:int}")]
        [PermissionsFilter("DeleteInspectionItem")]
        public async Task<IActionResult> DeleteInspectionItem(int itemId)
        {
            var itemObj = await this.AppService.SingleOrDefaultItemAsync(ent => ent.ID == itemId);
            if (itemObj == null)
            {
                return BadRequest(this.ModelState);
            }
            await this.AppService.DeleteItemAsync(itemObj);
            await this.AppService.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [PermissionsFilter("AddTicketFromInspectionItem")]
        public async Task<IActionResult> AddTicketFromInspectionItem([FromBody] InspectionItemTicketCreateViewModel itemTicketVm)
        {

            if (!this.ModelState.IsValid || itemTicketVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<InspectionItemTicketCreateViewModel, InspectionItemTicket>(itemTicketVm);
            var inspectionItemTicketObject = await this.AppService.AddTicketFromInspectionItemAsync(obj);

            var inspectionItem = await this.AppService.GetInspectionItemByIdAsync(itemTicketVm.InspectionItemId);
            var inspection = await this.AppService.SingleOrDefaultAsync(i => i.ID == inspectionItem.InspectionId);
            if (inspection != null)
            {
                inspection.Status = Domain.Enums.InspectionStatus.Active;
                await this.AppService.UpdateAsync(inspection);
            }

            await this.AppService.SaveChangesAsync();
            var result = this.Mapper.Map<InspectionItemTicket, InspectionItemTicketCreateViewModel>(obj);

            return new JsonResult(result);

        }
        #endregion

        #region Attachments

        [HttpPost]
        [AllowAnonymous]
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
        [HttpGet("{id:int}")]
        [PermissionsFilter("DeleteInspectionItemAttachment")]
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
        [AllowAnonymous]
        [PermissionsFilter("DeleteInspectionItemAttachment")]
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

        #endregion

        #region Notes
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ReadAllNotes(DataSourceRequest request, int id)
        {
            var objsVM = await this.AppService.ReadAllNotesDapperAsync(request, id);
            return new JsonResult(objsVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteInspectionNote")]
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
        [PermissionsFilter("AddInspectionNote")]
        public async Task<IActionResult> AddNote([FromBody] InspectionNoteCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<InspectionNoteCreateViewModel, InspectionNote>(vm);
            await this.AppService.AddNoteAsync(obj);
            await this.AppService.SaveChangesAsync();
            return new JsonResult(obj);
        }

        [HttpPut]
        [PermissionsFilter("UpdateInspectionNote")]
        public async Task<IActionResult> UpdateNote([FromBody] InspectionNoteUpdateViewModel vm)
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
        [HttpPost]
        [PermissionsFilter("CloseInspectionItemTask")]
        public async Task<IActionResult> CloseInspectionItemTask(int id, bool isCompleted)
        {
            try
            {
                var result = await this.AppService.UpdateCompletedStatusTaskAsync(id, isCompleted);
                if (result == null)
                {
                    return NoContent();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
        #endregion
    }
}
