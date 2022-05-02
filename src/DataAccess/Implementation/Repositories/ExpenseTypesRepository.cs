// -----------------------------------------------------------------------
// <copyright file="ExpenseTypesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.ExpenseType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ExpenseTypesRepository : BaseRepository<ExpenseType, int>, IExpenseTypesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public ExpenseTypesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ExpenseTypeListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<ExpenseTypeListBoxViewModel>
            {
                Payload = new List<ExpenseTypeListBoxViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
                        SELECT 
                            ET.Guid,
                            ET.ID,
                            ET.Description AS Name,
                            ET.ExpenseCategory
                        FROM [dbo].[ExpenseTypes] AS ET
                        WHERE ET.Status = 1
                        AND ET.CompanyId = @CompanyId
                    ) payload
                    ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);            
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<ExpenseTypeListBoxViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ExpenseTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1)
        {
            var result = new DataSource<ExpenseTypeGridViewModel>
            {
                Payload = new List<ExpenseTypeGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
                        SELECT 
                            ET.Guid,
                            ET.ID,
                            ET.Description,
                            ET.ExpenseCategory,                            
                            ET.Status,
                            (SELECT COUNT(ES.ID) FROM ExpenseSubcategories ES WHERE ES.ExpenseTypeId = ET.ID) as Subcategories
                        FROM [dbo].[ExpenseTypes] AS ET
                        WHERE ET.CompanyId = @CompanyId 
                        AND ET.[Status] = CASE WHEN ISNULL(@isActive, -1) = -1 THEN ET.[status] ELSE @isActive END
                        AND ISNULL(ET.Description,'') LIKE '%' + ISNULL(@Filter,'') +'%'
                    ) payload
                    ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@Filter", request.Filter);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@isActive", isActive);

            var payload = await _baseDapperRepository.QueryAsync<ExpenseTypeGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
