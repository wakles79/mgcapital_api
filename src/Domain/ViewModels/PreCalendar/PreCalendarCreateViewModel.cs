using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PreCalendar
{
    public class PreCalendarCreateViewModel : PreCalendarBaseViewModel
    {
        public virtual IList<PreCalendarTaskCreateViewModel> Tasks { get; set; }
    }
}
