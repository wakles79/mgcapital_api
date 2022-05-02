using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.WorkOrderService
{
    public class WorkOrderServiceListBoxViewModel : ListBoxViewModel
    {
        public string UnitFactor { get; set; }

        public WorkOrderServiceFrequency Frequency { get; set; }

        public string FrequencyName => this.Frequency.ToString().SplitCamelCase();

        public double Rate { get; set; }

        public bool RequiresScheduling { get; set; }

        public bool QuantityRequiredAtClose { get; set; }

        public bool HoursRequiredAtClose { get; set; }
    }
}
