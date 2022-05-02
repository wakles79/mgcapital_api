using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.ExpenseType
{
    public class ExpenseTypeGridViewModel : ExpenseTypeBaseViewModel, IGridViewModel
    {
        public Guid Guid { get; set; }

        public string ExpenseCategoryName => this.ExpenseCategory.ToString().SplitCamelCase();

        public int Subcategories { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
