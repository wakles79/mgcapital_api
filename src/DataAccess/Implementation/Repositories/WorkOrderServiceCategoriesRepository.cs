// -----------------------------------------------------------------------
// <copyright file="WorkOrderServiceCategoriesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.WorkOrderServiceCategory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderServiceCategoriesRepository : BaseRepository<WorkOrderServiceCategory, int>, IWorkOrderServiceCategoriesRepository
    {
        private IBaseDapperRepository _baseDapperRepository;

        public WorkOrderServiceCategoriesRepository(
            IBaseDapperRepository baseDapperRepository,
            MGCapDbContext context
            ) : base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<WorkOrderServiceCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<WorkOrderServiceCategoryGridViewModel>()
            {
                Count = 0,
                Payload = new List<WorkOrderServiceCategoryGridViewModel>()
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";
            string query = $@"
            -- payload query
            SELECT *, [Count] = COUNT(*) OVER() FROM (
                SELECT
	                C.[ID],
	                C.[Name],
                    C.CreatedDate
                FROM [WorkOrderServiceCategories] AS C
                WHERE C.[CompanyId] = @companyId
            ) payload 
            ORDER BY {orders} CreatedDate DESC, ID
            OFFSET @pageSize * @pageNumber ROWS
            FETCH NEXT @pageSize ROWS ONLY;
            ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@companyId", companyId);
            parameters.Add("@filter", request.Filter);
            parameters.Add("@pageNumber", request.PageNumber);
            parameters.Add("@pageSize", request.PageSize);

            var rows = await this._baseDapperRepository.QueryAsync<WorkOrderServiceCategoryGridViewModel>(query, parameters);
            result.Count = rows.FirstOrDefault()?.Count ?? 0;
            result.Payload = rows;

            return result;
        }

        public async Task<DataSource<WorkOrderServiceCategoryListViewModel>> ReadAllCboDapperAsync(int companyId)
        {
            var result = new DataSource<WorkOrderServiceCategoryListViewModel>()
            {
                Count = 0,
                Payload = new List<WorkOrderServiceCategoryListViewModel>()
            };

            string query = $@"
            SELECT
	            C.[ID],
	            C.[Name]
            FROM [WorkOrderServiceCategories] AS C
            WHERE C.[CompanyId] = @companyId
            ORDER BY C.[Name] ASC";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@companyId", companyId);

            var rows = await this._baseDapperRepository.QueryAsync<WorkOrderServiceCategoryListViewModel>(query, parameters);
            result.Count = rows.FirstOrDefault()?.Count ?? 0;
            result.Payload = rows;

            return result;
        }
    }
}
