// -----------------------------------------------------------------------
// <copyright file="ExpenseSubcategoriesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.ExpenseSubcategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    class ExpenseSubcategoriesRepository : BaseRepository<ExpenseSubcategory, int>, IExpenseSubcategoriesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public ExpenseSubcategoriesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ExpenseSubcategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int expenseTypeId, int? isEnabled)
        {
            var result = new DataSource<ExpenseSubcategoryListBoxViewModel>
            {
                Payload = new List<ExpenseSubcategoryListBoxViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
                        ES.ID,
                        ES.Name,
                        ES.Rate,
                        ES.RateType,
                        ES.Periodicity
                    FROM [dbo].[ExpenseSubcategories] ES
                    WHERE ES.ExpenseTypeId = @expenseTypeId
                    AND ES.[Status] = CASE WHEN ISNULL(@isEnabled, -1) = -1 THEN ES.[Status] ELSE @isEnabled END
                ) payload
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@expenseTypeId", expenseTypeId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@isEnabled", isEnabled);

            var payload = await _baseDapperRepository.QueryAsync<ExpenseSubcategoryListBoxViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<ExpenseSubcategory>> FindByName(int companyId, string name)
        {
            string query = $@"SELECT
	                            ES.ID,
	                            ES.Name,
                                ES.Rate,
                                ES.RateType,
                                ES.Periodicity
                            FROM [dbo].[ExpenseSubcategories] AS ES
	                            INNER JOIN [dbo].[ExpenseTypes] ET ON ET.ID = ES.ExpenseTypeId
                            WHERE ET.CompanyId = @CompanyId AND ES.Name LIKE '{name}'";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);

            var result = await _baseDapperRepository.QueryAsync<ExpenseSubcategory>(query, pars);

            return result;
        }
    }
}
