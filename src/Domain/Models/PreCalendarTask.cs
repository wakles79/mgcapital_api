using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class PreCalendarTask : AuditableEntity
    {
        public int PreCalendarId { get; set; }

        [ForeignKey("PreCalendarId")]
        public PreCalendar PreCalendar { get; set; }

        public bool IsComplete { get; set; }

        public string Description { get; set; }

        public int? ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        public double UnitPrice { get; set; }

        public double Quantity { get; set; }

        public double DiscountPercentage { get; set; }

        public string Note { get; set; }

        /// <summary>
        ///     Determines the last time the task was marked
        ///     as 'completed'
        /// </summary>
        public DateTime LastCheckedDate { get; set; }
    }
}
