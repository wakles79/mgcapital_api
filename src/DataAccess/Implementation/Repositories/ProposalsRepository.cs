// -----------------------------------------------------------------------
// <copyright file="ProposalsRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Proposal;
using MGCap.Domain.ViewModels.ProposalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ProposalsRepository : BaseRepository<Proposal, int>, IProposalsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProposalsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public ProposalsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository
            )
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<ProposalReportDetailViewModel> GetProposalReportDetailsDapperAsync(int? proposalId, Guid? guid)
        {
            var result = new ProposalReportDetailViewModel();
            var pars = new DynamicParameters();
            string whereQuery = "";

            if (guid.HasValue)
            {
                whereQuery = " P.Guid = @ProposalGuid";
                pars.Add("@ProposalGuid", guid);
            }
            else
            {
                whereQuery = " P.ID = @ProposalId";
                pars.Add("@ProposalId", proposalId);
            }

            string proposalQuery = $@"SELECT 
                                        P.ID,
                                        P.Guid,
                                        P.CreatedDate,
                                        P.CustomerId,
                                        (CASE WHEN C.ID IS NULL THEN P.CustomerName ELSE C.Name END) AS CustomerName,
                                        P.CustomerEmail,
                                        P.ContactId,
                                        CONCAT(CO.FirstName, ' ', CO.LastName) AS ContactName,
                                        P.Location,
                                        P.Status,
                                        P.BillTo,
                                        P.BillToName,
                                        P.BillToEmail,
                                        P.StatusChangedDate
                                    FROM [dbo].[Proposals] AS P
                                        LEFT JOIN Customers AS C ON C.ID = P.CustomerId
                                        INNER JOIN Contacts AS CO ON CO.ID = P.ContactId
                                    WHERE { whereQuery }";

            var proposalData = await this._baseDapperRepository.QueryAsync<ProposalReportDetailViewModel>(proposalQuery, pars);
            result = proposalData.FirstOrDefault();

            if (guid.HasValue)
            {
                pars.Add("@ProposalId", result.ID);
            }

            string proposalServicesQuery = $@"
                                            SELECT 
                                                PS.ID,
                                                PS.ProposalId,
                                                PS.BuildingId,
                                                (CASE WHEN B.ID IS NULL THEN PS.BuildingName ELSE B.Name END) AS BuildingName,
                                                PS.OfficeServiceTypeId,
                                                OS.Name AS OfficeServiceTypeName,
                                                PS.Quantity,
                                                PS.RequesterName,
                                                PS.Description,
                                                PS.Location,
                                                PS.Rate,
                                                PS.DateToDelivery
                                            FROM [dbo].[ProposalServices] AS PS
                                                LEFT JOIN Buildings B ON B.ID = PS.BuildingId
                                                INNER JOIN OfficeServiceTypes OS ON OS.ID = PS.OfficeServiceTypeId
                                            WHERE PS.ProposalId = @ProposalId";

            var proposalServicesData = await this._baseDapperRepository.QueryAsync<ProposalServiceGridViewModel>(proposalServicesQuery, pars);
            if (proposalServicesData.Any())
                result.ProposalServices = proposalServicesData;

            return result;
        }

        public async Task<DataSource<ProposalGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? status = -1)
        {
            var result = new DataSource<ProposalGridViewModel>
            {
                Payload = new List<ProposalGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
                        P.ID,
                        P.Guid,
                        P.CreatedDate,
                        P.CustomerId,
                        (CASE WHEN C.ID IS NULL THEN P.CustomerName ELSE C.Name END) AS CustomerName,
                        P.ContactId,
                        CONCAT(CO.FirstName, ' ', CO.LastName) AS ContactName,
                        P.Location,
                        P.Status,
                        (SELECT COUNT(PS.ID) FROM [dbo].[ProposalServices] AS PS WHERE PS.ProposalId = P.ID) AS LineItems,
                        (SELECT SUM(PS.Rate * PS.Quantity) FROM [dbo].[ProposalServices] AS PS WHERE PS.ProposalId = P.ID) AS Value
                    FROM [dbo].[Proposals] AS P	
                        LEFT JOIN Customers AS C ON C.ID = P.CustomerId
                        INNER JOIN Contacts AS CO ON CO.ID = P.ContactId
                    WHERE P.CompanyId = @CompanyId
                        AND CONCAT(C.Name, CO.FirstName,CO.LastName,P.Location) LIKE '%' + ISNULL(@filter, '') + '%'
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@filter", request.Filter);

            var payload = await this._baseDapperRepository.QueryAsync<ProposalGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
