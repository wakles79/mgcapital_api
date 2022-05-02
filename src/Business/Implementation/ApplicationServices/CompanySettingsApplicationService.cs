// -----------------------------------------------------------------------
// <copyright file="CompanySettingsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CompanySettings;
using MGCap.Domain.ViewModels.Permission;
using MGCap.Domain.ViewModels.PermissionRole;
using MGCap.Domain.ViewModels.Role;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class CompanySettingsApplicationService : BaseSessionApplicationService<CompanySettings, int>, ICompanySettingsApplicationService
    {
        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        protected new ICompanySettingsRepository Repository => base.Repository as ICompanySettingsRepository;
        private readonly IEmployeesApplicationService EmployeesApplicationService;
        private readonly IPermissionRolesRepository PermissionRolesRepository;
        private readonly IRolesRepository RolesRepository;
        private readonly IPermissionsRepository PermissionsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanySettingsApplicationService"/> class.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="httpContextAccessor"></param>
        public CompanySettingsApplicationService(
            ICompanySettingsRepository repository,
            IEmployeesApplicationService employeesApplicationService,
            IPermissionRolesRepository permissionRolesRepository,
            IRolesRepository rolesRepository,
            IPermissionsRepository permissionsRepository,
        IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            this.EmployeesApplicationService = employeesApplicationService;
            this.PermissionRolesRepository = permissionRolesRepository;
            this.RolesRepository = rolesRepository;
            this.PermissionsRepository = permissionsRepository;
        }

        public async Task<CompanySettingsDetailViewModel> GetCompanySettings()
        {
            return await this.Repository.GetCompanySettingsDapperAsync(this.CompanyId);
        }

        public void UpdateDapperAsync(CompanySettings settings)
        {
            this.Repository.UpdateCompanySettingDapperAsync(settings);
        }
        #region Roles
        public Task<Role> GetRoleAsync(int id)
        {
            return this.RolesRepository.SingleOrDefaultAsync(r => r.ID == id);
        }

        public Task<Role> AddRoleAsync(Role role)
        {
            return this.RolesRepository.AddAsync(role);
        }

        public Task<Role> UpdateRoleAsync(Role role)
        {
            return this.RolesRepository.UpdateAsync(role);
        }

        public async Task RemoveRoleAsync(int id)
        {
            await this.RolesRepository.RemoveAsync(r => r.ID == id);
        }

        public async Task<IEnumerable<Role>> ReadAllRolesAsync()
        {
            var result = await this.RolesRepository.ReadAllAsync(r => (r.Level > 10 || (r.Type == RoleType.Customizable)) && r.Level != 110);
            return result;
        }

        public async Task<AccessType> GetModuleAccessByRole(int module)
        {
            var companyId = this.CompanyId;
            var userEmail = this.UserEmail;
            
            var role = await this.EmployeesApplicationService.GetEmployeeRoleAsync(companyId, userEmail);

            if (role == null)
            {
                return AccessType.None;
            }

            if (role.Level == 10)
            {
                return AccessType.Full;
            }

            var result = await this.PermissionRolesRepository.GetModuleAccessType((ApplicationModule)module, role.ID);

            return result;
        }

        public async Task<IEnumerable<PermissionAssignmentViewModel>> GetModulePermissionsByRole(ApplicationModule moduleId) {

            //var employee = await this.EmployeesApplicationService.SingleOrDefaultAsync(e => e.CompanyId == this.CompanyId && e.Email == this.UserEmail);

            //if (!employee.RoleId.HasValue)
            //{
            //    throw new Exception("User does not have a valid role");
            //}

            return await this.PermissionsRepository.ReadAssignedModulePermissionsByRoleId(moduleId, this.CompanyId, this.UserEmail);
        }

        public async Task<IEnumerable<PermissionRoleModuleAccessGridViewModel>> ReadModulesAccessTypeByRoleAsync()
        {
            var employee = await this.EmployeesApplicationService.SingleOrDefaultAsync(e => e.CompanyId == this.CompanyId && e.Email == this.UserEmail);

            var result = new List<PermissionRoleModuleAccessGridViewModel>();
            var permissions = await this.PermissionRolesRepository.ReadAllByRoleDapperAsync(employee.RoleId.Value);

            foreach (ApplicationModule module in (ApplicationModule[])Enum.GetValues(typeof(ApplicationModule)))
            {
                int modulePermissions = permissions.Where(p => p.Module == module).Count();
                int writeCount = permissions.Where(p => p.Module == module && p.Type == ActionType.Write).Count();

                // hack: to show dashboard 
                if (module == ApplicationModule.Dashboard)
                {
                    modulePermissions = 1;
                }

                result.Add(new PermissionRoleModuleAccessGridViewModel()
                {
                    Module = module,
                    Type = modulePermissions == 0 ? AccessType.None : (writeCount == 0 ? AccessType.ReadOnly : AccessType.Full)
                });
            }

            return result;
        }

        public async Task UpdateRoleModulePermisssionsAsync(PermissionRoleModuleAccessUpdateViewModel vm)
        {
            // Load Module Permissions
            var modulePermissions = await this.PermissionsRepository.ReadAllAsync(p => p.Module == vm.Module);

            // Remove Permissions From The Role of the Current Module
            foreach (var permission in modulePermissions)
            {
                await this.PermissionRolesRepository.RemoveAsync(r => r.RoleId == vm.RoleId && r.PermissionId == permission.ID);
            }

            var newPermissionRoles = new List<PermissionRole>();

            // Check new access type
            switch (vm.Type)
            {
                case AccessType.Full:
                    // create all permissions role
                    newPermissionRoles = modulePermissions
                        .Select(p => new PermissionRole() { ID = 0, PermissionId = p.ID, RoleId = vm.RoleId }).ToList();
                    break;
                case AccessType.ReadOnly:
                    // create read permissions role
                    newPermissionRoles = modulePermissions
                        .Where(p => p.Type == ActionType.Read)
                        .Select(p => new PermissionRole() { ID = 0, PermissionId = p.ID, RoleId = vm.RoleId }).ToList();
                    break;
                case AccessType.None:
                    break;
                default:
                    break;
            }

            if (newPermissionRoles.Count > 0)
            {
                this.PermissionRolesRepository.AddRange(newPermissionRoles);
            }
        }

        public async Task<IEnumerable<PermissionRoleModuleViewModel>> GetAssignedModulePermissionsByRole(int roleId)
        {
            IList<PermissionRoleModuleViewModel> result = new List<PermissionRoleModuleViewModel>();

            var permissions = await this.PermissionsRepository.ReadAllAssignedPermissionsByRoleId(roleId);

            foreach (ApplicationModule module in (ApplicationModule[])Enum.GetValues(typeof(ApplicationModule)))
            {
                result.Add(new PermissionRoleModuleViewModel()
                {
                    Module = module,
                    Permissions = permissions.Where(p => p.Module == module)
                }); ;
            }

            return result.AsEnumerable();
        }

        public async Task UpdateModulePermissionRoleId(PermissionRoleUpdateViewModel permissionRoleUpdateVm)
        {
            var permissionRole = await this.PermissionRolesRepository.SingleOrDefaultAsync(p => p.PermissionId == permissionRoleUpdateVm.PermissionId && p.RoleId == permissionRoleUpdateVm.RoleId);


            if (permissionRole == null && permissionRoleUpdateVm.IsAssigned)
            {
                await this.PermissionRolesRepository.AddAsync(new PermissionRole()
                {
                    ID = 0,
                    PermissionId = permissionRoleUpdateVm.PermissionId,
                    RoleId = permissionRoleUpdateVm.RoleId
                });
            }
            else if (permissionRole != null && !permissionRoleUpdateVm.IsAssigned)
            {
                await this.PermissionRolesRepository.RemoveAsync(permissionRole);
            }

        }

        public async Task<IEnumerable<RoleListBoxViewModel>> ReadAllDefaultRolesCbo()
        {
            var roles = await this.RolesRepository.ReadAllAsync(r => r.Type == RoleType.Default && r.Level != 110);

            var result = roles.Select(r => new RoleListBoxViewModel()
            {
                ID = r.ID,
                Level = r.Level,
                Name = r.Name
            });

            return result;
        }
        #endregion

    }
}
