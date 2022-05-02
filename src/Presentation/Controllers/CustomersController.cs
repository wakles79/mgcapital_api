using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Presentation.ViewModels.DataViewModels.Address;
using MGCap.Presentation.ViewModels.DataViewModels.Customer;
using MGCap.Presentation.ViewModels.DataViewModels.EntityPhone;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MGCap.Presentation.Filters;
using System.Linq;
using System.Text;
using System.Net;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.Customer;

namespace MGCap.Presentation.Controllers
{
    public class CustomersController : BaseController
    {
        private readonly ICustomersApplicationService _customersApplicationService;

        public CustomersController(
            IEmployeesApplicationService employeeAppService,
            ICustomersApplicationService customerApplicationService,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            _customersApplicationService = customerApplicationService;
        }

        #region Customers
        // GET: api/customers/readall
        [HttpGet]
        [PermissionsFilter("ReadManagementCo")]
        public async Task<JsonResult> ReadAll(DataSourceRequestCustomer request)
        {
            var dataSource = await this._customersApplicationService.ReadAllDapperAsync(request);
            return new JsonResult(dataSource);
        }

        [HttpGet("{guid:Guid}")]
        [PermissionsFilter("ReadManagementCo")]
        public async Task<IActionResult> Update(Guid guid)
        {
            var customerObj = await _customersApplicationService.SingleOrDefaultAsync(ent => ent.Guid == guid);
            if (customerObj == null)
            {
                return this.NotFound();
            }

            var customerVMs = this.Mapper.Map<Customer, CustomerUpdateViewModel>(customerObj);

            return new JsonResult(customerVMs);
        }

