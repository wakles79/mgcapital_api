using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PreCalendar
{
    public class PreCalendarDetailViewModel : PreCalendarBaseViewModel
    {
        public IEnumerable<PreCalendarTaskGridViewModel> Tasks { get; set; }

        public PreCalendarDetailViewModel()
        {
            this.Tasks = new HashSet<PreCalendarTaskGridViewModel>();
        }
    }
}
