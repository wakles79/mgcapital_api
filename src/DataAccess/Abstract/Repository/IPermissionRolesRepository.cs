// -----------------------------------------------------------------------
// <copyright file="IPermissionRolesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Permission;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IPermissionRolesRepository : IBaseRepository<PermissionRole, int>
    {
        Task<AccessType> GetModuleAccessType(ApplicationModule module, int RoleId);

        Task<IEnumerable<PermissionBaseViewModel>> ReadAllByRoleDapperAsync(int roleId);
    }
}
