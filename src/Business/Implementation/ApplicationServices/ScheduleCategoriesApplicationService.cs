using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ScheduleCategories;
using MGCap.Domain.ViewModels.ScheduleSubCategories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class ScheduleCategoriesApplicationService : BaseSessionApplicationService<ScheduleSettingCategory, int>, IScheduleCategoriesApplicationService
    {
        public new IScheduleCategoriesRepository Repository => base.Repository as IScheduleCategoriesRepository;
        private readonly IScheduleSubCategoriesRepository ScheduleSubcategoriesRepository;

        public ScheduleCategoriesApplicationService(
            IScheduleCategoriesRepository repository,
            IScheduleSubCategoriesRepository scheduleSubcategoriesRepository,
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor)
        {
            this.ScheduleSubcategoriesRepository = scheduleSubcategoriesRepository;
        }

        public Task<DataSource<ScheduleCategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllCboDapperAsync(request, this.CompanyId);
        }

        public Task<DataSource<ScheduleCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isActive = -1)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId, isActive);
        }

        public async Task<ScheduleSettingCategory> UpdateStatusAsync(int id)
        {
            var objCategory = await this.Repository.SingleOrDefaultAsync(e => e.ID == id);
            objCategory.Status = !objCategory.Status;
            return await this.UpdateAsync(objCategory);
        }

        public Task<ScheduleSettingSubCategory> AddSubcategoryAsync(ScheduleSettingSubCategory scheduleSubcategory)
        {
            return this.ScheduleSubcategoriesRepository.AddAsync(scheduleSubcategory);
        }

        public Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllSubcategoriesCboDapperAsync(DataSourceRequestScheduleCategory request, int scheduleId, int? isEnabled)
        {
            return this.ScheduleSubcategoriesRepository.ReadAllCboDapperAsync(request, scheduleId, isEnabled);
        }

        public async Task<ScheduleSettingSubCategory> GetSubcategoryAsync(int id)
        {
            return await this.ScheduleSubcategoriesRepository.SingleOrDefaultAsync(e => e.ID == id);

        }

        public Task<ScheduleSettingSubCategory> UpdateSubcategoryAsync(ScheduleSettingSubCategory Subcategory)
        {
            return this.ScheduleSubcategoriesRepository.UpdateAsync(Subcategory);
        }

        public Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllMultipleSubcategoriesCbo(DataSourceRequestScheduleCategory request)
        {
            return this.ScheduleSubcategoriesRepository.ReadAllMultipleCboDapperAsync(request);
        }
    }
}
