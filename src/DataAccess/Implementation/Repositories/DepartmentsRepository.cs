// -----------------------------------------------------------------------
// <copyright file="DepartmentsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Employee"/>
    /// </summary>
    public class DepartmentsRepository : BaseRepository<Department, int>, IDepartmentsRepository
    {

        private readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DepartmentsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public DepartmentsRepository(
            IBaseDapperRepository baseDapperRepository,
            MGCapDbContext dbContext)
            : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<DepartmentGridViewModel>> ReadAllDapperAsync(int companyId, DataSourceRequest request)
        {
            var result = new DataSource<DepartmentGridViewModel>()
            {
                Payload = new List<DepartmentGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID DESC" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
	                    D.[ID],
	                    D.[Name],
	                    D.[CreatedDate]
                    FROM [dbo].[Departments] AS D
                    WHERE D.[CompanyId] = @CompanyId
	                    AND ((@Filter IS NOT NULL AND D.[Name] LIKE '%'+@Filter+'%' ) OR (@Filter IS NULL))
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Filter", string.IsNullOrEmpty(request.Filter) ? null : request.Filter);
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@PageNumber", request.PageNumber);
            parameters.Add("@PageSize", request.PageSize);

            var rows = await this._baseDapperRepository.QueryAsync<DepartmentGridViewModel>(query, parameters);
            result.Count = rows.FirstOrDefault()?.Count ?? 0;
            result.Payload = rows;

            return result;
        }
    }
}
