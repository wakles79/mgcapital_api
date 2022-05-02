using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.ContractExpense;
using MGCap.Domain.ViewModels.ContractItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractExportPdfViewModel
    {
        public string ContractNumber { get; set; }

        public string CustomerName { get; set; }

        public string BuildingName { get; set; }

        public double ProductionRate { get; set; }

        public string Description { get; set; }

        public double DaysPerMonth { get; set; }

        public string Status { get; set; }

        public int NumberOfPeople { get; set; }

        public string ExpirationDate { get; set; }

        public double OccupiedSquareFeets { get; set; }

        public double UnoccupiedSquareFeets { get; set; }

        public double TotalSquareFeets { get; set; }


        public double ExpensesOverheadDaily { get; set; }

        public double ExpensesOverheadMonthly { get; set; }

        public double ExpensesOverheadYearly { get; set; }


        public double DailyProfit { get; set; }

        public double MonthlyProfit { get; set; }

        public double YearlyProfit { get; set; }

        public double DailyProfitRatio { get; set; }

        public double MonthlyProfitRatio { get; set; }

        public double YearlyProfitRatio { get; set; }

        public IEnumerable<ContractItemExportPdfViewModel> Revenue { get; set; }

        public IEnumerable<ContractExpenseExportPdfViewModel> Expenses { get; set; }

        public ContractExportPdfViewModel()
        {
            this.Revenue = new HashSet<ContractItemExportPdfViewModel>();
            this.Expenses = new HashSet<ContractExpenseExportPdfViewModel>();
        }
    }
}
