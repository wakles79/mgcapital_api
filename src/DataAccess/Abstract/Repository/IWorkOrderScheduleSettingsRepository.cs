using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IWorkOrderScheduleSettingsRepository : IBaseRepository<WorkOrderScheduleSetting, int>
    {
        Task<IEnumerable<string>> ReadAllWorkOrderNumberFromSequence(int workOrderScheduleSettingId);
    }
}
