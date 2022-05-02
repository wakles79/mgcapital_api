using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ScheduleSubCategories
{
    public class ScheduleSubCategoryListBoxViewModel : ListBoxViewModel
    {
        public ScheduleCategoryType ScheduleCategoryType { get; set; }

        public string ScheduleCategoryTypeName => this.ScheduleCategoryType.ToString().SplitCamelCase();

        public string CategoryName { get; set; }
    }
}
