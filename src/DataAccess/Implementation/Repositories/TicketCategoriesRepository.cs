using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.TicketCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class TicketCategoriesRepository : BaseRepository<TicketCategory, int>, ITicketCategoriesRepository
    {

        private readonly IBaseDapperRepository _baseDapperRepository;

        public TicketCategoriesRepository(
            IBaseDapperRepository baseDapperRepository,
            MGCapDbContext context
            ) : base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<TicketCategoryGridViewModel>> ReadAllDapperAsync(int companyId, DataSourceRequest request)
        {
            DataSource<TicketCategoryGridViewModel> result = new DataSource<TicketCategoryGridViewModel>()
            {
                Count = 0,
                Payload = new List<TicketCategoryGridViewModel>()
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID DESC" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                   SELECT 
	                    C.[ID],
	                    C.[Name],
	                    C.[CreatedDate]
                    FROM [dbo].[TicketCategories] AS C
                    WHERE C.[CompanyId] = @CompanyId
	                    AND ((@Filter IS NOT NULL AND C.[Name] LIKE '%'+@Filter+'%' ) OR (@Filter IS NULL))
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Filter", string.IsNullOrEmpty(request.Filter) ? null : request.Filter);
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@PageNumber", request.PageNumber);
            parameters.Add("@PageSize", request.PageSize);

            var rows = await this._baseDapperRepository.QueryAsync<TicketCategoryGridViewModel>(query, parameters);
            result.Count = rows.FirstOrDefault()?.Count ?? 0;
            result.Payload = rows;

            return result;
        }
    }
}
