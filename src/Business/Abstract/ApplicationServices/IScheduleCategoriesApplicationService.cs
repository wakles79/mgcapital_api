using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ScheduleCategories;
using MGCap.Domain.ViewModels.ScheduleSubCategories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IScheduleCategoriesApplicationService : IBaseApplicationService<ScheduleSettingCategory, int>
    {
        Task<DataSource<ScheduleCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? isActive = -1);

        Task<DataSource<ScheduleCategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request);

        Task<ScheduleSettingCategory> UpdateStatusAsync(int id);

        Task<ScheduleSettingSubCategory> AddSubcategoryAsync(ScheduleSettingSubCategory scheduleSubcategory);

        Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllSubcategoriesCboDapperAsync(DataSourceRequestScheduleCategory request, int scheduleId, int? isEnabled);

        Task<ScheduleSettingSubCategory> UpdateSubcategoryAsync(ScheduleSettingSubCategory Subcategory);

        Task<ScheduleSettingSubCategory> GetSubcategoryAsync(int id);

        Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllMultipleSubcategoriesCbo(DataSourceRequestScheduleCategory request);
    }
}
