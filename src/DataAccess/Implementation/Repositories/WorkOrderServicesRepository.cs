// -----------------------------------------------------------------------
// <copyright file="WorkOrderServicesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.WorkOrderService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderServicesRepository : BaseRepository<WorkOrderService, int>, IWorkOrderServicesRepository
    {
        private IBaseDapperRepository _baseDapperRepository;

        public WorkOrderServicesRepository(
             IBaseDapperRepository baseDapperRepository,
            MGCapDbContext context
            ) : base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<WorkOrderServiceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int categoryId = -1)
        {
            var result = new DataSource<WorkOrderServiceGridViewModel>()
            {
                Count = 0,
                Payload = new List<WorkOrderServiceGridViewModel>()
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string where = "";

            if (categoryId > 0)
            {
                where += " AND S.[WorkOrderServiceCategoryId] = @categoryId";
            }

            string query = $@"
            -- payload query
            SELECT *, [Count] = COUNT(*) OVER() FROM (
                SELECT
	                S.[ID],
	                S.[WorkOrderServiceCategoryId],
	                S.[Name],
	                S.[UnitFactor],
	                S.[Frequency],
	                S.[Rate],
	                C.[Name] AS [CategoryName],
                    S.CreatedDate,
                    S.[RequiresScheduling],
                    S.[QuantityRequiredAtClose],
                    S.[HoursRequiredAtClose]
                FROM [WorkOrderServices] AS S
	                INNER JOIN [WorkOrderServiceCategories] AS C ON C.[ID] = S.[WorkOrderServiceCategoryId]
                WHERE C.[CompanyId] = @companyId {where}
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
            parameters.Add("@categoryId", categoryId);

            var rows = await this._baseDapperRepository.QueryAsync<WorkOrderServiceGridViewModel>(query, parameters);
            result.Count = rows.FirstOrDefault()?.Count ?? 0;
            result.Payload = rows;

            return result;
        }

        public async Task<DataSource<WorkOrderServiceListBoxViewModel>> ReadAllCboDapperAsync (int companyId, int categoryId = -1, IEnumerable<int> categoryIds = null)
        {
            var result = new DataSource<WorkOrderServiceListBoxViewModel>()
            {
                Count = 0,
                Payload = new List<WorkOrderServiceListBoxViewModel>()
            };

            string where = "";

            if (categoryId > 0)
            {
                where += " AND S.[WorkOrderServiceCategoryId] = @categoryId";
            }

            if (categoryIds.Any())
            {
                where += " AND S.WorkOrderServiceCategoryId IN @categoryIds";
            }

            string query = $@"
            SELECT
	            S.[ID],
	            S.[Name],
                S.[UnitFactor],
                S.[Frequency],
                S.[Rate],
                S.[RequiresScheduling],
                S.[QuantityRequiredAtClose],
                S.[HoursRequiredAtClose]
            FROM [WorkOrderServiceS] AS S
	            INNER JOIN [WorkOrderServiceCategories] AS C ON C.[ID] = S.[WorkOrderServiceCategoryId]
            WHERE C.[CompanyId] = @companyId {where}";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@companyId", companyId);
            parameters.Add("@categoryId", categoryId);
            parameters.Add("@categoryIds", categoryIds);

            var rows = await this._baseDapperRepository.QueryAsync<WorkOrderServiceListBoxViewModel>(query, parameters);
            result.Count = rows.FirstOrDefault()?.Count ?? 0;
            result.Payload = rows;

            return result;
        }
    }
}
