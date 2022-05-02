using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractExpense
{
    public class ContractExpenseExportPdfViewModel
    {
        public string Description { get; set; }

        public double Quantity { get; set; }

        public string TypeName { get; set; }

        public double Rate { get; set; }

        public double Value { get; set; }

        public string RatePeriodicity { get; set; }

        public double OverheadPercent { get; set; }

        public double TaxesAndInsurance { get; set; }

        public ExpenseCategory ExpenseCategory { get; set; }

        public string ExpenseCategoryName => this.ExpenseCategory.ToString().SplitCamelCase();

        public double DailyRate { get; set; }

        public double MonthlyRate { get; set; }

        public double YearlyRate { get; set; }

        public ContractExpenseExportPdfViewModel(string description, double quantity, string typeName, double rate, double value, string ratePeriodicity, double overheadPercent, ExpenseCategory expenseCategory, double daysPerMonth)
        {
            Description = description;
            Quantity = quantity;
            TypeName = typeName;
            Rate = rate;
            Value = value;
            RatePeriodicity = ratePeriodicity;
            OverheadPercent = overheadPercent;
            ExpenseCategory = expenseCategory;

            this.TaxesAndInsurance = this.OverheadPercent == 0 ? 0 : (this.OverheadPercent / 100) * this.Rate;

            this.CalculateRatePeriods(daysPerMonth);
        }

        private void CalculateRatePeriods(double daysPerMonth)
        {
            if (this.ExpenseCategory == ExpenseCategory.Labor)
            {
                this.Rate = this.Rate + (this.Rate * (this.OverheadPercent / 100));
            }

            switch (this.RatePeriodicity)
            {
                case "Daily":
                    this.DailyRate = (this.Value * this.Rate) * this.Quantity;
                    this.MonthlyRate = this.DailyRate * daysPerMonth;
                    this.YearlyRate = this.MonthlyRate * 12;
                    break;
                case "Monthly":
                    this.MonthlyRate = (this.Value * this.Rate) * this.Quantity;
                    this.DailyRate = this.MonthlyRate / daysPerMonth;
                    this.YearlyRate = this.MonthlyRate * 12;
                    break;
                case "Yearly":
                    this.YearlyRate = (this.Value * this.Rate) * this.Quantity;
                    this.MonthlyRate = this.YearlyRate / 12;
                    this.DailyRate = this.MonthlyRate / daysPerMonth;
                    break;
                default:
                    break;
            }
        }
    }
}
