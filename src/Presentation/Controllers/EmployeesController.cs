using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly IEmployeesApplicationService _employeesApplicationService;
        private readonly IContactsApplicationService _contactsApplicationService;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeesController(
            IEmployeesApplicationService employeeAppService,
            IContactsApplicationService contactsApplicationService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            _userManager = userManager;
            _employeesApplicationService = employeeAppService;
            _contactsApplicationService = contactsApplicationService;
        }

        [HttpGet]
        [PermissionsFilter("ReadUsers")]
        public async Task<JsonResult> ReadAll(DataSourceRequest request, int? roleLevel, int? roleId)
        {
            var employeesVMS = await _employeesApplicationService.ReadAllAsyncDapper(request, roleLevel, roleId);
            return new JsonResult(employeesVMS);
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllByBuildingCbo(int buildingId)
        {
            try
            {
                var result = await this.EmployeeApplicationService.ReadAllByBuildingCboDapperAsync(buildingId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/employees/readallcbo
        //[HttpGet]
        //public async Task<JsonResult> ReadAllCbo()
        //{
        //    var employeesObj = await _employeesApplicationService.ReadAllAsync();
        //    var employeesVMs = employeesObj.Select(e => new ListBoxViewModel { ID = e.ID, Name = e.Contact.FullName });
        //    return new JsonResult(employeesVMs);
        //}

        // GET: api/employees/get/<guid>
        [HttpGet]
        [PermissionsFilter("ReadUsers")]
        public async Task<JsonResult> Get(Guid guid)
        {
            var employeeObj = await _employeesApplicationService.SingleOrDefaultAsync(entity => entity.Guid == guid);
            // TODO : Use a different view model
            var employeeVM = this.Mapper.Map<Employee, EmployeeGridViewModel>(employeeObj);
            return new JsonResult(employeeVM);
        }

        [HttpGet("{guid:guid}")]
        [AllowAnonymous]
        public async Task<JsonResult> PublicGet(Guid guid)
        {
            var employeeObj = await _employeesApplicationService.SingleOrDefaultAsync(entity => entity.Guid == guid);
            // TODO : Use a different view model
            var employeeVM = this.Mapper.Map<Employee, EmployeeGridViewModel>(employeeObj);
            return new JsonResult(employeeVM);
        }

        [HttpGet("{guid:guid}")]
        [PermissionsFilter("ReadUsers")]
        public async Task<IActionResult> Update(Guid guid)
        {
            if (guid == null)
            {
                return this.BadRequest(this.ModelState);
            }

            return await Update(entity => entity.Guid == guid);

        }

        [HttpGet("{id:int}")]
        [PermissionsFilter("ReadUsers")]
        public async Task<IActionResult> Update(int id)
        {
            return await Update(entity => entity.ID == id);
        }

        /// <summary>
        ///     Common method for retrieving user info
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<IActionResult> Update(Func<Employee, bool> func)
        {
            var employeeObj = await _employeesApplicationService.SingleOrDefaultAsync(func);
            if (employeeObj == null)
            {
                return this.NotFound();
            }

            var employeeVM = this.Mapper.Map<Employee, EmployeeUpdateViewModel>(employeeObj);

            return new JsonResult(employeeVM);
        }

        [HttpPut]
        [PermissionsFilter("UpdateUsers")]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateViewModel employeeVM)
        {
            if (this.ModelState.IsValid)
            {
                if (employeeVM == null)
                {
                    return BadRequest(this.ModelState);
                }
                var employeeObj = await _employeesApplicationService.SingleOrDefaultAsync(entity => entity.ID == employeeVM.ID);

                // user treatment only if email is different
                if (employeeObj.Email != employeeVM.Email)
                {
                    var user = this._userManager.Users
                                    .Where(u => u.Email == employeeObj.Email)
                                    .FirstOrDefault();
                    ContactEmail contactEmail = await _contactsApplicationService.GetContactEmailByContactIdAndEmail(employeeObj.ContactId, employeeObj.Email);
                    if (contactEmail != null)
                    {
                        contactEmail.Email = employeeVM.Email;
                        await _contactsApplicationService.UpdateEmailAsync(contactEmail);
                    }

                    if (user != null)
                    {
                        user.UserName = employeeVM.Email;
                        user.Email = employeeVM.Email;
                        await this._userManager.UpdateAsync(user);
                    }
                }

                var contactObj = employeeObj.Contact;

                this.Mapper.Map(employeeVM, employeeObj);
                // employeeObj = this._employeesApplicationService.AssignRolePermissions(employeeObj, (ent => ent.Level == employeeVM.RoleLevel));
                await this._employeesApplicationService.SaveChangesAsync();

                this._employeesApplicationService.Update(employeeObj);

                this.Mapper.Map(employeeVM, contactObj);
                this._contactsApplicationService.Update(contactObj);

                await this._employeesApplicationService.SaveChangesAsync();
                return Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllCbo(DataSourceRequest request, int? id, int? employeeId, int? roleLevel)
        {
            var employeeVM = await _employeesApplicationService.ReadAllCboAsyncDapper(request, id, employeeId, roleLevel);
            return new JsonResult(employeeVM);
        }

        [HttpGet]
        public async Task<JsonResult> ReadAllSupervisorsCbo(DataSourceRequest request, int? buildingId)
        {
            var payload = await _employeesApplicationService.ReadAllSupervisorsCboDapperAsync(request, buildingId);
            return new JsonResult(payload);
        }

        [HttpGet]
        public JsonResult ReadAllCboRoles()
        {
            var rolesVM = _employeesApplicationService.ReadAllCboRoles();
            return new JsonResult(rolesVM);
        }

        /// <summary>
        /// Gets employees given a role level and comparison operator such as: "Equal", "Unequal", "LessThan", "LessThanOrEqualTo", "GreaterThan", "GreaterThanOrEqualTo"
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <param name="roleLevel"></param>
        /// <param name="comparisonValue"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ReadAllEmployeeByRoleAndComparisonValueCbo(DataSourceRequest request, int? id, int roleLevel, ComparisonPredicate comparisonValue)
        {
            var employeeVM = await _employeesApplicationService.ReadAllCboAsyncDapper(request, comparisonValue, roleLevel, id);
            return new JsonResult(employeeVM);
        }

        [HttpDelete]
        [PermissionsFilter("DeleteUsers")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await this.EmployeeApplicationService.SingleOrDefaultAsync(e => e.ID == id);
            if (employee == null)
            {
                return NoContent();
            }
            var contact = await this.EmployeeApplicationService.GetContact(employee.ContactId);
            try
            {
                // Remove employee emails
                this._contactsApplicationService.RemoveEmails(contact.ID);
                // Remove employee phones
                this._contactsApplicationService.RemovePhones(contact.ID);
                // Remove employee adresses
                this._contactsApplicationService.RemoveAddresses(contact.ID);
                // Remove employee contact
                this._contactsApplicationService.Remove(contact.ID);
                // Remove employee
                await this.EmployeeApplicationService.RemoveAsync(employee);
                await this._contactsApplicationService.SaveChangesAsync();
                await this.EmployeeApplicationService.SaveChangesAsync();
                // Verify if no other employee shares email
                var employee2 = await this.EmployeeApplicationService.SingleOrDefaultAsync(e => e.Email == employee.Email);
                // Remove user from aspnetuser if email has no other employee linked to.
                if (employee2 == null)
                {
                    // Recover user from aspnetusers
                    var user = await this._userManager.FindByEmailAsync(employee.Email);
                    await _userManager.DeleteAsync(user);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequest("Delete user not allowed");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteByGuid(Guid guid)
        {
            var result = new EmployeeDeteleResponseViewModel();

            var employee = await this.EmployeeApplicationService.SingleOrDefaultAsync(e => e.Guid.Equals(guid));
            if (employee == null)
            {
                result.Success = false;
                result.Message = "The user doesn't exist";
            }

            var contact = await this.EmployeeApplicationService.GetContact(employee.ContactId);
            string username = contact == null ? string.Empty : contact.FullName;

            try
            {
                // Remove employee emails
                this._contactsApplicationService.RemoveEmails(contact.ID);
                // Remove employee phones
                this._contactsApplicationService.RemovePhones(contact.ID);
                // Remove employee adresses
                this._contactsApplicationService.RemoveAddresses(contact.ID);
                // Remove employee contact
                this._contactsApplicationService.Remove(contact.ID);
                // Remove employee
                await this.EmployeeApplicationService.RemoveAsync(employee);
                await this._contactsApplicationService.SaveChangesAsync();
                await this.EmployeeApplicationService.SaveChangesAsync();
                // Verify if no other employee shares email
                var employee2 = await this.EmployeeApplicationService.SingleOrDefaultAsync(e => e.Email == employee.Email);
                // Remove user from aspnetuser if email has no other employee linked to.
                if (employee2 == null)
                {
                    // Recover user from aspnetusers
                    var user = await this._userManager.FindByEmailAsync(employee.Email);
                    await _userManager.DeleteAsync(user);
                }
                result.Success = true;
                result.Message = $"The user ({username}) was deleted succesfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Delete user ({username}) not allowed";
            }

            return Ok(result);
        }
    }
}
