using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Calendar
{
    public class CalendarGridViewModel : CalendarBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
