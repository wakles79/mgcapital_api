// -----------------------------------------------------------------------
// <copyright file="PermissionsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="PermissionsRepository"/>
    /// </summary>
    public class PermissionsRepository : BaseRepository<Permission, int>, IPermissionsRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PermissionsRepository"/> class.
        /// </summary>
        public PermissionsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<IEnumerable<PermissionAssignmentViewModel>> ReadAllAssignedPermissionsByRoleId(int roleId)
        {
            IEnumerable<PermissionAssignmentViewModel> result = new List<PermissionAssignmentViewModel>();

            string query = $@"
                SELECT
                    P.[ID],
                    P.[Name],
                    P.[Module],
                    P.[Type],
                    (CASE WHEN PR.RoleId IS NULL THEN 0 ELSE 1 END) AS [IsAssigned]
                FROM [dbo].[Permissions] AS P
                    LEFT JOIN [dbo].[PermissionRoles] AS PR ON PR.[PermissionId] = P.[ID] AND PR.[RoleId] = @RoleID
                ORDER BY P.[Module]";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@RoleID", roleId);

            var rows = await  this._baseDapperRepository.QueryAsync<PermissionAssignmentViewModel>(query, pars);

            if (rows.Any())
            {
                result = rows;
            }

            return result;
        }

        public async Task<IEnumerable<PermissionAssignmentViewModel>> ReadAssignedModulePermissionsByRoleId(ApplicationModule module, int companyId, string email)
        {
            IEnumerable<PermissionAssignmentViewModel> result = new List<PermissionAssignmentViewModel>();

            string query = $@"
                DECLARE @RoleID INT

                SELECT TOP 1 @RoleID = [RoleId] FROM [Employees] E WHERE E.Email = @Email AND E.CompanyId = @CompanyId

                SELECT
                    P.[ID],
                    P.[Name],
                    P.[Module],
                    P.[Type],
                    (CASE WHEN PR.RoleId IS NULL THEN 0 ELSE 1 END) AS [IsAssigned]
                FROM [dbo].[Permissions] AS P
                    LEFT JOIN [dbo].[PermissionRoles] AS PR ON PR.[PermissionId] = P.[ID] AND PR.[RoleId] = @RoleID
                WHERE P.[Module] = @ModuleID
                ORDER BY P.[Module]";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@ModuleID", (int)module);
            pars.Add("@CompanyId", companyId);
            pars.Add("@Email", email);

            var rows = await this._baseDapperRepository.QueryAsync<PermissionAssignmentViewModel>(query, pars);

            if (rows.Any())
            {
                result = rows;
            }

            return result;
        }
    }
}
