using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.WorkOrderService
{
    public class WorkOrderServiceGridViewModel : WorkOrderServiceBaseViewModel, IGridViewModel
    {
        public string CategoryName { get; set; }

        public string FrequencyName => this.Frequency.ToString().SplitCamelCase();

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
