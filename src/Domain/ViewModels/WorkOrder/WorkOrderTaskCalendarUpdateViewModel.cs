using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskCalendarUpdateViewModel : EntityViewModel
    {
        public string Description { get; set; }

        public double? UnitPrice { get; set; }
    }
}
