using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Revenue;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class RevenuesController : BaseEntityController<Revenue, int>
    {
        public new IRevenuesApplicationService AppService => base.AppService as IRevenuesApplicationService;
        public RevenuesController
        (
             IEmployeesApplicationService employeeAppService,
             IRevenuesApplicationService appService,
             IMapper mapper
        ) : base(employeeAppService, appService, mapper)
        {

        }

        #region revenue

        [HttpGet]
        [PermissionsFilter("ReadRevenues")]
        public async Task<ActionResult<DataSource<RevenueGridViewModel>>> ReadAll(DataSourceRequest request, int? status = -1, int? buildingId = null, int? customerId = null, int? contractId = null)
        {
            var revenueVm = await AppService.ReadAllDapperAsync(request, status, buildingId, customerId, contractId);
            return revenueVm;
        }

        [HttpGet]
        public async Task<ActionResult<DataSource<RevenueGridViewModel>>> ReadAllByBudgetId(DataSourceRequest request, int budgetId, int month, int year)
        {
            var result = await this.AppService.ReadAllByBudgetIdDapperAsync(request, budgetId, month, year);
            return new JsonResult(result);
        }

        [HttpPost]
        [PermissionsFilter("AddRevenues")]
        public async Task<ActionResult> Add([FromBody] RevenueCreateViewModel revenueVm)
        {
            if (!this.ModelState.IsValid || revenueVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var revenue = this.Mapper.Map<RevenueCreateViewModel, Revenue>(revenueVm);
            var revenueObject = await this.AppService.AddAsync(revenue);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Revenue, RevenueCreateViewModel>(revenueObject);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadRevenues")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.GetRevenueDetailAsync(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        private async Task<RevenueUpdateViewModel> GetRevenueDetailAsync(int id)
        {
            var revenue = await this.AppService.SingleOrDefaultAsync(p => p.ID == id);
            if (revenue == null)
            {
                return null;
            }

            var proposalDetail = this.Mapper.Map<Revenue, RevenueUpdateViewModel>(revenue);
            return proposalDetail;
        }

        [HttpDelete]
        [PermissionsFilter("DeleteRevenues")]
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
        [PermissionsFilter("UpdateRevenues")]
        public async Task<IActionResult> Update([FromBody] RevenueUpdateViewModel revenueVm)
        {
            if (!this.ModelState.IsValid || revenueVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var revenueObject = await this.AppService.SingleOrDefaultAsync(p => p.ID == revenueVm.ID);
            this.Mapper.Map(revenueVm, revenueObject);

            await this.AppService.UpdateAsync(revenueObject);
            await this.AppService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult> AddCSV([FromBody] RevenueCreateViewModel revenueVm)
        {
            /*
            * 0 = save
            * 1 = repeated budget
            * 2 = repeated expense
            * 3 = error
            */

            try
            {
                var expenses = await this.AppService.AddCSVExpense(revenueVm);
                await this.AppService.SaveChangesAsync();
                return new JsonResult(expenses);
            }
            catch (Exception)
            {
                return new JsonResult("3");
            }
        }
    }
}
