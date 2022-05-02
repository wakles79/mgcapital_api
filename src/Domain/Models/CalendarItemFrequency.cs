using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CalendarItemFrequency : AuditableCompanyEntity
    {
        /// <summary>
        /// The type of entity
        /// <para>0 => Work Order</para>
        /// </summary>
        public int ItemType { get; set; }

        public int Quantity { get; set; }

        public CalendarFrequency Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public IEnumerable<int> Months { get; set; }

        public CalendarItemFrequency()
        {
            this.Months = new HashSet<int>();
        }
    }
}
