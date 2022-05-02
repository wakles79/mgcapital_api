using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrderTask
{
    public class WorkOrderTaskUpdateCompletedStatusViewModel
    {
        public int Id { get; set; }

        public bool IsComplete { get; set; }

        public double QuantityExecuted { get; set; }

        public double HoursExecuted { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}
