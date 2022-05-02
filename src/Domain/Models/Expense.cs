// -----------------------------------------------------------------------
// <copyright file="Expense.cs" company="Axzes">
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
    public class Expense : AuditableCompanyEntity
    {
        public int? ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }

        public DateTime Date { get; set; }

        public string Vendor { get; set; }

        public ExpenseCategory Type { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        [MaxLength(32)]
        public string TransactionNumber { get; set; }
    }
}
