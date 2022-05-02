using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Expense;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class ExpensesController : BaseEntityController<Expense, int>
    {
        private new IExpensesApplicationService AppService => base.AppService as IExpensesApplicationService;

        public ExpensesController(
            IEmployeesApplicationService employeeAppService,
            IExpensesApplicationService appService,
            IMapper mapper
            ) : base(employeeAppService, appService, mapper)
        {
        }

        [HttpGet]
        [PermissionsFilter("ReadExpenses")]
        public async Task<ActionResult<DataSource<ExpenseGridViewModel>>> ReadAll(DataSourceRequest request, int? buildingId = null, int? customerId = null, int? contractId = null)
        {
            var result = await this.AppService.ReadAllDapperAsync(request, buildingId, customerId, contractId);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult<DataSource<ExpenseGridViewModel>>> ReadAllByBudgetId(DataSourceRequest request, int budgetId, int month, int year)
        {
            var result = await this.AppService.ReadAllByBudgetIdDapperAsync(request, budgetId, month, year);
            return new JsonResult(result);
        }

        [HttpPost]
        [PermissionsFilter("AddExpenses")]
        public async Task<ActionResult> Add([FromBody] ExpenseCreateViewModel expenseVm)
        {
            if (!this.ModelState.IsValid || expenseVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            string result = string.Empty;
            //if (expenseVm.BuildingId.HasValue)
            //{
            var expense = this.Mapper.Map<ExpenseCreateViewModel, Expense>(expenseVm);
            var expenseObject = await this.AppService.AddAsync(expense);
            result = "Expense Added";
            //}
            //else
            //{
            //    var expenses = await this.AppService.AddIndirectExpenseAsync(expenseVm);
            //    if (expenses == null)
            //        result = "Not Available Contracts";
            //    else
            //        result = "Indirect Expenses Added";
            //}

            await this.AppService.SaveChangesAsync();

            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadExpenses")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.GetExpenseDetailAsync(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteExpenses")]
        public async new Task<IActionResult> Delete(int id)
        {
            var expense = await this.AppService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (expense == null)
            {
                return BadRequest(this.ModelState);
            }
            try
            {
                this.AppService.Remove(expense);
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
        [PermissionsFilter("UpdateExpenses")]
        public async Task<IActionResult> Update([FromBody] ExpenseUpdateViewModel expenseVm)
        {
            if (!this.ModelState.IsValid || expenseVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var expenseObject = await this.AppService.SingleOrDefaultAsync(p => p.ID == expenseVm.ID);
            if (expenseObject == null)
            {
                return this.NoContent();
            }

            this.Mapper.Map(expenseVm, expenseObject);
            await this.AppService.UpdateAsync(expenseObject);
            await this.AppService.SaveChangesAsync();
            var result = await this.GetExpenseDetailAsync(expenseObject.ID);
            return new JsonResult(result);
        }

        private async Task<ExpenseBaseViewModel> GetExpenseDetailAsync(int id)
        {
            var expense = await this.AppService.SingleOrDefaultAsync(p => p.ID == id);
            if (expense == null)
            {
                return null;
            }

            var expenseDetail = this.Mapper.Map<Expense, ExpenseBaseViewModel>(expense);
            return expenseDetail;
        }

        [HttpPost]
        public async Task<IActionResult> AddCSV([FromBody] ExpenseCreateViewModel expenseVm)
        {
            /*
             * 0 = save
             * 1 = repeated budget
             * 2 = repeated expense
             * 3 = error
             */

            try
            {
                var expenses = await this.AppService.AddCSVExpense(expenseVm);
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
