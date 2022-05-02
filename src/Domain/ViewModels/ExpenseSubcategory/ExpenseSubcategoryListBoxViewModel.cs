using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ExpenseSubcategory
{
    public class ExpenseSubcategoryListBoxViewModel : ListBoxViewModel
    {
        public double Rate { get; set; }

        public ExpenseRateType RateType { get;  set; }

        public string RateTypeName => this.RateType.ToString().SplitCamelCase();

        public string Periodicity { get; set; }
    }
}
