using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ExpenseType;
using MGCap.Domain.ViewModels.ScheduleCategories;
using MGCap.Domain.ViewModels.ScheduleSubCategories;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class ScheduleSettingsCategoryController : BaseController
    {
        private readonly IScheduleCategoriesApplicationService _AppService;

        public ScheduleSettingsCategoryController(
            IEmployeesApplicationService employeeAppService,
            IScheduleCategoriesApplicationService appService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            this._AppService = appService;
        }

        #region CATEGORY
        [HttpGet]
        [PermissionsFilter("ReadScheduledWorkOrderCategories")]
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
        [PermissionsFilter("AddScheduledWorkOrderCategories")]
        public async Task<ActionResult> Add([FromBody] ScheduleCategoryCreateViewModel scheduleVM)
        {
            if (!this.ModelState.IsValid || scheduleVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<ScheduleCategoryCreateViewModel, ScheduleSettingCategory>(scheduleVM);

            var expenseTypeObject = await this._AppService.AddAsync(obj);

            await this._AppService.SaveChangesAsync();
            var result = this.Mapper.Map<ScheduleSettingCategory, ScheduleCategoryCreateViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpDelete]
        [PermissionsFilter("ReadScheduledWorkOrderCategories")]
        public async Task<ActionResult> Delete(int id)
        {
            var Obj = await this._AppService.SingleOrDefaultAsync(id);
            if (Obj == null)
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

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadScheduledWorkOrderCategories")]
        public async Task<ActionResult> Get(int id)
        {
            var obj = await this._AppService.SingleOrDefaultAsync(e => e.ID == id);
            if (obj == null)
            {
                return null;
            }

            var result = this.Mapper.Map<ScheduleSettingCategory, ScheduleCategoryDetailsViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateScheduledWorkOrderCategories")]
        public async Task<ActionResult> Update([FromBody] ScheduleCategoryUpdateViewModel scheduleVM)
        {
            if (!this.ModelState.IsValid || scheduleVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var objSchedule = await this._AppService.SingleOrDefaultAsync(d => d.ID == scheduleVM.ID);
            if (objSchedule == null)
            {
                return BadRequest(this.ModelState);
            }

            this.Mapper.Map(scheduleVM, objSchedule);
            await this._AppService.UpdateAsync(objSchedule);
            await this._AppService.SaveChangesAsync();
            var result = await this._AppService.SingleOrDefaultAsync(objSchedule.ID);
            return new JsonResult(result);
        }
        #endregion CATEGORY

        #region SUBCATEGORY
        [HttpPost]
        public async Task<ActionResult> AddSubcategory([FromBody] ScheduleSubCategoryCreateViewModel scheduleSubcategoryVM)
        {
            if (!this.ModelState.IsValid || scheduleSubcategoryVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<ScheduleSubCategoryCreateViewModel, ScheduleSettingSubCategory>(scheduleSubcategoryVM);
            var expenseSubcategoryObject = await this._AppService.AddSubcategoryAsync(obj);
            await this._AppService.SaveChangesAsync();
            var result = this.Mapper.Map<ScheduleSettingSubCategory, ScheduleSubCategoryCreateViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<JsonResult> ReadAllSubcategoriesCbo(DataSourceRequestScheduleCategory request, int categoryId, int? isEnabled)
        {
            var result = await this._AppService.ReadAllSubcategoriesCboDapperAsync(request, categoryId, isEnabled);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllMultipleSubcategoriesCbo(DataSourceRequestScheduleCategory request)
        {
            var result = await this._AppService.ReadAllMultipleSubcategoriesCbo(request);
            return new JsonResult(result);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSubcategory([FromBody] ScheduleSubCategoryUpdateViewModel scheduleSubcategoryVM)
        {
            if (!this.ModelState.IsValid || scheduleSubcategoryVM == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var SubcategoryObject = await this._AppService.GetSubcategoryAsync(scheduleSubcategoryVM.ID);
            if (SubcategoryObject == null)
            {
                return BadRequest(this.ModelState);
            }

            this.Mapper.Map(scheduleSubcategoryVM, SubcategoryObject);
            await this._AppService.UpdateSubcategoryAsync(SubcategoryObject);
            await this._AppService.SaveChangesAsync();
            var result = await this._AppService.GetSubcategoryAsync(SubcategoryObject.ID);
            return new JsonResult(result);
        }
        #endregion SUBCATEGORY
    }
}
