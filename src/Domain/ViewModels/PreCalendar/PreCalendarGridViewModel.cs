using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.PreCalendar
{
    public class PreCalendarGridViewModel : PreCalendarBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }

        public string BuildingName { get; set; }

        public string EmployeeName { get; set; }

        public string TypeName => this.Type.ToString().SplitCamelCase();

        public string PeriodicityName => this.Periodicity.ToString().SplitCamelCase();

        public string Status { get; set; }
    }
}
