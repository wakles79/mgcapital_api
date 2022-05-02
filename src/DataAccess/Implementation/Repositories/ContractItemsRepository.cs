// -----------------------------------------------------------------------
// <copyright file="ContractItemsRepository.cs" company="Axzes">
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
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    /// Contains the implementation of the functionalities
    /// for handling operations on the <see cref="ContractItem"/>
    /// </summary>
    class ContractItemsRepository : BaseRepository<ContractItem, int>, IContractItemsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractItemsRepository"/> class
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> implementation</param>
        /// <param name="baseDapperRepository"></param>
        public ContractItemsRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ContractItemGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int contractId)
        {
            var result = new DataSource<ContractItemGridViewModel>
            {
                Payload = new List<ContractItemGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
                        CI.ID,
                        CI.Quantity,
                        CI.Description,
                        CI.ContractId,
                        CI.OfficeServiceTypeId,
                        CI.OfficeServiceTypeName,
                        CI.Rate,
                        CI.RateType,
                        CI.RatePeriodicity,
                        CI.Hours,
                        CI.Amount,
                        CI.Rooms,
                        CI.SquareFeet
                    FROM [dbo].[ContractItems] AS CI
                    WHERE CI.ContractId = @ContractId
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@ContractId", contractId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);

            var payload = await this._baseDapperRepository.QueryAsync<ContractItemGridViewModel>(query,pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<ContractOfficeSpaceGridViewModel>> ReadAllSquareFeetByTypeDapperAsync(int contractId)
        {
            string query = $@"
                SELECT 
	                CI.[ContractId],
	                CI.[OfficeServiceTypeId],
	                SUM(CI.[SquareFeet]) AS [SquareFeet],
	                CI.[OfficeServiceTypeName] AS [OfficeTypeName]
                FROM [dbo].[ContractItems] AS CI 
                WHERE ContractId = @contractId 
                    AND CI.[RateType] = {(int)ServiceRateType.SquareFeet}
                GROUP BY [ContractId],[OfficeServiceTypeId],[OfficeServiceTypeName]";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@contractId",contractId);

            var result = await this._baseDapperRepository.QueryAsync<ContractOfficeSpaceGridViewModel>(query, pars);

            return result;
        }
    }
}
