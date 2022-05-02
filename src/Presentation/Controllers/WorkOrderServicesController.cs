using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.WorkOrderService;
using MGCap.Domain.ViewModels.WorkOrderServiceCategory;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class WorkOrderServicesController : BaseEntityController<WorkOrderServiceCategory, int>
    {
        private new IWorkOrderServiceCategoriesApplicationService AppService => base.AppService as IWorkOrderServiceCategoriesApplicationService;

        public WorkOrderServicesController
         (
             IEmployeesApplicationService employeeAppService,
             IWorkOrderServiceCategoriesApplicationService appService,
             IMapper mapper
         ) : base(employeeAppService, appService, mapper)
        {

        }

        #region Services
        [HttpGet]
        [PermissionsFilter("ReadWorkOrderServices")]
        public async Task<IActionResult> ReadAll(DataSourceRequest request, int categoryId = -1)
        {
            try
            {
                var result = await this.AppService.ReadAllServicesAsync(request, categoryId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllServicesCbo(int categoryId = -1, IEnumerable<int> categoryIds = null)
        {
            try
            {
                var result = await this.AppService.ReadAllCboServicesAsync(categoryId, categoryIds);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [PermissionsFilter("AddWorkOrderServices")]
        public async Task<IActionResult> Add([FromBody] WorkOrderServiceCreateViewModel vm)
        {
            if (vm == null || !this.ModelState.IsValid)
            {
                return BadRequest();
            }

            var service = this.Mapper.Map<WorkOrderServiceCreateViewModel, WorkOrderService>(vm);
            var result = await this.AppService.AddServiceAsync(service);
            await this.AppService.SaveChangesAsync();
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadWorkOrderServices")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.AppService.GetServiceById(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateWorkOrderServices")]
        public async Task<IActionResult> Update([FromBody] WorkOrderServiceUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var serviceObj = await this.AppService.GetServiceById(vm.ID);
            if (serviceObj == null)
            {
                return this.NoContent();
            }

            this.Mapper.Map(vm, serviceObj);
            var result = await this.AppService.UpdateServiceAsync(serviceObj);
            await this.AppService.SaveChangesAsync();

            return new JsonResult(result);
        }
        #endregion


        #region Categories
        [HttpGet]
        [PermissionsFilter("ReadWorkOrderServices")]
        public async Task<IActionResult> ReadAllCategories(DataSourceRequest request)
        {
            try
            {
                var result = await this.AppService.ReadAllCategoriesAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllCategoriesCbo()
        {
            try
            {
                var result = await this.AppService.ReadAllCboCategoriesAsync();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [PermissionsFilter("AddWorkOrderServices")]
        public async Task<IActionResult> AddCategory([FromBody] WorkOrderServiceCategoryCreateViewModel vm)
        {
            if (vm == null || !this.ModelState.IsValid)
            {
                return BadRequest();
            }

            var category = this.Mapper.Map<WorkOrderServiceCategoryCreateViewModel, WorkOrderServiceCategory>(vm);
            var result = await this.AppService.AddAsync(category);
            await this.AppService.SaveChangesAsync();
            return new JsonResult(result);
        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadWorkOrderServices")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await this.AppService.SingleOrDefaultAsync(id);

            if (result == null)
            {
                return this.NoContent();
            }

            return new JsonResult(result);
        }

        [HttpPut]
        [PermissionsFilter("UpdateWorkOrderServices")]
        public async Task<IActionResult> UpdateCategory([FromBody] WorkOrderServiceCategoryUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var categoryObj = await this.AppService.SingleOrDefaultAsync(vm.ID);
            if (categoryObj == null)
            {
                return this.NoContent();
            }

            this.Mapper.Map(vm, categoryObj);
            var result = await this.AppService.UpdateAsync(categoryObj);
            await this.AppService.SaveChangesAsync();

            return new JsonResult(result);
        }
        #endregion
    }
}
