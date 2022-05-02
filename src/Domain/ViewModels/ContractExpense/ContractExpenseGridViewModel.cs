using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractExpense
{
    public class ContractExpenseGridViewModel : ContractExpenseBaseViewModel
    {
        public int ExpenseTypeId { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
