using System;
using System.Threading.Tasks;
using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Building;
using MGCap.Presentation.ViewModels.DataViewModels.Address;
using Microsoft.AspNetCore.Mvc;
using MGCap.Presentation.Filters;
using System.Linq;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using System.Xml.Linq;
using MGCap.Domain.ViewModels.Employee;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Net;
using MGCap.Domain.ViewModels.Common;
using System.Text;

namespace MGCap.Presentation.Controllers
{
    public class BuildingsController : BaseEntityController<Building, int>
    {
        public new IBuildingsApplicationService AppService => base.AppService as IBuildingsApplicationService;
        private readonly IContactsApplicationService _contactsAppService;
        private readonly ICustomersApplicationService _customerAppService;
        private readonly IWorkOrderEmployeesRepository _woEmployeeRepository;

        // HACK: Temporary for creation of the contract with the customerId selected from BuildingUpdateViewModel
        private readonly IContractsApplicationService _contractAppService;

        public BuildingsController(
            IEmployeesApplicationService employeeAppService,
            IBuildingsApplicationService appService,
            ICustomersApplicationService customerAppService,
            IContactsApplicationService contactsAppService,
            IContractsApplicationService contractAppService,
            IWorkOrderEmployeesRepository woEmployeesRepository,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
            this._customerAppService = customerAppService;
            this._contactsAppService = contactsAppService;
            this._contractAppService = contractAppService;

            _woEmployeeRepository = woEmployeesRepository;
        }

        /// <summary>
        ///     Common method for retrieving all buildings in a ListBoxViewModel
        /// </summary>
        /// <returns>A list with all the Buildings for the current Company with format of BuildingListBoxViewModel</returns>
        [HttpGet]
        public async Task<ActionResult<DataSource<BuildingListBoxViewModel>>> ReadAllCbo(DataSourceRequest request, int? id, int? employeeId)
        {
            var buildingsVM = await this.AppService.ReadAllCboDapperAsync(request, id, employeeId);
            return new JsonResult(buildingsVM);
        }

        /// <summary>
        /// Reads all buildings in a given address radio.
        /// </summary>
        /// <returns>The all in radio.</returns>
        /// <param name="address">Address.</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> ReadAllInRadio(string address)
        {
            var buildings = await this.AppService.FindBuildingsInRadioAsync(address);
            return new JsonResult(buildings);
        }

        /// <summary>
        /// Get the specified building.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="id">Identifier.</param>
        [HttpGet]
        [PermissionsFilter("ReadBuildings")]
        public async Task<ActionResult<BuildingUpdateViewModel>> Get(int id)
        {
            var result = await this.Get<BuildingUpdateViewModel>(b => b.ID == id);
            return new JsonResult(result);
        }

        private async Task<ActionResult<BuildingUpdateViewModel>> GetBuildingAsync(int id)
        {
            var obj = await this.AppService.SingleOrDefaultAsync(b => b.ID == id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }

            var vm = this.Mapper.Map<Building, BuildingUpdateViewModel>(obj);

            // add customerId to the BuildingUpdateViewModel
            //var contractObj = await this._contractAppService.SingleOrDefaultContractByBuildingAsync(id);
            //if (contractObj != null)
            //{
            //    vm.CustomerId = contractObj.CustomerId;
            //}

            return new JsonResult(vm);
        }

        //HACK: Return the customer id associated to the contract of the building
        /// <summary>
        /// Gets the specified building.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="id">Identifier.</param>
        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadBuildings")]
        public Task<ActionResult<BuildingUpdateViewModel>> Update(int id)
        {
            return this.GetBuildingAsync(id);
        }

        /// <summary>
        /// Get details of the specified building.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BuildingUpdateViewModel>> GetDetail(int id)
        {
            //return await this.Get<BuildingUpdateViewModel>(b => b.ID == id);

            var obj = await this.AppService.SingleOrDefaultAsync(b => b.ID == id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }

            var vm = this.Mapper.Map<Building, BuildingUpdateViewModel>(obj);

            // add customerId to the BuildingUpdateViewModel
            var contractObj = await this._contractAppService.SingleOrDefaultContractByBuildingAsync(id);
            if (contractObj != null)
            {
                vm.CustomerId = contractObj.CustomerId;
            }

            return new JsonResult(vm);
        }

        /// <summary>
        /// Reads all buidings.
        /// </summary>
        /// <returns>The all.</returns>
        /// <param name="request">Request.</param>
        /// <param name="isActive">Is active.</param>
        /// <param name="isAvailable">Is available.</param>
        /// <param name="customerId">Customer Id</param>
        [HttpGet]
        [PermissionsFilter("ReadBuildings")]
        public async Task<ActionResult<DataSource<BuildingGridViewModel>>> ReadAll(DataSourceRequest request, int? isActive = -1, int? isAvailable = -1, int? customerId = -1)
        {
            var dataSource = await this.AppService.ReadAllDapperAsync(request, isActive, isAvailable, customerId);
            return new JsonResult(dataSource);
        }