        [HttpPut]
        [PermissionsFilter("UpdateManagementCo")]
        public async Task<IActionResult> Update([FromBody] CustomerUpdateViewModel customerVM)
        {
            if (this.ModelState.IsValid)
            {
                if (customerVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var customerObj = await _customersApplicationService.SingleOrDefaultAsync(ent => ent.Guid == customerVM.Guid);
                if (customerObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(customerVM, customerObj);
                await this._customersApplicationService.UpdateAsync(customerObj);

                //Remove all current assigned groups
                this._customersApplicationService.RemoveAllCustomerCustomerGroups(customerObj.ID);

                if (customerVM.GroupIds != null)
                {
                    //Insert all groups
                    this._customersApplicationService.AssignCustomerGroups(customerVM.GroupIds, customerObj.ID);
                }
                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpPost]
        [PermissionsFilter("AddManagementCo")]
        public async Task<IActionResult> Add([FromBody] CustomerCreateViewModel customerVM)
        {
            if (this.ModelState.IsValid)
            {
                if (customerVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var customerObj = this.Mapper.Map<CustomerCreateViewModel, Customer>(customerVM);
                await this._customersApplicationService.AddAsync(customerObj);
                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{guid:Guid}")]
        [PermissionsFilter("DeleteManagementCo")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var customerObj = await _customersApplicationService.SingleOrDefaultAsync(ent => ent.Guid == guid);
            if (customerObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _customersApplicationService.Remove(customerObj.ID);
            try
            {
                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return this.BadRequest("h");
            }
        }

        [HttpDelete]
        [PermissionsFilter("DeleteManagementCo")]
        public async Task<IActionResult> DeleteByGuid(Guid guid)
        {
            var result = new EmployeeDeteleResponseViewModel();

            var customer = await this._customersApplicationService.SingleOrDefaultAsync(e => e.Guid.Equals(guid));
            if (customer == null)
            {
                result.Success = false;
                result.Message = "The customer doesn't exist";
            }

            string username = customer == null ? string.Empty : customer.Name;

            try
            {
                await this._customersApplicationService.RemoveAsync(customer);
                await this._customersApplicationService.SaveChangesAsync();

                result.Success = true;
                result.Message = $"The customer ({username}) was deleted succesfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Delete customer ({username}) not allowed";
            }

            return Ok(result);
        }


        // GET: api/customers/get/<guid>
        [HttpGet("{guid:Guid}")]
        [PermissionsFilter("ReadManagementCo")]
        public async Task<IActionResult> Get(Guid guid)
        {
            if (guid == null)
            {
                return this.BadRequest();
            }

            return await Get(entity => entity.Guid == guid);
        }

        // GET: api/customers/get/<id>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            return await Get(entity => entity.ID == id);
        }

        /// <summary>
        ///     Common method for retrieving customer info
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<IActionResult> Get(Func<Customer, bool> func)
        {
            var customerObj = await _customersApplicationService.SingleOrDefaultAsync(func);
            var customerVM = this.Mapper.Map<Customer, CustomerDetailsViewModel>(customerObj);
            return new JsonResult(customerVM);
        }

        [HttpGet("{id:int?}")]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequestCustomer request, int? id)
        {
            DataSource<CustomerListBoxViewModel> vendorsVM = await this._customersApplicationService.ReadAllCboDapperAsync(request, id);
            return new JsonResult(vendorsVM);
        }

        [HttpGet]
        public async Task<ActionResult> ValidateCustomerCode(string code, int customerId = -1)
        {
            bool exist = await this.ExistingCustomerCode(code, customerId);
            return new JsonResult(exist ? "Existing" : "Available");
        }

        private async Task<bool> ExistingCustomerCode(string code, int customerId = -1)
        {
            var results = await this._customersApplicationService.ReadAllAsync(b => b.Code == code && b.ID != customerId);

            return results.Count() > 0 ? true : false;
        }

        [HttpGet]
        public async Task<FileResult> CustomerReportCsv(DataSourceRequestCustomer request)
        {
            // HACK: To ensure all records are included within the daterange filter
            request.PageSize = int.MaxValue;
            request.PageNumber = 0;

            var vm = await this._customersApplicationService.ReadAllToCsv(request);

            var csv = vm.ToCsv(true);

            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return File(bytes, "text/csv", "customer-report.csv");
        }

        #endregion

        #region Phone
        // GET: api/customers/readallphones/<customerId>
        [HttpGet("{customerId:int}")]
        public async Task<JsonResult> ReadAllPhones(int customerId)
        {
            var phonesObj = await _customersApplicationService.ReadAllPhonesAsync(customerId);
            var phonesVMs = this.Mapper.Map<IEnumerable<CustomerPhone>, IEnumerable<EntityPhoneGridViewModel>>(phonesObj);
            return new JsonResult(phonesVMs);
        }

        [HttpPost]
        [PermissionsFilter("AddManagementCoPhone")]
        public async Task<IActionResult> AddPhone([FromBody] EntityPhoneCreateViewModel phonesVM)
        {
            if (this.ModelState.IsValid)
            {
                if (phonesVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var phonesObj = this.Mapper.Map<EntityPhoneCreateViewModel, CustomerPhone>(phonesVM);
                await this._customersApplicationService.AddPhoneAsync(phonesObj);
                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(this.ModelState);
        }


        [HttpGet("{customerPhoneId:int}")]
        public async Task<IActionResult> UpdatePhone(int customerPhoneId)
        {
            var phonesObj = await _customersApplicationService.GetPhoneByIdAsync(customerPhoneId);
            if (phonesObj == null)
            {
                return this.NotFound();
            }

            var phonesVMs = this.Mapper.Map<CustomerPhone, EntityPhoneUpdateViewModel>(phonesObj);

            return new JsonResult(phonesVMs);
        }

        [HttpPut]
        [PermissionsFilter("UpdateManagementCoPhone")]
        public async Task<IActionResult> UpdatePhone([FromBody] EntityPhoneUpdateViewModel phonesVM)
        {
            if (this.ModelState.IsValid)
            {
                if (phonesVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var phonesObj = await _customersApplicationService.GetPhoneByIdAsync(phonesVM.ID);
                if (phonesObj == null)
                {
                    return BadRequest(this.ModelState);
                }

                this.Mapper.Map(phonesVM, phonesObj);
                await this._customersApplicationService.UpdatePhoneAsync(phonesObj);

                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{customerPhoneId:int}")]
        [PermissionsFilter("DeleteManagementCoPhone")]
        public async Task<IActionResult> DeletePhone(int customerPhoneId)
        {
            var phonesObj = await _customersApplicationService.GetPhoneByIdAsync(customerPhoneId);
            if (phonesObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _customersApplicationService.RemovePhone(customerPhoneId);
            await this._customersApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Address Related
        // GET: api/customers/readalladdresses/<customerId>
        [HttpGet("{customerId:int}")]
        public async Task<JsonResult> ReadAllAddresses(int customerId)
        {
            var addressesObj = await _customersApplicationService.ReadAllAddressAsync(customerId);
            var addressesVMs = this.Mapper.Map<IEnumerable<CustomerAddress>, IEnumerable<AddressGridViewModel>>(addressesObj);
            return new JsonResult(addressesVMs);
        }

        [HttpPost]
        [PermissionsFilter("AddManagementCoAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressCreateViewModel addressVM)
        {
            if (this.ModelState.IsValid)
            {
                if (addressVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var addressObj = this.Mapper.Map<AddressCreateViewModel, Address>(addressVM);
                var customerAddressObj = this.Mapper.Map<AddressCreateViewModel, CustomerAddress>(addressVM);
                await this._customersApplicationService.AddAddressAsync(addressObj);
                customerAddressObj.AddressId = addressObj.ID;
                await this._customersApplicationService.AddCustomerAddressAsync(customerAddressObj);
                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }


        [HttpGet("{customerId:int}")]
        public async Task<IActionResult> UpdateAddress(int customerId, int addressId)
        {
            var customerAddressObj = await _customersApplicationService.GetContactAddressByIdAsync(customerId, addressId);
            if (customerAddressObj == null)
            {
                return this.NotFound();
            }

            var AddressVMs = this.Mapper.Map<CustomerAddress, AddressUpdateViewModel>(customerAddressObj);

            return new JsonResult(AddressVMs);
        }

        [HttpPut]
        [PermissionsFilter("UpdateManagementCoAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdateViewModel AddressVM)
        {
            if (this.ModelState.IsValid)
            {
                if (AddressVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var customerAddressObj = await _customersApplicationService.GetContactAddressByIdAsync(AddressVM.EntityId, AddressVM.AddressId);
                if (customerAddressObj == null)
                {
                    return BadRequest(this.ModelState);
                }
                var addressObj = customerAddressObj.Address;

                this.Mapper.Map(AddressVM, customerAddressObj);
                this.Mapper.Map(AddressVM, addressObj);
                await this._customersApplicationService.UpdateCustomerAddressAsync(customerAddressObj);
                await this._customersApplicationService.UpdateAddressAsync(addressObj);

                await this._customersApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpDelete("{customerId:int}")]
        [PermissionsFilter("DeleteManagementCoAddress")]
        public async Task<IActionResult> DeleteAddress(int customerId, int addressId)
        {
            var addressObj = await _customersApplicationService.GetContactAddressByIdAsync(customerId, addressId);
            if (addressObj == null)
            {
                return BadRequest(this.ModelState);
            }
            _customersApplicationService.RemoveAddress(addressObj);
            await this._customersApplicationService.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region CustomerGroups
        // GET: api/customers/readallgroups/
        [HttpGet("{customerId:int}")]
        public async Task<JsonResult> ReadAllAssignedGroups(int customerId)
        {
            var customerGroupObj = await _customersApplicationService.ReadAllCustomerGroupAsync(customerId);
            // TODO : Use a different view model
            var customerVM = this.Mapper.Map<IEnumerable<CustomerCustomerGroup>, IEnumerable<ListBoxViewModel>>(customerGroupObj);
            return new JsonResult(customerVM);
        }

        // GET: api/customers/readallgroups/
        [HttpGet]
        public async Task<JsonResult> ReadAllGroups()
        {
            var customerGroupObj = await _customersApplicationService.ReadAllCustomerGroupAsync();
            // TODO : Use a different view model
            var customerVM = this.Mapper.Map<IEnumerable<CustomerGroup>, IEnumerable<ListBoxViewModel>>(customerGroupObj);
            return new JsonResult(customerVM);
        }
        #endregion

        #region Employees
        // GET: api/customers/readallemployees
        [HttpGet("{customerId:int}")]
        public async Task<JsonResult> ReadAllEmployees(DataSourceRequest request, int customerId)
        {
            var dataSource = await this._customersApplicationService.ReadAllEmployeesAsyncDapper(request, customerId);
            return new JsonResult(dataSource);
        }
        #endregion
    }
}
