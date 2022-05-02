// -----------------------------------------------------------------------
// <copyright file="OfficeServiceTypesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.OfficeServiceType;
using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="OfficeServiceType"/>
    /// </summary>
    public class OfficeServiceTypesRepository : BaseRepository<OfficeServiceType, int>, IOfficeServiceTypesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public OfficeServiceTypesRepository(
                 MGCapDbContext dbContext,
                 IBaseDapperRepository baseDapperRepository)
                : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<OfficeServiceTypeListViewModel>> ReadAllCboDapperAsync(int companyId, int status = -1, int rateType = -1, string exclude = "")
        {
            var result = new DataSource<OfficeServiceTypeListViewModel>
            {
                Payload = new List<OfficeServiceTypeListViewModel>(),
                Count = 0
            };

            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(exclude))
            {
                where = $" AND OT.ID NOT IN ({exclude})";
            }

            string query = $@"
						-- payload query
						SELECT *, [Count] = COUNT(*) OVER() FROM (
							SELECT 
								OT.ID,
								OT.Name
							FROM [dbo].[OfficeServiceTypes] AS OT
							WHERE OT.CompanyId = @CompanyId 
                            AND OT.[status] = CASE WHEN ISNULL(@status, -1) = -1 THEN OT.[status] ELSE @status END
							AND OT.[RateType] = CASE WHEN ISNULL(@rateType, -1) = -1 THEN OT.[RateType] ELSE @rateType END	{where}
						) payload
						 ORDER BY ID DESC";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@status", status);
            pars.Add("@rateType", rateType);

            var payload = await _baseDapperRepository.QueryAsync<OfficeServiceTypeListViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<OfficeServiceTypeGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isEnabled = -1)
        {
            var result = new DataSource<OfficeServiceTypeGridViewModel>
            {
                Payload = new List<OfficeServiceTypeGridViewModel>(),
                Count = 0
            };
            
            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";


            string query = $@"
						-- payload query
						SELECT *, [Count] = COUNT(*) OVER() FROM (
							SELECT 
                                OT.Guid,
								OT.ID,
								OT.Name,
								OT.Rate,
                                OT.RateType,
                                OT.Periodicity,
                                OT.Status,
                                OT.SupplyFactor,
								CASE 
									WHEN (SELECT COUNT(CI.ID) FROM ContractItems AS CI WHERE CI.OfficeServiceTypeId = OT.ID) > 0 THEN 1
									WHEN (SELECT COUNT(CO.ID) FROM ContractOfficeSpaces AS CO WHERE CO.OfficeTypeId = OT.ID) > 0 THEN 1
									WHEN (SELECT COUNT(PS.ID) FROM ProposalServices AS PS WHERE PS.OfficeServiceTypeId = OT.ID) > 0 THEN 1
									ELSE 0
								END
								AS IsUsed
							FROM [dbo].[OfficeServiceTypes] AS OT
							WHERE OT.CompanyId = @CompanyId 
                            AND OT.[status] = CASE WHEN ISNULL(@isEnabled, -1) = -1 THEN OT.[status] ELSE @isEnabled END
								AND ISNULL(OT.Name,'') LIKE '%' + ISNULL(@Filter,'') +'%'
						) payload
						 ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@Filter", request.Filter);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@isEnabled", isEnabled);

            var payload = await _baseDapperRepository.QueryAsync<OfficeServiceTypeGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            if (!string.IsNullOrEmpty(request.SortField))
            {
                if (request.SortField.ToLower() == "ratetype") {
                    result.Payload = request.SortOrder.ToLower() == "asc" ? result.Payload.OrderBy(t => t.RateTypeName) : result.Payload.OrderByDescending(t => t.RateTypeName);
                }
            }

            return result;
        }

        public async Task<IEnumerable<OfficeServiceType>> FindByName(int companyId, string name)
        {
            string query = $@"
                SELECT
	                OT.[ID],
	                OT.[Name],
	                OT.[Rate],
	                OT.[RateType],
	                OT.[Periodicity],
	                OT.[Status],
	                OT.[SupplyFactor]
                FROM [dbo].[OfficeServiceTypes] AS OT
                WHERE OT.[CompanyId] = @CompanyId AND OT.[Name] LIKE '{name}'";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);

            var result = await this._baseDapperRepository.QueryAsync<OfficeServiceType>(query, pars);

            return result;
        }
    }
}