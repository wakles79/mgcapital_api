using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderContactViewModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Type { get; set; }

        public bool SendNotifications { get; set; } 

        public WorkOrderContactType WorkOrderContactType => new WorkOrderContactType(this.Type);
    }
    
    public class WorkOrderEmployeeContactViewModel : WorkOrderContactViewModel
    {
        public int EmployeeId { get; set; }
    }
}
