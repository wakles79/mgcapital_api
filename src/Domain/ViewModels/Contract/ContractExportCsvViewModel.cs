using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractExportCsvViewModel
    {
        public string Building { get; set; }

        public string MGMTCompany { get; set; }

        public double OccupiedSqFt { get; set; }

        public double MonthlyAmount { get; set; }

        // public double BillableRequests { get; set; }

        public double Total
        {
            get
            {
                return this.MonthlyAmount;
            }
        }

        public double Supervisor { get; set; }

        public double Dayporter { get; set; }

        public double Worker { get; set; }

        public double OperationsAdmin { get; set; }

        public double Van { get; set; }

        public double GrossLabor
        {
            get
            {
                return this.Supervisor + this.Dayporter + this.Worker + this.OperationsAdmin + this.Van;
            }
        }

        public double EmployeeTax { get; set; }

        public double Overhead { get; set; }

        public double TotalSupplies { get; set; }

        public double Insurance { get; set; }

        public double Phone { get; set; }

        public double Uniform { get; set; }

        public double TotalExpense { get; set; }

        public double GrossProfit { get; set; }

        public double Profit { get; set; }
    }
}
