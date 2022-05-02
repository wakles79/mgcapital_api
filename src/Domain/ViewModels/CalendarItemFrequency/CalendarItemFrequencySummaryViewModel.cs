using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CalendarItemFrequency
{
    public class CalendarItemFrequencySummaryViewModel : CalendarItemFrequencyBaseViewModel
    {
        public IEnumerable<DateTime> AddedDates { get; set; }

        public CalendarItemFrequencySummaryViewModel()
        {
            this.AddedDates = new HashSet<DateTime>();
        }
    }
}
