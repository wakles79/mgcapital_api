// -----------------------------------------------------------------------
// <copyright file="ICompanySettingsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CompanySettings;
using MGCap.Domain.ViewModels.Permission;
using MGCap.Domain.ViewModels.PermissionRole;
using MGCap.Domain.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ICompanySettingsApplicationService : IBaseApplicationService<CompanySettings, int>
    {
        Task<CompanySettingsDetailViewModel> GetCompanySettings();

        void UpdateDapperAsync(CompanySettings settings);

        #region Roles
        Task<AccessType> GetModuleAccessByRole(int module);

        Task<IEnumerable<PermissionAssignmentViewModel>> GetModulePermissionsByRole(ApplicationModule moduleId);

        Task<IEnumerable<PermissionRoleModuleAccessGridViewModel>> ReadModulesAccessTypeByRoleAsync();

        Task UpdateRoleModulePermisssionsAsync(PermissionRoleModuleAccessUpdateViewModel vm);

        Task<IEnumerable<Role>> ReadAllRolesAsync();

        Task<IEnumerable<PermissionRoleModuleViewModel>> GetAssignedModulePermissionsByRole(int roleId);

        Task UpdateModulePermissionRoleId(PermissionRoleUpdateViewModel permissionRoleUpdateVm);

        Task<IEnumerable<RoleListBoxViewModel>> ReadAllDefaultRolesCbo();

        Task<Role> GetRoleAsync(int id);

        Task<Role> AddRoleAsync(Role role);

        Task<Role> UpdateRoleAsync(Role role);

        Task RemoveRoleAsync(int id);
        #endregion

    }
}
