using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ScheduleCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ScheduleCategoriesRepository : BaseRepository<ScheduleSettingCategory, int>, IScheduleCategoriesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public ScheduleCategoriesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ScheduleCategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<ScheduleCategoryListBoxViewModel>
            {
                Payload = new List<ScheduleCategoryListBoxViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
                              SELECT TOP (1000) [ID]
                              ,[CreatedDate]
                              ,[CreatedBy]
                              ,[UpdatedDate]
                              ,[UpdatedBy]
                              ,[CompanyId]
                              ,[Guid]
                              ,[ScheduleCategoryType]
                              ,[Status]
                              ,[Name]
                              ,[Description]
                          FROM [dbo].[ScheduleSettingCategories]
                          WHERE Status = 1
                          AND CompanyId = @CompanyId
                    ) payload
                    ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<ScheduleCategoryListBoxViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ScheduleCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1)
        {
            var result = new DataSource<ScheduleCategoryGridViewModel>
            {
                Payload = new List<ScheduleCategoryGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
                               SELECT [ID]
                              ,SC.[CreatedDate]
                              ,SC.[CreatedBy]
                              ,SC.[UpdatedDate]
                              ,SC.[UpdatedBy]
                              ,SC.[CompanyId]
                              ,SC.[Guid]
                              ,SC.[ScheduleCategoryType]
                              ,SC.[Status]
                              ,SC.[Name]
                              ,SC.[Description],
							  (select count(*) From [dbo].[ScheduleSettingSubCategories] where ScheduleSettingCategoryId = SC.id) as Subcategories
                          FROM [dbo].[ScheduleSettingCategories] as SC
                          WHERE SC.CompanyId = @CompanyId
                            AND ISNULL(SC.Description,'') LIKE '%' + ISNULL(@Filter,'') +'%'
                    ) payload
                    ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@Filter", request.Filter);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@isActive", isActive);

            var payload = await _baseDapperRepository.QueryAsync<ScheduleCategoryGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
