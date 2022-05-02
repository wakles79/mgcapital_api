using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CalendarItemFrequency
{
    public class CalendarItemFrequencyBaseViewModel : EntityViewModel
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
    }
}
