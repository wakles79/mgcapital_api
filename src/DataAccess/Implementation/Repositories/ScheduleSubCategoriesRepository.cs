using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ScheduleSubCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ScheduleSubCategoriesRepository : BaseRepository<ScheduleSettingSubCategory, int>, IScheduleSubCategoriesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public ScheduleSubCategoriesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequestScheduleCategory request, int scheduleId, int? isEnabled)
        {
            var result = new DataSource<ScheduleSubCategoryListBoxViewModel>
            {
                Payload = new List<ScheduleSubCategoryListBoxViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
                        [ID]
                      ,[CreatedDate]
                      ,[CreatedBy]
                      ,[UpdatedDate]
                      ,[UpdatedBy]
                      ,[Name]
                      ,[ScheduleSettingCategoryId]
                    FROM [dbo].[ScheduleSettingSubCategories]
                    WHERE [ScheduleSettingCategoryId] = @scheduleId
                ) payload
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@scheduleId", scheduleId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@isEnabled", isEnabled);

            var payload = await _baseDapperRepository.QueryAsync<ScheduleSubCategoryListBoxViewModel>(query, pars);
            result.Count = payload.Count();
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllMultipleCboDapperAsync(DataSourceRequestScheduleCategory request)
        {
            var result = new DataSource<ScheduleSubCategoryListBoxViewModel>
            {
                Payload = new List<ScheduleSubCategoryListBoxViewModel>(),
                Count = 0
            };
            string where = string.Empty;
            DynamicParameters parameters = new DynamicParameters();

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            if (request.ScheduleCategory != null)
            {
                where += $" WHERE SSUB.[ScheduleSettingCategoryId] in @scheduleCategory";
                parameters.Add("@scheduleCategory", request.ScheduleCategory.ToList());
            }

             
            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
                       SSUB.[ID]
                      ,SSUB.[CreatedDate]
                      ,SSUB.[CreatedBy]
                      ,SSUB.[UpdatedDate]
                      ,SSUB.[UpdatedBy]
					  ,(select description from ScheduleSettingCategories where ID =SSUB.ScheduleSettingCategoryId) AS categoryName
                      ,SSUB.[Name]
                      ,SSUB.[ScheduleSettingCategoryId] 
                    FROM [dbo].[ScheduleSettingSubCategories] as SSUB             
                    {where}
                ) payload
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@ScheduleCategory", request.ScheduleCategory.ToList());

            var payload = await _baseDapperRepository.QueryAsync<ScheduleSubCategoryListBoxViewModel>(query, pars);
            result.Count = payload.Count();
            result.Payload = payload;

            return result;
        }
    }
}
