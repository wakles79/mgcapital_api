// -----------------------------------------------------------------------
// <copyright file="TagRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Tag;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class TagRepository : BaseRepository<Tag, int>, ITagRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public TagRepository(
            MGCapDbContext context,
            IBaseDapperRepository baseDapperRepository
            ) : base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<TagGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<TagGridViewModel>()
            {
                Count = 0,
                Payload = new List<TagGridViewModel>()
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
	                    T.[ID],
	                    T.[CompanyId],
	                    T.[CreatedDate],
	                    T.[UpdatedDate],
	                    T.[Description],
	                    (SELECT COUNT (R.[ID]) FROM TicketTags AS R WHERE R.[TagId] = T.[ID] ) AS [ReferenceCount],
                        T.[HexColor]
                    FROM [Tags] AS [T]
                    WHERE T.[CompanyId] = @CompanyId
                    AND ((@Filter IS NOT NULL AND CONCAT(T.[Description], '') LIKE '%'+@Filter+'%') OR (@Filter IS NULL))
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY
                ";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@Filter", request.Filter);

            var payload = await this._baseDapperRepository.QueryAsync<TagGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<ListBoxViewModel>> ReadAllCboDapperAsync(int companyId)
        {
            string query = $@"
                    SELECT 
	                    T.[ID],
	                    T.[Description] AS [Name],
                        T.[HexColor]
                    FROM [Tags] AS [T]
                    WHERE T.[CompanyId] = @CompanyId
                    ORDER BY T.[Description] ASC
                ";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);

            var rows = await this._baseDapperRepository.QueryAsync<ListBoxViewModel>(query, pars);

            return rows.AsEnumerable();
        }

        public async Task<IEnumerable<TicketTagAssignmentViewModel>> ReadAllTicketTags(int ticketId)
        {
            string query = $@"
                SELECT 
	                T.[ID] AS [TagId],
	                R.[ID] AS [TicketTagId],
	                T.[Description],
                    T.[HexColor]
                FROM [Tags] AS T
	                INNER JOIN [TicketTags] AS R ON R.[TagId] = T.[ID]
                WHERE R.[TicketId] = @TicketId ORDER BY T.[Description] ASC";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TicketId", ticketId);

            var rows = await this._baseDapperRepository.QueryAsync<TicketTagAssignmentViewModel>(query, parameters);

            return rows.AsEnumerable();
        }
    }
}
