using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PreCalendar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IPreCalendarApplicationService : IBaseApplicationService<PreCalendar, int>
    {
        Task<DataSource<PreCalendarGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? buildingId = null);

        #region Tasks
        Task RemoveTasksAsync(int objId);

        Task<PreCalendarDetailViewModel> SingleOrDefaultDapperAsync(int iD);

        #endregion

    }
}
