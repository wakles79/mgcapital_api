using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MGCap.Domain.Utils;

namespace MGCap.Domain.ViewModels.PreCalendar
{
    public class PreCalendarTaskBaseViewModel : AuditableEntityViewModel
    {
        public int PreCalendarId { get; set; }

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
    }
}
