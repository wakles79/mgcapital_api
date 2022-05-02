// -----------------------------------------------------------------------
// <copyright file="IPermissionsRepository.cs" company="Axzes">
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
    /// <summary>
    ///     Contains the declaration of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public interface IPermissionsRepository : IBaseRepository<Permission, int>
    {
        Task<IEnumerable<PermissionAssignmentViewModel>> ReadAllAssignedPermissionsByRoleId(int roleId);

        Task<IEnumerable<PermissionAssignmentViewModel>> ReadAssignedModulePermissionsByRoleId(ApplicationModule module, int companyId, string email);
    }
}
