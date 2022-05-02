using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrderTask
{
    public class WorkOrderTaskGridViewModel : EntityViewModel
    {
        public string Location { get; set; }

        public string Description { get; set; }

        public double Quantity { get; set; }

        public double Rate { get; set; }

        public string UnitFactor { get; set; }

        public int ServiceId { get; set; }

        public int WorkOrderServiceId { get; set; }

        public string ServiceName { get; set; }

        public string CategoryName { get; set; }

        public bool IsComplete { get; set; }

        public double Total => this.Quantity * this.Rate;

        public bool OldVersion { get; set; }

        public bool RequiresScheduling { get; set; }

        public bool QuantityRequiredAtClose { get; set; }

        public bool HoursRequiredAtClose { get; set; }

        public double QuantityExecuted { get; set; }

        public double HoursExecuted { get; set; }

        public WorkOrderServiceFrequency Frequency { get; set; }

        public DateTime? CompletedDate { get; set; }

        public int EpochCompletedDate => this.CompletedDate.HasValue ? this.CompletedDate.Value.ToEpoch() : 0;

        public string FrequencyName => this.WorkOrderServiceId > 0 ? this.Frequency.ToString().SplitCamelCase() : "";
    }
}
