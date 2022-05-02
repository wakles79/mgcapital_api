using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Expense
{
    public class ExpenseBaseViewModel : EntityViewModel
    {
        public int? ContractId { get; set; }

        public int? BuildingId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime Date { get; set; }

        public string Vendor { get; set; }

        public ExpenseCategory Type { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        public string TransactionNumber { get; set; }
    }
}
