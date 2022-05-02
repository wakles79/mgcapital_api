using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WOCustomerGridViewModel : EntityViewModel, IGridViewModel
    {
        public Guid Guid { get; set; }

        public int Number { get; set; }

        public string BuildingName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public WorkOrderStatus StatusId { get; set; }

        public int Count { get; set; }
    }
}
