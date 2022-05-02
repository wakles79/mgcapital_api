using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionFromPreCalendarCreateViewModel : InspectionBaseViewModel
    {
        public CalendarPeriodicity Periodicity { get; set; }

        public int Quantity { get; set; }
    }
}
