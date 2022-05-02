using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Enums;

namespace MGCap.Domain.ViewModels.ContractExpense
{
    public class ContractExpenseCsvViewModel
    {
        public string ContractNumber { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Expense Category 
        /// </summary>
        public string ExpenseType { get; set; }

        public string ExpenseSubcategory { get;set;}

        public double Rate { get; set; }

        public int RateType { get; set; }

        public string RatePeriodicity { get; set; }

        public double Value { get; set; }

        public double TaxPercent { get; set; }
    }
}
