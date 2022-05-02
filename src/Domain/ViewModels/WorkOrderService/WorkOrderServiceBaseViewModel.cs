using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.WorkOrderService
{
    public class WorkOrderServiceBaseViewModel : EntityViewModel
    {
        public int WorkOrderServiceCategoryId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string UnitFactor { get; set; }

        public WorkOrderServiceFrequency Frequency { get; set; }

        public double Rate { get; set; }

        public bool RequiresScheduling { get; set; }

        public bool QuantityRequiredAtClose { get; set; }

        public bool HoursRequiredAtClose { get; set; }
    }
}
