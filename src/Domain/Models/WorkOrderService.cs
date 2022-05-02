using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class WorkOrderService : AuditableEntity
    {
        public int WorkOrderServiceCategoryId { get; set; }

        [ForeignKey("WorkOrderServiceCategoryId")]

        public WorkOrderServiceCategory WorkOrderServiceCategory { get; set; }

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
