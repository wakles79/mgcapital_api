using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Enums;
using System.Runtime.Serialization;


namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderDashboardViewModel
    {
        public int Quantity { get; set; }

        public int FooterValue { get; set; }

        public DashboardCriteria Criteria { get; set; }                
    }
}
