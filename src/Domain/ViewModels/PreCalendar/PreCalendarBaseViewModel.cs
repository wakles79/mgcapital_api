using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Enums;

namespace MGCap.Domain.ViewModels.PreCalendar
{
    public class PreCalendarBaseViewModel : EntityViewModel
    {
        public int Quantity { get; set; }

        public CalendarPeriodicity Periodicity { get; set; }

        public CalendarEventType Type { get; set; }

        public DateTime? SnoozeDate { get; set; }

        public string Description { get; set; }

        public int? BuildingId { get; set; }

        public int? EmployeeId { get; set; }

    }
}
