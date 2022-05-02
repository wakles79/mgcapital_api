// -----------------------------------------------------------------------
// <copyright file="ContractOfficeSpacesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.ContractItem;
using MGCap.Domain.ViewModels.ContractOfficeSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ContractOfficeSpacesRepository : BaseRepository<ContractOfficeSpace, int>, IContractOfficeSpacesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public ContractOfficeSpacesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ContractOfficeSpaceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int contractId)
        {
            var result = new DataSource<ContractOfficeSpaceGridViewModel>
            {
                Payload = new List<ContractOfficeSpaceGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"-- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
	                    CO.[ID],
	                    CO.[ContractId],
	                    CO.[OfficeTypeId],
	                    CO.[SquareFeet],
                        OT.[Name] AS OfficeTypeName
                    FROM [dbo].[ContractOfficeSpaces] AS CO
                        LEFT JOIN OfficeServiceTypes OT ON OT.ID = CO.OfficeTypeId
                    WHERE CO.ContractId = @ContractId
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@ContractId", contractId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);

            var payload = await this._baseDapperRepository.QueryAsync<ContractOfficeSpaceGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;


            return result;
        }
    }
}
