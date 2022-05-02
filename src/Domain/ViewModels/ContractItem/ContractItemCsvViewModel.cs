using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemCsvViewModel
    {
        public string ContractNumber { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }

        public string OfficeServiceTypeName { get; set; }

        public double Rate { get; set; }

        public ServiceRateType RateType { get; set; }

        public double DailyRate { get; set; }

        public string RatePeriodicity { get; set; }

        public double Hours { get; set; }

        public double Amount { get; set; }

        public double Rooms { get; set; }

        public double SquareFeet { get; set; }

        public double Value { get; set; }
    }
}