        /// <summary>
        /// Read all profile building
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isActive"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<DataSource<BuildingGridViewModel>>> ReadAllBuildingProfile(DataSourceRequest request, int? isActive = -1, int? isAvailable = -1)
        {
            var dataSource = await this.AppService.ReadAllDapperAsync(request, isActive, isAvailable);
            return new JsonResult(dataSource);
        }

        /// <summary>
        /// Add the specified building.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="vm">Vm.</param>
        [HttpPost]
        [PermissionsFilter("AddBuildings")]
        public async Task<ActionResult<BuildingUpdateViewModel>> Add([FromBody] BuildingCreateViewModel vm)
        {
            if (!this.ModelState.IsValid || vm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            bool invalidCode = await this.ExistingBuildingCode(vm.Code);
            if (invalidCode)
            {
                return BadRequest("The building code has already been taken");
            }

            var obj = this.Mapper.Map<BuildingCreateViewModel, Building>(vm);
            var addressObj = this.Mapper.Map<AddressViewModel, Address>(vm.Address);

            await this._contactsAppService.AddAddressAsync(addressObj);
            obj.AddressId = addressObj.ID;

            await this.AppService.AddAsync(obj);
            await this.AppService.SaveChangesAsync();
            var result = this.Mapper.Map<Building, BuildingUpdateViewModel>(obj);
            return new JsonResult(result);
        }

        //HACK: Tempory add or update a contract for the building
        /// <summary>
        /// Update the specified building.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="vm">Vm.</param>
        [HttpPut]
        [PermissionsFilter("UpdateBuildings")]
        public async Task<ActionResult<BuildingUpdateViewModel>> Update([FromBody] BuildingUpdateViewModel vm)
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

            bool invalidCode = await this.ExistingBuildingCode(vm.Code, vm.ID);
            if (invalidCode)
            {
                return BadRequest("The building code has already been taken");
            }

            this.Mapper.Map(vm, obj);
            obj.Employees.Clear();
            await this.AppService.UpdateAsync(obj);
            await this.AppService.SaveChangesAsync();

            // Removes existing BuildingEmployees
            await AppService.UnassignEmployeesByBuildingIdAsync(obj.ID);

            var woIds = await AppService.GetOpenWorkOrderIds(obj.ID);

            // Removes existing WorkOrderEmployees
            await _woEmployeeRepository.RemoveByWorkOrderIdsAsync(woIds);

            if (vm.Employees.Any())
            {
                var buildingEmployees = vm.Employees.Select(e => new EntityEmployee
                {
                    EmployeeId = e.ID,
                    Type = (int)e.Type,
                    EntityId = obj.ID
                });

                await AppService.AssignEmployeesDapperAsync(buildingEmployees);

                var workOrderEmployees = new List<EntityEmployee>();

                foreach (var woId in woIds)
                {
                    foreach (var employee in vm.Employees)
                    {
                        workOrderEmployees.Add(new EntityEmployee
                        {
                            EmployeeId = employee.ID,
                            Type = (int)employee.Type,
                            EntityId = woId
                        });
                    }
                }

                await _woEmployeeRepository.AssignEmployeesDapperAsync(workOrderEmployees);
            }

            return await this.GetBuildingAsync(obj.ID);
        }

        /// <summary>
        /// Delete the specified building.
        /// </summary>
        /// <returns>The delete.</returns>
        /// <param name="id">Identifier.</param>
        [HttpDelete("{id:int}")]
        [PermissionsFilter("DeleteBuildings")]
        public new async Task<IActionResult> Delete(int id)
        {
            var obj = await this.AppService.SingleOrDefaultAsync(id);
            if (obj == null)
            {
                return this.NotFound(this.ModelState);
            }
            obj.IsActive = false;
            this.AppService.Update(obj);
            await this.AppService.SaveChangesAsync();
            return this.Ok();
        }

