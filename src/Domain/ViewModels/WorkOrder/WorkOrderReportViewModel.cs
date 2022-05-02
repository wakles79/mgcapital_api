using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderReportViewModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public string BuildingName { get; set; }

        public DateTime WorkOrderCreatedDate { get; set; }
        public int EpochWorkOrderCreatedDate => this.WorkOrderCreatedDate.ToEpoch();
        public string WorkOrderCreatedDateText { get { return String.Format("{0:g}", this.WorkOrderCreatedDate); } }

        public DateTime WorkOrderCompletedDate { get; set; }
        public int EpochWorkOrderCompletedDate => this.WorkOrderCompletedDate.ToEpoch();
        public string WorkOrderCompletedDateText { get { return String.Format("{0:g}", this.WorkOrderCompletedDate); } }

        public IEnumerable<WorkOrderTaskDetailsViewModel> Tasks { get; set; }
    }
}
