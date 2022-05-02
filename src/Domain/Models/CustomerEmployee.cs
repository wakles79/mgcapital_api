// -----------------------------------------------------------------------
// <copyright file="CustomerEmployee.cs" company="Axzes">
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
    /// <summary>
    ///     Pivot Table to establish the "many-to-many relationship"
    ///     with <see cref="Employee"/> and <see cref="Customer"/>
    /// </summary>
    public class CustomerEmployee
    {
        [Key]
        [Required]
        public int EmployeeId { get; set; }

        [Key]
        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        /// <summary>
        ///     Gets or Sets the current relationthip type
        ///     E.G "Outside Sales Repr 1", "Outside Sales Repr 2",
        ///     "Inside Sales Repr"
        /// </summary>
        [MaxLength(128)]
        public string Type { get; set; }
    }
}
