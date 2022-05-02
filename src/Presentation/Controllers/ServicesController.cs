using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.Service;
using MGCap.Presentation.Filters;

namespace MGCap.Presentation.Controllers
{
    public class ServicesController : BaseEntityController<Service, int>
    {
        public new IServicesApplicationService AppService => base.AppService as IServicesApplicationService;
        public ServicesController(
            IEmployeesApplicationService employeeAppService,
            IServicesApplicationService appService,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
        }

        /// <summary>
        ///     Common method for retrieving all services in a ListBoxViewModel
        /// </summary>
        /// <returns>A list with all Services for the current Company with format of ListBoxViewModel</returns>
        [HttpGet]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? id)
        {
            var dataSource = await this.AppService.ReadAllCboDapperAsync(request, id);
            return new JsonResult(dataSource);
        }

        /// <summary>
        ///     Common method for retrieving Services in a ServiceGridViewModel
        /// </summary>
        /// <returns>A list with Services for the current Company with format of ServiceGridViewModel</returns>
        [HttpGet]
        [PermissionsFilter("ReadServices")]
        public async Task<JsonResult> ReadAll(DataSourceRequest request)
        {
            var dataSource = await this.AppService.ReadAllDapperAsync(request);
            return new JsonResult(dataSource);
        }

        /// <summary>
        ///     Common method for retrieving a Service by id
        /// </summary>
        /// <returns>A service for the current Company that corresponding with the giving id with format of ServiceUpdateViewModel</returns>
        [HttpGet]
        [PermissionsFilter("ReadServices")]
        public async Task<IActionResult> Get(int id)
        {
            return await this.Get<ServiceUpdateViewModel>(s => s.ID == id);
        }

        /// <summary>
        ///     Common method for add a service from a ServiceCreateViewModel
        /// </summary>
        /// <returns>A bad request in case it was not possible add the service or the service created with format of ServiceUpdateViewModel</returns>
        [HttpPost]
        [PermissionsFilter("AddServices")]
        public async Task<IActionResult> Add([FromBody] ServiceCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = this.Mapper.Map<ServiceCreateViewModel, Service>(vm);
              
            await this.AppService.AddAsync(obj);
            await this.AppService.SaveChangesAsync();
            var result = this.Mapper.Map<Service, ServiceUpdateViewModel>(obj);
            return new JsonResult(result);
        }

        /// <summary>
        ///     Common method for retriving a service by id
        /// </summary>
        /// <returns>A service for the current Company that corresponding with the giving id with format of ServiceUpdateViewModel</returns>
        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadServices")]
        public async Task<IActionResult> Update(int id)
        {
            return await this.Get<ServiceUpdateViewModel>(s => s.ID == id);           
        }

        /// <summary>
        ///     Common method for updating a service with format of ServiceUpdateViewModel
        /// </summary>
        /// <returns>The service update with format of ServiceUpdateViewModel</returns>
        [HttpPut]
        [PermissionsFilter("UpdateServices")]
        public async Task<IActionResult> Update([FromBody] ServiceUpdateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var obj = await this.AppService.SingleOrDefaultAsync(vm.ID);
            if (obj == null)
            {
                return this.BadRequest(this.ModelState);
            }            

            this.Mapper.Map(vm, obj);
            await this.AppService.UpdateAsync(obj);
            await this.AppService.SaveChangesAsync();

            var result = this.Mapper.Map<Service, ServiceUpdateViewModel>(obj);
            return new JsonResult(result);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteServices")]
        public new async Task<IActionResult> Delete(int id)
        {
            var obj = await this.AppService.SingleOrDefaultAsync(id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }
            this.AppService.Remove(obj);
            await this.AppService.SaveChangesAsync();
            return this.Ok();               
        }
    }
}
