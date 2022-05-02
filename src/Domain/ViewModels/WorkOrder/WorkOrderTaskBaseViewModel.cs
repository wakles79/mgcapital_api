using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskBaseViewModel : AuditableEntityViewModel
    {
        public int WorkOrderId { get; set; }

        [Required]
        public bool IsComplete { get; set; }

        [Required]
        public string Description { get; set; }

        public int? ServiceId { get; set; }

        public double? UnitPrice { get; set; }

        public double? Quantity { get; set; }

        public double? DiscountPercentage { get; set; }

        public string Note { get; set; }

        public DateTime? LastCheckedDate { get; set; }

        public int EchoLastCheckedDate
        {
            get
            {
                return this.LastCheckedDate?.ToEpoch() ?? 0;
            }
        }

        [MaxLength(128)]
        public string Location { get; set; }

        public int? WorkOrderServiceCategoryId { get; set; }

        public int? WorkOrderServiceId { get; set; }

        public string UnitFactor { get; set; }

        public WorkOrderServiceFrequency Frequency { get; set; }

        public double Rate { get; set; }

        public double QuantityExecuted { get; set; }

        public double HoursExecuted { get; set; }

        public DateTime? CompletedDate { get; set; }

        public bool QuantityRequiredAtClose { get; set; }

        public string GeneralNote { get; set; }

    }
}
