using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CompanySettings;
using MGCap.Domain.ViewModels.Freshdesk;
using MGCap.Domain.ViewModels.PermissionRole;
using MGCap.Domain.ViewModels.Role;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MGCap.Presentation.Controllers
{
    public class CompanySettingsController : BaseEntityController<CompanySettings, int>
    {
        protected new ICompanySettingsApplicationService AppService => base.AppService as ICompanySettingsApplicationService;
        protected readonly IAzureStorage _azureStorage;
        private readonly IFreshdeskApplicationService FreshdeskApplicationService;
        private readonly IGMailApiService GmailApiService;

        public CompanySettingsController(
            IEmployeesApplicationService employeeAppService,
            ICompanySettingsApplicationService appService,
            IFreshdeskApplicationService freshdeskApplicationService,
            IGMailApiService gmailApiService,
            IAzureStorage azureStorage,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
            _azureStorage = azureStorage;
            this.FreshdeskApplicationService = freshdeskApplicationService;
            this.GmailApiService = gmailApiService;
        }

        [HttpGet]
        [PermissionsFilter("ReadCompanySettings")]
        public async Task<IActionResult> Get()
        {
            var result = await AppService.GetCompanySettings();
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> PublicGet()
        {
            var result = await AppService.GetCompanySettings();
            return new JsonResult(result);
        }

        [AllowAnonymous]
        [HttpPost("Tick")]
        public async Task<IActionResult> Gmail()
        {
            try
            {
                var vm = await this.AppService.GetCompanySettings();
                var obj = this.Mapper.Map<CompanySettingsDetailViewModel, CompanySettings>(vm);

                if (obj.GmailEnabled)
                {
                    obj.LastHistoryId = this.GmailApiService.StartService();
                }
                else
                {
                    this.GmailApiService.StopService();
                }
            
                return Ok(new { CompanyId = obj.CompanyId, LastHistoryId = obj.LastHistoryId });
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.PreconditionFailed, e.Message);
            }
        }

        [HttpPut]
        [PermissionsFilter("UpdateCompanySettings")]
        public async Task<IActionResult> Update([FromBody] CompanySettingsUpdateViewModel settingsViewModel)
        {
            if (!this.ModelState.IsValid || settingsViewModel == null)
            {
                return BadRequest(this.ModelState);
            }

            try
            {
                var settings = this.Mapper.Map<CompanySettingsUpdateViewModel, CompanySettings>(settingsViewModel);

                if (settings.ID == 0)
                {
                    await this.AppService.AddAsync(settings);
                }
                else
                {
                    var obj = await this.AppService.GetCompanySettings();
                    settings.FreshdeskEmail = obj.FreshdeskEmail;
                    settings.LogoBlobName = obj.LogoBlobName;
                    settings.LogoFullUrl = obj.LogoFullUrl;
                    settings.LastHistoryId = obj.LastHistoryId;
                    //// Verify ig Gmail email changes
                    //if (settings.GmailEmail != obj.GmailEmail)
                    //{
                    //    // If changes, verify if service was running
                    //    if (obj.GmailEnabled)
                    //    {
                    //        // Stop service
                    //        this.GmailApiService.StopService();
                    //    }
                    //}
                    // Verify if GmailServiceStatus changes
                    if(settings.GmailEnabled != obj.GmailEnabled)
                    {
                        // If service is enabled, recover new historyId
                        if (settings.GmailEnabled)
                            settings.LastHistoryId = this.GmailApiService.StartService();
                        else
                            this.GmailApiService.StopService();
                    }
                    this.AppService.UpdateDapperAsync(settings);
                }

                await this.AppService.SaveChangesAsync();

                var result = await AppService.GetCompanySettings();

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [PermissionsFilter("UpdateCompanySettings")]
        public async Task<IActionResult> UpdateLogo([FromBody] CompanySettingsUpdateLogoViewModel vm)
        {
            try
            {
                var settings = await this.AppService.SingleOrDefaultAsync(vm.CompanySettingsId);
                if (settings == null)
                {
                    var companySettings = new CompanySettings();
                    companySettings.LogoBlobName = vm.BlobName;
                    companySettings.LogoFullUrl = vm.FullUrl;
                    await this.AppService.AddAsync(companySettings);
                }
                else
                {
                    if (!string.IsNullOrEmpty(settings.LogoBlobName))
                    {
                        await _azureStorage.DeleteImageAsync(blobName: settings.LogoBlobName);
                    }

                    settings.LogoBlobName = vm.BlobName;
                    settings.LogoFullUrl = vm.FullUrl;
                    await this.AppService.UpdateAsync(settings);
                }

                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #region Permissions
        /// <summary>
        /// to get module access type (Full, ReadOnly), called in module-resolver
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetModuleAccess(int module)
        {
            var result = await this.AppService.GetModuleAccessByRole(module);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetModulePermissions(ApplicationModule module)
        {
            try
            {
                var result = await this.AppService.GetModulePermissionsByRole(module);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [PermissionsFilter("ReadPermissionsRoles")]
        public async Task<IActionResult> GetModuleAccesByRole(int roleId)
        {
            var result = await this.AppService.GetAssignedModulePermissionsByRole(roleId);
            return new JsonResult(result);
        }

        [HttpPost]
        [PermissionsFilter("UpdatePermissionsRoles")]
        public async Task<IActionResult> UpdateRoleModuleAccess([FromBody] PermissionRoleModuleAccessUpdateViewModel vm)
        {
            try
            {
                await this.AppService.UpdateRoleModulePermisssionsAsync(vm);

                await this.AppService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [PermissionsFilter("UpdatePermissionsRoles")]
        public async Task<IActionResult> UpdateRoleModulePermission([FromBody] PermissionRoleUpdateViewModel vm)
        {
            try
            {
                await this.AppService.UpdateModulePermissionRoleId(vm);
                await this.AppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [PermissionsFilter("ReadPermissionsRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await this.AppService.ReadAllRolesAsync();
            result = result.OrderBy(r => r.Level);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleModuleAccess()
        {
            var result = await this.AppService.ReadModulesAccessTypeByRoleAsync();
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDefaultRolesCbo()
        {
            var result = await this.AppService.ReadAllDefaultRolesCbo();
            return new JsonResult(result);
        }
        #endregion

        #region Roles
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var role = await this.AppService.GetRoleAsync(id);
            var result = this.Mapper.Map<Role, RoleUpdateViewModel>(role);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleCreateViewModel vm)
        {
            try
            {
                if (vm == null || !this.ModelState.IsValid)
                {
                    return BadRequest();
                }

                var roleObj = this.Mapper.Map<RoleCreateViewModel, Role>(vm);

                var newRole = await this.AppService.AddRoleAsync(roleObj);
                await this.AppService.SaveChangesAsync();

                var result = this.Mapper.Map<Role, RoleBaseViewModel>(newRole);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateViewModel vm)
        {
            try
            {
                if (vm == null || !this.ModelState.IsValid)
                {
                    return BadRequest();
                }

                var role = await this.AppService.GetRoleAsync(vm.ID);
                if (role == null)
                {
                    return NoContent();
                }

                if (role.Type == RoleType.Default)
                {
                    return BadRequest("Cannot update this role");
                }

                this.Mapper.Map(vm, role);
                var updatedRole = await this.AppService.UpdateRoleAsync(role);
                await this.AppService.SaveChangesAsync();

                var result = this.Mapper.Map<Role, RoleUpdateViewModel>(updatedRole);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var role = await this.AppService.GetRoleAsync(id);
                if (role == null)
                {
                    return NoContent();
                }

                if (role.Type == RoleType.Default)
                {
                    return BadRequest("Cannot remove this role");
                }

                await this.AppService.RemoveRoleAsync(id);
                await this.AppService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        #endregion

        #region Freshdesk
        [HttpPost]
        public IActionResult VerifyFreshdesk([FromBody] VerifyFresdeshAccessViewModel vm)
        {
            var result = this.FreshdeskApplicationService.VerifyAccess(vm);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("The data entered is invalid");
            }
        }
        #endregion
    }
}
