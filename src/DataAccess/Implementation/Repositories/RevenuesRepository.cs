using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Revenue;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class RevenuesRepository : BaseRepository<Revenue, int>, IRevenuesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public RevenuesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
         ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<RevenueGridViewModel>> ReadAllByBudgetIdDapperAsync(DataSourceRequest request,int companyId, int budgetId, int month, int year)
        {
            var result = new DataSource<RevenueGridViewModel>
            {
                Payload = new List<RevenueGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";
            
            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                      SELECT 
                            [RE].[ID],
						    [RE].[CreatedDate],
						    [RE].[CreatedBy],
						    [RE].[UpdatedDate],
						    [RE].[UpdatedBy],
						    [RE].[CompanyId],
						    [RE].[Guid],
						    [C].[BuildingId],					  
						    (SELECT BU.Name FROM Buildings as BU WHERE BU.ID = C.[BuildingId]) as BuildingName,
						    [C].[CustomerId],
						    (SELECT CU.Name FROM Customers as CU WHERE cU.ID = C.[CustomerId]) as CustomerName,
                            [RE].[ContractId],
						    (SELECT ContractNumber from  Contracts as CON WHERE CON.ID = RE.[ContractId]) as ContractNumber,
						    [RE].[Date],
						    [RE].[SubTotal],
						    [RE].[Tax],
						    [RE].[Total],
						    [RE].[Description],
						    [RE].[Reference]
					  FROM [Revenues] as RE	
                        LEFT JOIN [Contracts] C ON C.ID = RE.ID
                    WHERE RE.ContractId = @BudgetId AND  RE.CompanyId=@CompanyId
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@BudgetId", budgetId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@CompanyId", companyId);

            var payload = await this._baseDapperRepository.QueryAsync<RevenueGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<RevenueGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? status = -1, int? buildingId = null, int? customerId = null, int? contractId = null)
        {
            var result = new DataSource<RevenueGridViewModel>
            {
                Payload = new List<RevenueGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";
            string whereStr = string.Empty;

            if (buildingId.HasValue)
            {
                whereStr += $" AND CON.BuildingId = {buildingId.Value}";
            }

            if (customerId.HasValue)
            {
                whereStr += $" AND CON.customerId = {customerId.Value}";
            }

            if (contractId.HasValue)
            {
                whereStr += $" AND RE.contractId = {contractId.Value}";
            }

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST(RE.[Date] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST(RE.[Date] AS DATE) <= @dateTo ";
            }

            string query = $@"
   -- payload query 
                SELECT *, [Count] = COUNT(*) OVER() FROM (                 
                   SELECT RE.[ID] 
						  ,RE.[CreatedDate] 
						  ,RE.[CreatedBy] 
						  ,RE.[UpdatedDate] 
						  ,RE.[UpdatedBy] 
						  ,RE.[CompanyId] 
						  ,RE.[Guid] 
                          ,RE.[ContractId] 
						  ,B.[ID]as BuildingId				   
						  ,CU.[ID] as CustomerId 
						  ,RE.[Date] 
						  ,RE.[SubTotal] 
						  ,RE.[Tax] 
						  ,RE.[Total] 
						  ,RE.[Description] 
						  ,RE.[Reference] 
                          ,RE.[TransactionNumber] 
						  ,B.[Name] AS BuildingName
                          ,CU.[Name] AS CustomerName 
						  ,(SELECT ContractNumber from  Contracts as CON WHERE CON.ID = RE.[ContractId]) as ContractNumber 
					  FROM [Revenues] as RE	 
					  INNER JOIN Contracts CON ON RE.ContractId = CON.ID
                      INNER JOIN Buildings B ON  B.ID = CON.BuildingId 
					  INNER JOIN Customers CU ON CU.ID = CON.CustomerId		   
                    WHERE RE.CompanyId = @CompanyId  
                    AND CONCAT(RE.Description, RE.Reference, RE.Total, RE.SubTotal, RE.Tax, B.[Name], CU.[Name]) LIKE '%' + ISNULL(@filter, '') + '%' {whereStr} 
                ) payload  
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@filter", request.Filter);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);

            var payload = await this._baseDapperRepository.QueryAsync<RevenueGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<RevenueGridViewModel>> GetRepeated(int contractId, string reference, double total, int companyId)
        {
            var result = new DataSource<RevenueGridViewModel>
            {
                Payload = new List<RevenueGridViewModel>(),
                Count = 0
            };

            string query = $@"
                      SELECT 
                            [RE].[ID],
						    [RE].[CreatedDate],
						    [RE].[CreatedBy],
						    [RE].[UpdatedDate],
						    [RE].[UpdatedBy],
						    [RE].[CompanyId],
						    [RE].[Guid],
                            [RE].[ContractId],
						    (SELECT ContractNumber from  Contracts as CON WHERE CON.ID = RE.[ContractId]) as ContractNumber,
						    [RE].[Date],
						    [RE].[SubTotal],
						    [RE].[Tax],
						    [RE].[Total],
						    [RE].[Description],
						    [RE].[Reference]
					  FROM [Revenues] as RE		
                      where
                            RE.ContractId = @ContractId AND
					        RE.Reference = @reference AND
					        RE.Total = @Total AND
                            RE.CompanyId = @CompanyId";

            var pars = new DynamicParameters();
            pars.Add("@ContractId", contractId);
            pars.Add("@Reference", reference);
            pars.Add("@Total", total);
            pars.Add("@CompanyId", companyId);
                
            var payload = await this._baseDapperRepository.QueryAsync<RevenueGridViewModel>(query, pars);
            result.Count = payload.Count();
            result.Payload = payload;

            return result;
        }

    }
}
