using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemBaseViewModel : EntityViewModel
    {
        public int Quantity { get; set; }

        [MaxLength(128)]
        public string Description { get; set; }

        public int ContractId { get; set; }

        public int OfficeServiceTypeId { get; set; }

        public string OfficeServiceTypeName { get; set; }

        public double Rate { get; set; }

        public int RateType { get; set; }

        [MaxLength(15)]
        public string RatePeriodicity { get; set; }

        public double? SquareFeet { get; set; }

        public double? Amount { get; set; }

        public double? Hours { get; set; }

        public double? Rooms { get; set; }

        public ContractItemDefaultType DefaultType { get; set; }

        public double DailyRate { get; set; }

        public double MonthlyRate { get; set; }

        public double YearlyRate { get; set; }

        public int Order { get; set; }
    }
}