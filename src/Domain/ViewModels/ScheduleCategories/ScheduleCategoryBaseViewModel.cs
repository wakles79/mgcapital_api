using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ScheduleCategories
{
    public class ScheduleCategoryBaseViewModel : EntityViewModel
    {
        public ScheduleCategoryType ScheduleCategoryType { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }

        public string Name { get; set; }

        public int Color { get; set; }
    }
}
