using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PreCalendar;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class PreCalendarController : BaseEntityController<PreCalendar, int>
    {
        public new IPreCalendarApplicationService AppService => base.AppService as IPreCalendarApplicationService;

        public PreCalendarController
        (
            IEmployeesApplicationService employeeAppService,
            IPreCalendarApplicationService appService,
            IMapper mapper
        ) : base(employeeAppService, appService, mapper)
        {

        }

        [HttpGet]
        [PermissionsFilter("ReadPreCalendar")]
        public async Task<ActionResult<DataSource<PreCalendarGridViewModel>>> ReadAll(DataSourceRequest request, int? buildingId = null)
        {
            var preCalendarVm = await AppService.ReadAllDapperAsync(request, buildingId);
            return preCalendarVm;
        }

        [HttpPost]
        [PermissionsFilter("AddPreCalendar")]
        public async Task<ActionResult> Add([FromBody] PreCalendarCreateViewModel preCalendarVm)
        {
            if (!this.ModelState.IsValid || preCalendarVm == null)
            {
                return this.BadRequest(this.ModelState);
            }
            try
            {
                var preCaledar = this.Mapper.Map<PreCalendarCreateViewModel, PreCalendar>(preCalendarVm);
                var preCalendarObject = await this.AppService.AddAsync(preCaledar);
                await this.AppService.SaveChangesAsync();
                return new JsonResult("Ok");
            }
            catch (Exception)
            {
                return this.BadRequest("Error");
            }
           // var result = this.Mapper.Map<PreCalendar, PreCalendarCreateViewModel>(preCalendarObject);
        }

        [HttpDelete]
        [PermissionsFilter("DeletePreCalendar")]
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

        [HttpPut]
        [PermissionsFilter("UpdatePreCalendar")]
        public async Task<IActionResult> Update([FromBody] PreCalendarUpdateViewModel preCalendarVm)
        {
            if (!this.ModelState.IsValid || preCalendarVm == null)
            {
                return this.BadRequest(this.ModelState);
            }
            //Delete task
            var itemObj = await this.AppService.SingleOrDefaultDapperAsync(preCalendarVm.ID);
            if (itemObj == null)
            {
                return NotFound();
            }
            foreach (var att in itemObj.Tasks)
            {
                await this.AppService.RemoveTasksAsync(att.ID);
            }


            var preCalendarObject = await this.AppService.SingleOrDefaultAsync(p => p.ID == preCalendarVm.ID);
            this.Mapper.Map(preCalendarVm, preCalendarObject);

            await this.AppService.UpdateAsync(preCalendarObject);
            await this.AppService.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadPreCalendar")]
        public async Task<PreCalendarDetailViewModel> GetPreCalendarItemDetailAsync(int id) 
        {
            // TODO: Generate functions to get the inspection details
            var obj = await this.AppService.SingleOrDefaultDapperAsync(id);
            return obj;
        }

        #region Tasks
        //[HttpGet]
        //public async Task<JsonResult> ReadAllTasks(DataSourceRequest request, int pcId)
        //{
        //    var objsVM = await this.AppService.ReadAllTasksDapperAsync(request, pcId);
        //    return new JsonResult(objsVM);
        //}
        #endregion
    }
}
