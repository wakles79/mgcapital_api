// -----------------------------------------------------------------------
// <copyright file="PermissionRolesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.PermissionRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="PermissionRolesRepository"/>
    /// </summary>
    public class PermissionRolesRepository : BaseRepository<PermissionRole, int>, IPermissionRolesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PermissionsRepository"/> class.
        /// </summary>
        public PermissionRolesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<IEnumerable<PermissionBaseViewModel>> ReadAllByRoleDapperAsync(int roleId)
        {
            string query = $@"
                SELECT 
                    P.[ID],
                    P.[Module],
                    P.[Name],
                    P.[Type]
                FROM [dbo].[Permissions] AS P
                    INNER JOIN [PermissionRoles] PR ON PR.PermissionId = P.ID
                WHERE PR.[RoleId] = @RoleID";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@RoleID", roleId);

            var result = await this._baseDapperRepository.QueryAsync<PermissionBaseViewModel>(query, pars);

            return result;
        }

        public async Task<AccessType> GetModuleAccessType(ApplicationModule module, int roleId)
        {
            AccessType access = AccessType.None;

            string query = $@"
                SELECT 
                    P.[ID],
                    P.[Module],
                    P.[Name],
                    P.[Type]
                FROM [dbo].[Permissions] AS P
                    INNER JOIN [PermissionRoles] PR ON PR.PermissionId = P.ID
                WHERE P.[Module] = @Module AND PR.[RoleId] = @RoleID";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@Module", (int)module);
            pars.Add("@RoleID", roleId);

            var result = await this._baseDapperRepository.QueryAsync<PermissionBaseViewModel>(query, pars);

            if (result.Any())
            {
                int writeCount = result.Where(p => p.Type == ActionType.Write).Count();

                access = writeCount == 0 ? AccessType.ReadOnly : AccessType.Full;
            }

            return access;
        }
    }
}