        #region Address
        [HttpPut]
        [PermissionsFilter("UpdateBuildingAddress")]
        public async Task<ActionResult<Address>> UpdateAddress([FromBody] AddressUpdateViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                if (vm == null)
                {
                    return BadRequest(this.ModelState);
                }
                var obj = await this.AppService.SingleOrDefaultAddressAsync(vm.AddressId);
                if (obj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(vm, obj);
                await this._contactsAppService.UpdateAddressAsync(obj);
                await this.AppService.SaveChangesAsync();
                return new JsonResult(obj);
            }

            return this.BadRequest(this.ModelState);
        }
        #endregion

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeBuildingViewModel>>> GetSupervisors(int buildingId)
        {
            try
            {
                var result = await AppService.GetEmployeesByBuildingId(buildingId, BuildingEmployeeType.Supervisor);
                return new JsonResult(result);
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
        public async Task<ActionResult<DataSource<BuildingListBoxViewModel>>> ReadAllCboByContact(DataSourceRequest request, int contactId, int? id = null)
        {
            var buildingsVM = await this.AppService.ReadAllByContactCboDapperAsync(request, id, contactId);
            return new JsonResult(buildingsVM);
        }

        /// <summary>
        /// Reads all buildings given a customerId in a "list-box like" format
        /// </summary>
        /// <returns>Buildings ListBox data source.</returns>
        /// <param name="request">Request.</param>
        /// <param name="customerId">Customer identifier.</param>
        /// <param name="id">Building Identifier.</param>
        [HttpGet]
        public async Task<ActionResult<DataSource<BuildingListBoxViewModel>>> ReadAllCboByCustomer(DataSourceRequest request, int customerId, int? id = null)
        {
            var buildingsVM = await this.AppService.ReadAllByCustomerCboDapperAsync(request, customerId, id);
            return new JsonResult(buildingsVM);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingByOperationsManagerListBoxViewModel>>> ReadAllBuildingsByOperationsManager(DataSourceRequestBuildingsByOperationsManager request)
        {
            try
            {
                var buildingsVM = await this.AppService.ReadAllBuildingsByOperationsManagerDapperAsync(request);
                return new JsonResult(buildingsVM);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.PreconditionFailed, "Error fetching buildings");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingOperationManagerGridViewModel>>> ReadAllSharedBuildingsByOperationsManager(DataSourceRequest request, int currentOperationsManager, int? operationsManager)
        {
            try
            {
                var buildingsVM = await this.AppService.ReadAllSharedBuildingsFromOperationsManagerDapperAsync(request, currentOperationsManager, operationsManager);
                return new JsonResult(buildingsVM);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.PreconditionFailed, "Error fetching buildings");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSharedBuildingOperationManager([FromBody] BuildingUpdateSharedBuildingsEmployeeViewModel vm)
        {
            try
            {
                await this.AppService.UpdateSharedBuildingOperationManager(vm);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeBuildings([FromBody] BuildingUpdateEmployeeBuildingsViewModel vm)
        {
            try
            {
                await this.AppService.UpdateEmployeeBuildings(vm);
                await this.AppService.SaveChangesAsync();
                return this.Ok();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return StatusCode((int)HttpStatusCode.PreconditionFailed, "Error updating employee's buildings");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBuildingPDFBase64(int id)
        {
            var result = await this.AppService.GetBuildingReportBase64(id);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBuildingsReportUrl(DataSourceRequest request, int? isActive = -1, int? isAvailable = -1)
        {
            // Hardoing page size and page number
            request.PageNumber = 0;
            request.PageSize = int.MaxValue;
            var result = await this.AppService.GetBuildingsReportUrl(request, isActive, isAvailable);
            return new JsonResult(result);
        }

        /// <summary>
        /// Fetches buildings with a list-box format with 
        /// location as additional information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingWithLocationViewModel>>> GetBuildingsWithLocationCbo()
        {
            var result = await this.AppService.GetBuildingsWithLocationCboAsync();
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<ActionResult> ValidateBuildingCode(string code, int buildingId = -1)
        {
            bool exist = await this.ExistingBuildingCode(code, buildingId);
            return new JsonResult(exist ? "Existing" : "Available");
        }

        private async Task<bool> ExistingBuildingCode(string code, int buildingId = -1)
        {
            var results = await this.AppService.ReadAllAsync(b => b.Code == code && b.ID != buildingId);

            return results.Count() > 0 ? true : false;
        }

        [HttpGet]
        public async Task<FileResult> ExportBuildingReportCsv(DataSourceRequest request, int? isActive = -1, int? isAvailable = -1, int? customerId = -1)
        {
            // HACK: To ensure all records are included within the daterange filter
            request.PageSize = int.MaxValue;
            request.PageNumber = 0;

            var vm = await this.AppService.ReadAllToCsv(request, isActive, isAvailable, customerId);

            var csv = vm.ToCsv(true);

            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return File(bytes, "text/csv", "building-report.csv");
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllActivityLog(DataSourceRequest request, int buildingId, int activityType = -1)
        {
            try
            {
                var result = await this.AppService.ReadAllActivityLogAsync(request, buildingId, activityType);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}