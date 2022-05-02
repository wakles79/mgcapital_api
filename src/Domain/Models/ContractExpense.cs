// <copyright file="ContractExpense.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ContractExpense : AuditableEntity
    {
        public int Quantity { get; set; }

        [MaxLength(128)]
        public string Description { get; set; }

        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }

        public int ContractId { get; set; }

        [Required]
        public int ExpenseCategory { get; set; }

        public string ExpenseTypeName { get; set; }

        public int ExpenseSubcategoryId { get; set; }

        public string ExpenseSubcategoryName { get; set; }

        [Required]
        public double Rate { get; set; }

        [Required]
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
