// <copyright file="ExpenseSubcategory.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ExpenseSubcategory : AuditableEntity
    {
        /// <summary>
        /// Name of the subcategory
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public int ExpenseTypeId { get; set; }

        [ForeignKey("ExpenseTypeId")]
        public ExpenseType ExpenseType { get; set; }

        /// <summary>
        /// Rate of the expense subcategory
        /// </summary>
        [Required]
        public double Rate { get; set; }

        /// <summary>
        /// Rate type
        /// </summary>
        [Required]
        public int RateType { get; set; }

        /// <summary>
        /// Periodicity of the subcategory to calculate the total
        /// </summary>
        [Required]
        public string Periodicity { get; set; }

        /// <summary>
        /// Status of the subcategory
        /// </summary>
        [Required]
        public bool Status { get; set; }
    }
}
