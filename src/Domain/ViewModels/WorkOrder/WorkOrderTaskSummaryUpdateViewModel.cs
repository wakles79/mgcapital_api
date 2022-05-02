using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskSummaryUpdateViewModel : EntityViewModel
    {
        public bool ClientApproved { get; set; }

        public bool ScheduleDateConfirmed { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public IEnumerable<WorkOrderTaskCalendarUpdateViewModel> Tasks { get; set; }
    }
}
