using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ExpenseType;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.ExpenseSubcategory;
using MGCap.Presentation.Filters;

namespace MGCap.Presentation.Controllers
{
    public class ExpenseTypesController : BaseController
    {
        private readonly IExpenseTypesApplicationService _AppService;

        public ExpenseTypesController(
            IEmployeesApplicationService employeeAppService,
            IExpenseTypesApplicationService appService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            this._AppService = appService;
        }

        #region Expense Types

        [HttpGet]
        [PermissionsFilter("ReadExpensesTypesCatalog")]
        public async Task<JsonResult> ReadAll(DataSourceRequest request, int? isActive = -1)
        {
            var result = await this._AppService.ReadAllDapperAsync(request, isActive);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? isActive)
        {
            var result = await this._AppService.ReadAllCboDapperAsync(request);
            return new JsonResult(result);
        }

        [HttpPost]
        [PermissionsFilter("AddExpensesTypesCatalog")]
        public async Task<ActionResult> Add([FromBody] ExpenseTypeCreateViewModel expenseTypeVM)
        {
            if (!this.ModelState.IsValid || expenseTypeVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<ExpenseTypeCreateViewModel, ExpenseType>(expenseTypeVM);
            var expenseTypeObject = await this._AppService.AddAsync(obj);
            await this._AppService.SaveChangesAsync();
            var result = this.Mapper.Map<ExpenseType, ExpenseTypeCreateViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadExpensesTypesCatalog")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await this.GetDetailsAsync(id);
            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateExpensesTypesCatalog")]
        public async Task<ActionResult> Update([FromBody] ExpenseTypeUpdateViewModel expenseTypeVM)
        {
            if (!this.ModelState.IsValid || expenseTypeVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var expenseTypeObject = await this._AppService.SingleOrDefaultAsync(d => d.ID == expenseTypeVM.ID);
            if (expenseTypeObject == null)
            {
                return BadRequest(this.ModelState);
            }

            this.Mapper.Map(expenseTypeVM, expenseTypeObject);
            await this._AppService.UpdateAsync(expenseTypeObject);
            await this._AppService.SaveChangesAsync();
            var result = await this.GetDetailsAsync(expenseTypeObject.ID);
            return new JsonResult(result);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteExpensesTypesCatalog")]
        public async Task<ActionResult> Delete(int id)
        {
            var expenseTypeObj = await this.GetDetailsAsync(id);
            if (expenseTypeObj == null)
            {
                return this.NotFound();
            }

            await this._AppService.UpdateStatusAsync(id);
            try
            {
                await this._AppService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return this.NoContent();
            }

            return this.Ok();
        }

        private async Task<ExpenseTypeDetailsViewModel> GetDetailsAsync(int id)
        {
            var obj = await this._AppService.SingleOrDefaultAsync(e => e.ID == id);
            if (obj == null)
            {
                return null;
            }

            var expenseTypeObject = this.Mapper.Map<ExpenseType, ExpenseTypeDetailsViewModel>(obj);
            return expenseTypeObject;
        }

        #endregion

        #region Expense Subcategories

        [HttpGet("{expenseTypeId:int}")]
        public async Task<JsonResult> ReadAllSubcategoriesCbo(DataSourceRequest request, int expenseTypeId, int? isEnabled)
        {
            var result = await this._AppService.ReadAllSubcategoriesCboDapperAsync(request, expenseTypeId, isEnabled);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> AddSubcategory([FromBody] ExpenseSubcategoryCreateViewModel expenseSubcategoryVM)
        {
            if (!this.ModelState.IsValid || expenseSubcategoryVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<ExpenseSubcategoryCreateViewModel, ExpenseSubcategory>(expenseSubcategoryVM);
            var expenseSubcategoryObject = await this._AppService.AddSubcategoryAsync(obj);
            await this._AppService.SaveChangesAsync();
            var result = this.Mapper.Map<ExpenseSubcategory, ExpenseSubcategoryCreateViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetSubcategory(int id)
        {
            var obj = await this.GetExpenseSubcategoryDetailsAsync(id);
            if (obj == null)
            {
                return this.NoContent();
            }

            return new JsonResult(obj);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSubcategory([FromBody] ExpenseSubcategoryUpdateViewModel expenseSubcategoryVM)
        {
            if (!this.ModelState.IsValid || expenseSubcategoryVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var expenseSubcategoryObject = await this._AppService.GetSubcategoryAsync(expenseSubcategoryVM.ID);
            if (expenseSubcategoryObject == null)
            {
                return BadRequest(this.ModelState);
            }

            this.Mapper.Map(expenseSubcategoryVM, expenseSubcategoryObject);
            await this._AppService.UpdateSubcategoryAsync(expenseSubcategoryObject);
            await this._AppService.SaveChangesAsync();
            var result = await this.GetExpenseSubcategoryDetailsAsync(expenseSubcategoryObject.ID);
            return new JsonResult(result);
        }

        private async Task<ExpenseSubcategoryDetailsViewModel> GetExpenseSubcategoryDetailsAsync(int id)
        {
            var obj = await this._AppService.GetSubcategoryAsync(id);
            if (obj == null)
            {
                return null;
            }

            var expenseSubcategory = this.Mapper.Map<ExpenseSubcategory, ExpenseSubcategoryDetailsViewModel>(obj);
            return expenseSubcategory;
        }

        #endregion
    }
}
