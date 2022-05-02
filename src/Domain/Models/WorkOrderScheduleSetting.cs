using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class WorkOrderScheduleSetting : AuditableEntity
    {
        public WorkOrderScheduleFrequency Frequency { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Ordinal { get; set; }

        public int? StartValue { get; set; }

        public int? EndValue { get; set; }

        public IEnumerable<int> Days { get; set; }

        public DateTime? ScheduleDate { get; set; }
    }
}
