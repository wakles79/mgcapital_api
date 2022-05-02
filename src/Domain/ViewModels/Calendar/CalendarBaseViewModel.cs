using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Calendar
{
    public class CalendarBaseViewModel : EntityViewModel
    {
        public DateTime? SnoozeDate { get; set; }

        public int EpochSnoozeDate => this.SnoozeDate.HasValue ? this.SnoozeDate.Value.ToEpoch() : 0;

        public string Description { get; set; }

        public int Type { get; set; }
    }
}
