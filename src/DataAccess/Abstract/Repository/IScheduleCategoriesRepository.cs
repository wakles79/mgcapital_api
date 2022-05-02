using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ScheduleCategories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IScheduleCategoriesRepository : IBaseRepository<ScheduleSettingCategory, int>
    {
        Task<DataSource<ScheduleCategoryGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1);
        Task<DataSource<ScheduleCategoryListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId); 
    }
}
