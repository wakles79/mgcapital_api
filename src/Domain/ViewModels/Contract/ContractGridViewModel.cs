using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MGCap.Domain.Utils;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractGridViewModel : EntityViewModel
    {
        public string ContractNumber { get; set; }

        public int CustomerId { get; set; }

        public string CustomerFullName { get; set; }

        public string CustomerCode { get; set; }

        public string BuildingName { get; set; }

        public string BuildingCode { get; set; }

        public int Status { get; set; }

        public int TotalItems { get; set; }

        public double MonthlyRate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int ExpirationDateEpoch => this.ExpirationDate.HasValue ? this.ExpirationDate.Value.ToEpoch() : 0;


        public double OccupiedSquareFeets { get; set; }

        public double UnoccupiedSquareFeets { get; set; }

        public DateTime UpdatedDate {get; set;}

        public double DailyProfit { get; set; }

        public double MonthlyProfit { get; set; }

        public double YearlyProfit { get; set; }

        public double DailyProfitRatio { get; set; }

        public double MonthlyProfitRatio { get; set; }

        public double YearlyProfitRatio { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }

        public double TotalProfitRatio { get; set; }

        public double TotalProfitAmount { get; set; }

        public double TotalMonthlyRevenue { get; set; }

        public double TotalMonthlyExpense { get; set; }

        public double TotalMonthlyLaborExpense { get; set; }
    }
}
