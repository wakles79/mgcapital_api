using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ScheduleSubCategories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public  interface IScheduleSubCategoriesRepository : IBaseRepository<ScheduleSettingSubCategory, int>
    {
        Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequestScheduleCategory request, int expenseTypeId, int? isEnabled);

        Task<DataSource<ScheduleSubCategoryListBoxViewModel>> ReadAllMultipleCboDapperAsync(DataSourceRequestScheduleCategory request);
    }
}
