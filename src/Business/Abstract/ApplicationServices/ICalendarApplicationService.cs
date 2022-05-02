// -----------------------------------------------------------------------
// <copyright file="ICalendarApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.CalendarItemFrequency;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ICalendarApplicationService
    {
        Task<DataSource<WorkOrderCalendarGridViewModel>> ReadAllActivitiesDapperAsync(DataSourceRequestCalendar request);

        #region Calendar Item
        Task<CalendarItemFrequencySummaryViewModel> AddCalendarItemFrequencyAsync(CalendarItemFrequency calendarItemFrequency);
        #endregion

        Task<IEnumerable<WorkOrderTaskSummaryViewModel>> ReadAllWorkOrderTaskBySequenceId(int calendarItemFrequencyId);
    }
}
