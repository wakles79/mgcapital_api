using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PreCalendar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IPreCalendarRepository : IBaseRepository<PreCalendar, int>
    {
        Task<DataSource<PreCalendarGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? buildingId = null);

        Task<PreCalendarDetailViewModel> SingleOrDefaultDapperAsync(int id);
    }
}
