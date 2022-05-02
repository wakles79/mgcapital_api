// <copyright file="ExpenseType.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.Models
{
    public class ExpenseType : AuditableCompanyEntity
    {
        /// <summary>
        /// Category of the expense type.
        /// '0 - Labor', '1 - Equipments', '2 - Supplies', '3 -Others'
        /// </summary>
        [Required]
        public int ExpenseCategory { get; set; }

        /// <summary>
        /// Description of the expense type
        /// </summary>
        [MaxLength(128)]
        public string Description { get; set; }

        /// <summary>
        /// Status of the expense type
        /// could be = '0 - Inactive', '1 - Active'
        /// </summary>
        [Required]
        public bool Status { get; set; }
    }
}
