using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractExpense
{
    public class ContractExpenseBaseViewModel : EntityViewModel
    {
        public int Quantity { get; set; }

        [MaxLength(128)]
        public string Description { get; set; }

        public int ContractId { get; set; }

        public int ExpenseCategory { get; set; }

        public string ExpenseTypeName { get; set; }

        public int ExpenseSubcategoryId { get; set; }

        public string ExpenseSubcategoryName { get; set; }

        public double Rate { get; set; }

        public int RateType { get; set; }

        [MaxLength(8)]
        public string RatePeriodicity { get; set; }

        public double Value { get; set; }

        public double OverheadPercent { get; set; }

        public ContractExpenseDefaultType DefaultType { get; set; }

        public double DailyRate { get; set; }

        public double MonthlyRate { get; set; }

        public double YearlyRate { get; set; }
    }
}
