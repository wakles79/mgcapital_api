using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemExportPdfViewModel
    {
        public string Description { get; set; }

        public double Quantity { get; set; }

        public string TypeName { get; set; }

        public double Rate { get; set; }

        public double Value { get; set; }

        public string RatePeriodicity { get; set; }

        public double DailyRate { get; set; }

        public double MonthlyRate { get; set; }

        public double BiMonthlyRate { get; set; }

        public double QuarterlyRate { get; set; }

        public double BiAnnuallyRate { get; set; }

        public double YearlyRate { get; set; }

        public ContractItemExportPdfViewModel(string description, double quantity, string typeName, double rate, double value, string ratePeriodicity, double daysPerMonth)
        {
            Description = description;
            Quantity = quantity;
            TypeName = typeName;
            Rate = rate;
            Value = value;
            RatePeriodicity = ratePeriodicity;

            this.CalculateRatePeriods(daysPerMonth);
        }

        private void CalculateRatePeriods(double daysPerMonth)
        {
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
                case "Bi-Monthly":
                    this.BiMonthlyRate = (this.Value * this.Rate) * this.Quantity;
                    this.MonthlyRate = this.BiMonthlyRate / 2;
                    this.QuarterlyRate = this.MonthlyRate * 3;
                    this.BiAnnuallyRate = this.MonthlyRate * 6;
                    this.DailyRate = this.MonthlyRate / daysPerMonth;
                    this.YearlyRate = this.MonthlyRate * 12;
                    break;
                case "QuarterlyRate":
                    this.QuarterlyRate = (this.Value * this.Rate) * this.Quantity;
                    this.MonthlyRate = this.QuarterlyRate / 3;
                    this.BiMonthlyRate = this.MonthlyRate * 2;
                    this.BiAnnuallyRate = this.MonthlyRate * 6;
                    this.DailyRate = this.MonthlyRate / daysPerMonth;
                    this.YearlyRate = this.MonthlyRate * 12;
                    break;
                case "Bi-Annually":
                    this.BiAnnuallyRate = (this.Value * this.Rate) * this.Quantity;
                    this.MonthlyRate = this.BiAnnuallyRate / 6;
                    this.BiMonthlyRate = this.MonthlyRate * 2;
                    this.QuarterlyRate = this.MonthlyRate * 3;
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
