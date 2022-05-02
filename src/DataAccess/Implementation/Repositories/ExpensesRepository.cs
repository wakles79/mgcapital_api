// -----------------------------------------------------------------------
// <copyright file="ExpenseRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ExpensesRepository : BaseRepository<Expense, int>, IExpensesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public ExpensesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ExpenseGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request,int companyId, int budgetId, int month, int year)
        {
            var result = new DataSource<ExpenseGridViewModel>
            {
                Payload = new List<ExpenseGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID DESC" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                        SELECT
                            E.[ID],
                            (CASE WHEN CS.[BuildingId] IS NULL THEN 0 ELSE 1 END) AS IsDirect,
                            CS.[BuildingId],
                            CS.[CustomerId],
                            E.[Type],
                            E.[Amount],
                            E.[Date],
                            E.[Vendor],
                            E.[Description],
                            E.[Reference],
                            B.[Name] AS BuildingName,
                            C.[Name] AS CustomerName
                        FROM [dbo].[Expenses] AS E
                            LEFT JOIN Contracts CS ON CS.ID = E.ContractId
                            LEFT JOIN Buildings B ON B.ID = CS.BuildingId
                            LEFT JOIN Customers C ON C.ID = CS.CustomerId
                        WHERE E.ContractId = @BudgetId 
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@BudgetId", budgetId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@CompanyId", companyId);

            var payload = await _baseDapperRepository.QueryAsync<ExpenseGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ExpenseGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? buildingId = null, int? customerId = null, int? contractId = null)
        {
            var result = new DataSource<ExpenseGridViewModel>
            {
                Payload = new List<ExpenseGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID DESC" : $"{request.SortField} {request.SortOrder.ToUpper()}";
            string whereStr = string.Empty;

            if (buildingId.HasValue)
            {
                whereStr += $" AND CON.BuildingId = {buildingId.Value}";
            }

            if (customerId.HasValue)
            {
                whereStr += $" AND CON.CustomerId = {customerId.Value}";
            }

            if (contractId.HasValue)
            {
                whereStr += $" AND E.ContractId = {contractId.Value}";
            }

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST(E.[Date] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST(E.[Date] AS DATE) <= @dateTo ";
            }

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                                               SELECT
                            E.[ID],
                            E.ContractId, 
						    B.[ID]as BuildingId,				   
						    CU.[ID] as CustomerId,
                            E.[Type],
                            E.[Amount],
                            E.[Date],
                            E.[Vendor],
                            E.[Description],
                            E.[Reference],
                            E.[TransactionNumber],
                            B.[Name] AS BuildingName,
                            CU.[Name] AS CustomerName, 
						    (SELECT ContractNumber from  Contracts as CON WHERE CON.ID = E.[ContractId]) as ContractNumber
                        FROM [dbo].[Expenses] AS E 
					  INNER JOIN Contracts CON ON E.ContractId = CON.ID
                      INNER JOIN Buildings B ON  B.ID = CON.BuildingId 
					  INNER JOIN Customers CU ON CU.ID = CON.CustomerId	
                        WHERE E.CompanyId = @CompanyId {whereStr}
                            AND CONCAT(E.[Description], E.[Reference], B.[Name], CU.[Name]) LIKE '%' + ISNULL(@filter, '') + '%'
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@filter", request.Filter);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);

            var payload = await _baseDapperRepository.QueryAsync<ExpenseGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ExpenseGridViewModel>> GetRepeated(int ContractId, string reference, double amount, int companyId)
        {
            var result = new DataSource<ExpenseGridViewModel>
            {
                Payload = new List<ExpenseGridViewModel>(),
                Count = 0
            };

            string query = $@"
                         SELECT
                            E.[ID],
                            E.[Type],
                            E.[Amount],
                            E.[Date],
                            E.[Vendor],
                            E.[Description],
                            E.[Reference]
                        FROM [dbo].[Expenses] AS E
                      where
                            E.ContractId = @ContractId AND
					        E.Reference = @Reference AND
					        E.Amount = @Amount  AND
                            E.CompanyId = @CompanyId";

            var pars = new DynamicParameters();
            pars.Add("@ContractId", ContractId);
            pars.Add("@Reference", reference);
            pars.Add("@Amount", amount);
            pars.Add("@CompanyId", companyId);

            var payload = await _baseDapperRepository.QueryAsync<ExpenseGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
