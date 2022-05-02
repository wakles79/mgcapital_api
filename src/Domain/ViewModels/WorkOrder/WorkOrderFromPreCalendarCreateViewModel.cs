using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderFromPreCalendarCreateViewModel : WorkOrderCreateViewModel
    {
        public CalendarPeriodicity Periodicity { get; set; }

        public int Quantity { get; set; }
    }
}
