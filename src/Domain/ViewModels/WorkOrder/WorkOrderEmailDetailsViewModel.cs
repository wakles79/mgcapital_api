using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderEmailDetailsViewModel : EntityViewModel
    {
        public Guid Guid { get; set; }
        public int WONumber { get; set; }

        public WorkOrderStatus StatusId {get;set;}

        public string Description { get; set; }
        public string BuildingName { get; set; }
        public string AssignedFullName { get; set; }
        public string RequesterFullName { get; set; }
        public string RequesterEmail { get; set; }
        public string EmployeeWhoClosedWO { get; set; }
    }
}
