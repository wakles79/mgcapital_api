using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public static class BugdetUtils
    {
        public static ContractExpense UpdateContractExpenseRatePeriods(this ContractExpense contractExpense, double daysPerMonth)
        {
            double rate = contractExpense.Rate;
            if (contractExpense.ExpenseCategory == (int)ExpenseCategory.Labor)
            {
                rate = rate + (rate * (contractExpense.OverheadPercent / 100));
            }
            switch (contractExpense.RatePeriodicity)
            {
                case "Daily":
                    contractExpense.DailyRate = (contractExpense.Value * rate) * contractExpense.Quantity;
                    contractExpense.MonthlyRate = contractExpense.DailyRate * daysPerMonth;
                    contractExpense.YearlyRate = contractExpense.MonthlyRate * 12;
                    break;
                case "Monthly":
                    contractExpense.MonthlyRate = (contractExpense.Value * rate) * contractExpense.Quantity;
                    contractExpense.DailyRate = contractExpense.MonthlyRate / daysPerMonth;
                    contractExpense.YearlyRate = contractExpense.MonthlyRate * 12;
                    break;
                case "Yearly":
                    contractExpense.YearlyRate = (contractExpense.Value * rate) * contractExpense.Quantity;
                    contractExpense.MonthlyRate = contractExpense.YearlyRate / 12;
                    contractExpense.DailyRate = contractExpense.MonthlyRate / daysPerMonth;
                    break;
                default:
                    break;
            }
            return contractExpense;
        }

        public static ContractItem UpdateContractRevenueRatePeriods(this ContractItem contractRevenue, double daysPerMonth)
        {
            double value = 0;
            if (contractRevenue.RateType == ServiceRateType.Hour)
            {
                value = contractRevenue.Hours.HasValue ? contractRevenue.Hours.Value : 0;
            }
            else if (contractRevenue.RateType == ServiceRateType.Unit)
            {
                value = 1;
            }
            else if (contractRevenue.RateType == ServiceRateType.Room)
            {
                value = contractRevenue.Rooms.HasValue ? contractRevenue.Rooms.Value : 0;
            }
            else if (contractRevenue.RateType == ServiceRateType.SquareFeet)
            {
                value = contractRevenue.SquareFeet.HasValue ? contractRevenue.SquareFeet.Value : 0;
            }

            double rate = contractRevenue.Rate;

            double quarterlyRate = 0;
            double biMonthlyRate = 0;
            double biAnnuallyRate = 0;

            switch (contractRevenue.RatePeriodicity)
            {
                case "Daily":
                    contractRevenue.DailyRate = (value * rate) * contractRevenue.Quantity;
                    contractRevenue.MonthlyRate = contractRevenue.DailyRate * daysPerMonth;
                    contractRevenue.YearlyRate = contractRevenue.MonthlyRate * 12;
                    break;
                case "Monthly":
                    contractRevenue.MonthlyRate = (value * rate) * contractRevenue.Quantity;
                    contractRevenue.DailyRate = contractRevenue.MonthlyRate / daysPerMonth;
                    contractRevenue.YearlyRate = contractRevenue.MonthlyRate * 12;
                    break;
                case "Bi-Monthly":
                    biMonthlyRate = (value * rate) * contractRevenue.Quantity;
                    contractRevenue.MonthlyRate = biMonthlyRate / 2;
                    contractRevenue.DailyRate = contractRevenue.MonthlyRate / daysPerMonth;
                    contractRevenue.YearlyRate = contractRevenue.MonthlyRate * 12;

                    break;
                case "Quarterly":
                    quarterlyRate = (value * rate) * contractRevenue.Quantity;
                    contractRevenue.MonthlyRate = quarterlyRate / 3;
                    contractRevenue.DailyRate = contractRevenue.MonthlyRate / daysPerMonth;
                    contractRevenue.YearlyRate = contractRevenue.MonthlyRate * 12;
                    break;
                case "Bi-Annually":
                    biAnnuallyRate = (value * rate) * contractRevenue.Quantity;
                    contractRevenue.MonthlyRate = biAnnuallyRate / 6;
                    contractRevenue.DailyRate = contractRevenue.MonthlyRate / daysPerMonth;
                    contractRevenue.YearlyRate = contractRevenue.MonthlyRate * 12;
                    break;
                case "Yearly":
                    contractRevenue.YearlyRate = (value * rate) * contractRevenue.Quantity;
                    contractRevenue.MonthlyRate = contractRevenue.YearlyRate / 12;
                    contractRevenue.DailyRate = contractRevenue.MonthlyRate / daysPerMonth;
                    break;
                default:
                    break;
            }
            return contractRevenue;
        }
    }
}
