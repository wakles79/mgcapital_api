using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrdersByDueDateViewModel
    {
        public int EmployeeId { get; set; }

        public string UserEmail { get; set; }

        public int DueToday { get; set; }

        public int PastDue { get; set; }
    }
}
