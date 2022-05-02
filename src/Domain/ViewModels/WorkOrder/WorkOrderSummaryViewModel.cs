using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderSummaryViewModel
    {
        public int PastDueTotal { get; set; } = 0;
        public int DueTodayTotal { get; set; } = 0;
    }
}
