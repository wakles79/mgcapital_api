// -----------------------------------------------------------------------
// <copyright file="ContractExpensesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.ContractExpense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    /// Contains the implementation of the functionalities
    /// for handling operations on the <see cref="ContractExpense"/>
    /// </summary>
    public class ContractExpensesRepository : BaseRepository<ContractExpense, int>, IContractExpensesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractExpensesRepository"/> class
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> implementation</param>
        /// <param name="baseDapperRepository"></param>
        public ContractExpensesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ContractExpenseGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int contractId)
        {
            var result = new DataSource<ContractExpenseGridViewModel>
            {
                Payload = new List<ContractExpenseGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT
                        CE.ID,
                        CE.Quantity,
                        CE.Description,
                        CE.ContractId,
                        CE.ExpenseSubcategoryId,
                        CE.ExpenseSubcategoryName,
                        CE.Rate,
                        CE.RateType,
                        CE.RatePeriodicity,
                        CE.Value,
                        CE.ExpenseCategory,
                        CE.OverheadPercent,
                        CASE WHEN ES.ID IS NOT NULL THEN ES.ExpenseTypeId ELSE 0 END AS ExpenseTypeId
                    FROM [dbo].[ContractExpenses] AS CE LEFT JOIN [dbo].[ExpenseSubcategories] AS ES ON ES.ID=CE.ExpenseSubcategoryId
                    WHERE CE.ContractId = @ContractId
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@ContractId", contractId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);

            var payload = await this._baseDapperRepository.QueryAsync<ContractExpenseGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
