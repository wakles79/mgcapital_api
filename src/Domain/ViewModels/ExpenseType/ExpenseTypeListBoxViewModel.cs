using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Utils;

namespace MGCap.Domain.ViewModels.ExpenseType
{
    public class ExpenseTypeListBoxViewModel : ListBoxViewModel
    {
        public ExpenseCategory ExpenseCategory { get; set; }

        public string ExpenseCategoryName => this.ExpenseCategory.ToString().SplitCamelCase();
    }
}
