// -----------------------------------------------------------------------
// <copyright file="ProposalServicesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.ProposalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ProposalServicesRepository : BaseRepository<ProposalService, int>, IProposalServicesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProposalServicesRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public ProposalServicesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository
            )
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ProposalServiceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int proposalId)
        {
            var result = new DataSource<ProposalServiceGridViewModel>
            {
                Payload = new List<ProposalServiceGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
                        PS.ID,
                        PS.ProposalId,
                        PS.BuildingId,
                        (CASE WHEN B.ID IS NULL THEN PS.BuildingName ELSE B.Name END) AS BuildingName,
                        PS.OfficeServiceTypeId,
                        PS.Quantity,
                        PS.RequesterName,
                        PS.Description,
                        PS.Location,
                        PS.Rate,
                        PS.DateToDelivery
                    FROM [dbo].[ProposalServices] AS PS
                        LEFT JOIN Buildings B ON B.ID = PS.BuildingId
                    WHERE PS.ProposalId = @ProposalId
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@ProposalId", proposalId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);

            var payload = await this._baseDapperRepository.QueryAsync<ProposalServiceGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
