
using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.OfficeServiceType;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class OfficeServiceTypesController : BaseController
    {
        private readonly IOfficeServiceTypesApplicationService _officeServiceTypesApplicationService;

        public OfficeServiceTypesController(
            IEmployeesApplicationService employeeAppService,
            IOfficeServiceTypesApplicationService officeServiceTypesAppService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            this._officeServiceTypesApplicationService = officeServiceTypesAppService;
        }

        /// <summary>
        /// Get all the office types according the company
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionsFilter("ReadContractServicesCatalog")]
        public async Task<ActionResult<DataSource<OfficeServiceTypeGridViewModel>>> ReadAll(DataSourceRequest request, int? isEnabled)
        {
            var dataSource = await this._officeServiceTypesApplicationService.ReadAllDapperAsync(request, isEnabled);
            return new JsonResult(dataSource);
        }

        [HttpGet]
        public async Task<ActionResult<DataSource<OfficeServiceTypeListViewModel>>> ReadAllCbo(int status = -1, int rateType = -1, string exclude = "")
        {
            var result = await this._officeServiceTypesApplicationService.ReadAllCboDapperAsync(status, rateType, exclude);
            return result;
        }

        /// <summary>
        /// Create a new office type
        /// </summary>
        /// <param name="officeTypeVM"></param>
        /// <returns>A response indicating the result of the request</returns>
        [HttpPost]
        [PermissionsFilter("AddContractServicesCatalog")]
        public async Task<ActionResult<OfficeServiceTypeDetailsViewModel>> Add([FromBody] OfficeServiceTypeCreateViewModel officeTypeVM)
        {
            if (this.ModelState.IsValid)
            {
                if (officeTypeVM == null)
                {
                    return BadRequest(this.ModelState);
                }

                var officeTypeObject = this.Mapper.Map<OfficeServiceTypeCreateViewModel, OfficeServiceType>(officeTypeVM);
                await this._officeServiceTypesApplicationService.AddAsync(officeTypeObject);
                await this._officeServiceTypesApplicationService.SaveChangesAsync();
                var result = await this.GetDetailsAsync(officeTypeObject.ID);
                return new JsonResult(result);
            }

            return BadRequest(this.ModelState);
        }

        /// <summary>
        /// Edit a office type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>All the fields that will be editable of the office type</returns>
        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadContractServicesCatalog")]
        public async Task<ActionResult<OfficeServiceTypeUpdateViewModel>> Update(int id)
        {
            var officeTypeObj = await this._officeServiceTypesApplicationService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (officeTypeObj == null)
            {
                return this.NoContent();
            }

            var officeTypeVM = this.Mapper.Map<OfficeServiceType, OfficeServiceTypeUpdateViewModel>(officeTypeObj);

            return new JsonResult(officeTypeVM);
        }

        /// <summary>
        /// Upate a office type by ID
        /// </summary>
        /// <param name="officeTypeVM"></param>
        /// <returns></returns>
        [HttpPut]
        [PermissionsFilter("UpdateContractServicesCatalog")]
        public async Task<ActionResult<OfficeServiceTypeDetailsViewModel>> Update([FromBody] OfficeServiceTypeUpdateViewModel officeTypeVM)
        {
            if (this.ModelState.IsValid)
            {
                if (officeTypeVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var officeTypeObject = await _officeServiceTypesApplicationService.SingleOrDefaultAsync(ent => ent.ID == officeTypeVM.ID);
                if (officeTypeObject == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(officeTypeVM, officeTypeObject);
                await this._officeServiceTypesApplicationService.UpdateAsync(officeTypeObject);

                await this._officeServiceTypesApplicationService.SaveChangesAsync();
                var result = await this.GetDetailsAsync(officeTypeObject.ID);
                return new JsonResult(result);
            }

            return BadRequest(this.ModelState);
        }

        /// <summary>
        /// Change status of the office service type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [PermissionsFilter("DeleteContractServicesCatalog")]
        public async Task<IActionResult> Delete(int id)
        {
            var officeTypeObj = await this._officeServiceTypesApplicationService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (officeTypeObj == null)
            {
                return this.NoContent();
            }
            
            await this._officeServiceTypesApplicationService.RemoveAsync(officeTypeObj);
            try
            {
                await this._officeServiceTypesApplicationService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Do Nothing
                return this.NoContent();
            }

            return this.Ok();
        }

        /// <summary>
        /// Common method to retrieve the details view model for
        /// a given office service type
        /// </summary>
        /// <returns>The details async.</returns>
        /// <param name="id">Identifier.</param>
        private async Task<OfficeServiceTypeDetailsViewModel> GetDetailsAsync(int id)
        {
            var obj = await this._officeServiceTypesApplicationService.SingleOrDefaultAsync(ent => ent.ID == id);
            if (obj == null)
            {
                return null;
            }

            var vm = this.Mapper.Map<OfficeServiceType, OfficeServiceTypeDetailsViewModel>(obj);
            return vm;
        }
    }
}