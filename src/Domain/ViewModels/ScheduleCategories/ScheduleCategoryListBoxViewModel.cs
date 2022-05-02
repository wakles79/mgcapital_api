using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ScheduleCategories
{
    public class ScheduleCategoryListBoxViewModel :  ListBoxViewModel 
    {

        public string Description { get; set; }

        public ScheduleCategoryType ScheduleCategoryType { get; set; }


        public string ScheduleCategoryTypeName => this.ScheduleCategoryType.ToString().SplitCamelCase();
    }
}
