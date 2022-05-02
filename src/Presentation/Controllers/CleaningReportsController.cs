using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.CleaningReportItem;
using MGCap.Presentation.Extensions;
using MGCap.Presentation.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class CleaningReportsController : BaseEntityController<CleaningReport, int>
    {
        protected readonly IAzureStorage _azureStorage;
        public CleaningReportsController(
            IEmployeesApplicationService employeeAppService,
            ICleaningReportsApplicationService appService,
            IAzureStorage azureStorage,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
            _azureStorage = azureStorage;
        }

        public new ICleaningReportsApplicationService AppService => base.AppService as ICleaningReportsApplicationService;

        #region CleaningReport
        [HttpGet]
        [PermissionsFilter("ReadCleaningReports")]
        public async Task<ActionResult<DataSourceCleaningReport>> ReadAll(DataSourceRequest request, int? statusId, int? employeeId, int? contactId, int? notesDirection, int? commentDirection)
        {
            var status = statusId == -1 ? null : statusId;
            var cleaningReportVM = await this.AppService.ReadAllDapperAsync(request, contactId, employeeId, status, commentDirection);
            return new JsonResult(cleaningReportVM);
        }

        [HttpGet]
        [PermissionsFilter("ReadCleaningReports")]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? id)
        {
            var cleaningReportVM = await this.AppService.ReadAllCboDapperAsync(request, id);
            return new JsonResult(cleaningReportVM);
        }

        [HttpPost]
        [PermissionsFilter("AddCleaningReports")]
        public async Task<IActionResult> Add([FromBody] CleaningReportCreateViewModel reportVM)
        {
            if (this.ModelState.IsValid)
            {
                if (reportVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                try
                {
                    var reportObj = this.Mapper.Map<CleaningReportCreateViewModel, CleaningReport>(reportVM);
                    await this.AppService.AddAsync(reportObj);

                    await this.AppService.SaveChangesAsync();
                    var result = this.Mapper.Map<CleaningReport, CleaningReportCreateViewModel>(reportObj);

                    return new JsonResult(result);
                }
                catch (Exception) // unexpected exception
                {
                    return this.BadRequest(this.ModelState);
                }
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet]
        [PermissionsFilter("ReadCleaningReports")]
        public async Task<JsonResult> GetCleaningReportDetails(int cleaningReportId)
        {
            var cleaningReportVM = await this.AppService.GetCleaningReportDetailsDapperAsync(cleaningReportId, null);
            return new JsonResult(cleaningReportVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> PublicGetCleaningReport(Guid guid)
        {
            var cleaningReportVM = await this.AppService.GetCleaningReportDetailsDapperAsync(guid: guid);
            return new JsonResult(cleaningReportVM);
        }

        [HttpPut]
        [PermissionsFilter("UpdateCleaningReports")]
        public async Task<IActionResult> Update([FromBody]  CleaningReportUpdateViewModel reportVM)
        {
            if (this.ModelState.IsValid)
            {
                if (reportVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var reportObj = await this.AppService.SingleOrDefaultAsync(ent => ent.ID == reportVM.ID);
                if (reportObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(reportVM, reportObj);
                await this.AppService.UpdateAsync(reportObj);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            return this.BadRequest(this.ModelState);
        }

        [HttpGet]
        [PermissionsFilter("ReadCleaningReports")]
        public async Task<IActionResult> Update(int id)
        {
            var reportObj = await this.AppService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (reportObj == null)
            {
                return this.NotFound();
            }

            var reportVM = this.Mapper.Map<CleaningReport, CleaningReportUpdateViewModel>(reportObj);
            return new JsonResult(reportVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteCleaningReports")]
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

        [HttpPost]
        [PermissionsFilter("SendCleaningReportsEmails")]
        public async Task<IActionResult> SendCleaningReport([FromBody] CleaningReportSendEmailViewModel vm)
        {
            try
            {
                var reportVM = await this.AppService.GetCleaningReportDetailsDapperAsync(vm.ID, null);
                if (reportVM == null)
                {
                    return BadRequest("Error finding report");
                }

                var additionalRecipents = vm.AdditionalRecipients;
                await this.AppService.SendCleaningReport(reportVM, additionalRecipents);
                await this.AppService.SaveChangesAsync();
                return new JsonResult(reportVM);
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
        [PermissionsFilter("ReadCleaningReports")]
        public async Task<IActionResult> GetPDFReportBase64(int id)
        {
            var result = await this.AppService.GetReportPDFBase64(id);
            return new JsonResult(result);
        }
        #endregion

        #region CleaningReportItem

        [HttpPost]
        [PermissionsFilter("AddCleaningReportItems")]
        public async Task<IActionResult> AddCleaningReportItem([FromBody] CleaningReportItemCreateViewModel itemVM)
        {
            if (this.ModelState.IsValid)
            {
                if (itemVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                try
                {
                    var itemObj = this.Mapper.Map<CleaningReportItemCreateViewModel, CleaningReportItem>(itemVM);
                    await this.AppService.AddCleaningReportItemAsync(itemObj);

                    await this.AppService.SaveChangesAsync();
                    var result = this.Mapper.Map<CleaningReportItem, CleaningReportItemCreateViewModel>(itemObj);

                    return new JsonResult(result);
                }
                catch (Exception) // unexpected exception
                {
                    return this.BadRequest(this.ModelState);
                }
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet]
        [PermissionsFilter("ReadCleaningReportItems")]
        public async Task<JsonResult> GetCleaningReportItemsDetails(int cleaningReportId, int? type = null)
        {
            var itemsVM = await AppService.GetCleaningReportItemsDapper(cleaningReportId, type);
            return new JsonResult(itemsVM);
        }

        [HttpGet("{cleaningReportItemId:int}")]
        [PermissionsFilter("ReadCleaningReportItems")]
        public async Task<IActionResult> UpdateCleaningReportItem(int cleaningReportItemId)
        {
            var itemVM = await this.AppService.GetCleaningReportItemAsync(cleaningReportItemId);
            if (itemVM == null)
            {
                return this.NotFound();
            }
            return new JsonResult(itemVM);
        }

        [HttpPut]
        [PermissionsFilter("UpdateCleaningReportItems")]
        public async Task<IActionResult> UpdateCleaningReportItem([FromBody] CleaningReportItemUpdateViewModel itemVM)
        {
            if (this.ModelState.IsValid)
            {
                if (itemVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var itemObj = await this.AppService.SingleOrDefaultItemAsync(ent => ent.ID == itemVM.ID);
                if (itemObj == null)
                {
                    return NotFound();
                }

                //Delete from storage attachments in DB but not in vm
                IEnumerable<int> atts = itemVM.Attachments.Select(v => v.ID);
                List<int> toDelete = itemObj.CleaningReportItemAttachments.Select(a => a.ID).Except(atts).ToList();
                while (toDelete.Count > 0)
                {
                    await DeleteAttachment(toDelete.ElementAt(0));
                    toDelete.RemoveAt(0);
                }

                this.Mapper.Map(itemVM, itemObj);

                itemObj.CleaningReportItemAttachments.Clear();
                await this.AppService.UpdateItemAsync(itemObj);
                await this.AppService.SaveChangesAsync();

                foreach (var objAttachment in itemVM.Attachments)
                {
                    var newObjAttachment = new CleaningReportItemAttachment
                    {
                        BlobName = objAttachment.BlobName,
                        FullUrl = objAttachment.FullUrl,
                        ImageTakenDate = objAttachment.ImageTakenDate,
                        Title = objAttachment.Title,
                        CleaningReportItemId = itemVM.ID
                    };
                    await this.AppService.AddAttachmentAsync(newObjAttachment);
                }

                await this.AppService.SaveChangesAsync();

                return Ok();

            }
            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{itemId:int}")]
        [PermissionsFilter("DeleteCleaningReportItems")]
        public async Task<IActionResult> DeleteCleaningReportItem(int itemId)
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

        [HttpDelete]
        [PermissionsFilter("DeleteCleaningReportAttachment")]
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

        #endregion

        #region Cleaning Report Notes

        [HttpPost]
        [PermissionsFilter("AddCleaningReportNotes")]
        public async Task<IActionResult> AddCleaningReportNote([FromBody] CleaningReportNoteCreateViewModel noteVM)
        {
            if (this.ModelState.IsValid)
            {
                if (noteVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                try
                {
                    var objNote = Mapper.Map<CleaningReportNoteCreateViewModel, CleaningReportNote>(noteVM);

                    objNote = await AppService.AddCleaningReportNoteAsync(objNote);

                    var reportVM = await this.AppService.GetCleaningReportDetailsDapperAsync(noteVM.CleaningReportId, null);

                    await this.AppService.SendCleaningReport(reportVM, null, true);

                    await AppService.SaveChangesAsync();

                    return new JsonResult(objNote);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                    this.LogError(ex, noteVM);

                    return NoContent();
                }
            }
            else
            {
                return BadRequest(this.ModelState);
            }
        }

        [HttpPost("{reportGuid:Guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> AddCleaningReportPublicNote([FromBody] CleaningReportNoteCreateViewModel noteVM)
        {
            if (this.ModelState.IsValid)
            {
                if (noteVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                try
                {
                    var objNote = Mapper.Map<CleaningReportNoteCreateViewModel, CleaningReportNote>(noteVM);

                    objNote = await AppService.AddCleaningReportNoteAsync(objNote);

                    await AppService.SaveChangesAsync();

                    return new JsonResult(objNote);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                    return NoContent();
                }
            }
            else
            {
                return BadRequest(this.ModelState);
            }
        }

        #endregion

        #region Cleaning Report Log
        [HttpGet("{cleaningReportId:int}")]
        [PermissionsFilter("ReadCleaningReports")]
        public async Task<JsonResult> GetAllActivityLog(DataSourceRequest request, int cleaningReportId)
        {
            var result = await this.AppService.GetAllActivityLogDapperAsync(request, cleaningReportId);
            return new JsonResult(result);
        }
        #endregion
    }
}
