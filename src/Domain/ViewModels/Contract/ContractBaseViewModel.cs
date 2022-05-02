using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{

    public class ContractBaseViewModel : EntityViewModel
    {
        [MaxLength(128)]
        public string ContractNumber { get; set; }

        public int Area { get; set; }

        public int NumberOfPeople { get; set; }

        public int BuildingId { get; set; }

        public int CustomerId { get; set; }

        public int ContactSignerId { get; set; }

        public ContractStatus Status { get; set; }

        public string Description { get; set; }

        public double DaysPerMonth { get; set; }
        
        public int NumberOfRestrooms { get; set; }

        public double FrequencyPerYear { get; set; }

        public DateTime? ExpirationDate { get; set; }
        
        public double ProductionRate { get; set; }

        public double UnoccupiedSquareFeets { get; set; }

        public bool EditionCompleted { get; set; }

        public double DailyProfit { get; set; }

        public double MonthlyProfit { get; set; }

        public double YearlyProfit { get; set; }

        public double DailyProfitRatio { get; set; }

        public double MonthlyProfitRatio { get; set; }

        public double YearlyProfitRatio { get; set; }

        // TODO: Define remaining fields: CreatedDate, FinishedDate, UpdatedDate, Revenues(Other Entity), Expenses (Other Entity)
    }
}
