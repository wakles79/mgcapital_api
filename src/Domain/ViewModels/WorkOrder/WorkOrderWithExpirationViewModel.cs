using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderWithExpirationViewModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public int StatusId { get; set; }

        public DateTime? DueDate { get; set; }

        public int EpochDueDate => this.DueDate.HasValue ? this.DueDate.Value.ToEpoch() : 0;

        public int IsExpired { get; set; }

        public int DueToday { get; set; }

        public int Number { get; set; }

        public string ClonePath { get; set; }

        public int BuildingId { get; set; }

        public string BuildingName { get; set; }

        public HashSet<int> EmployeeIds { get; set; }

        public WorkOrderWithExpirationViewModel()
        {
            this.EmployeeIds = new HashSet<int>();
        }
    }
}
