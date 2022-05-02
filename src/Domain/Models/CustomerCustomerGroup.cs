// -----------------------------------------------------------------------
// <copyright file="CustomerCustomerGroup.cs" company="Axzes">
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
    ///     with <see cref="Customer"/> and <see cref="CustomerGroup"/>
    /// </summary>
    public class CustomerCustomerGroup
    {
        [Key]
        [Required]
        public int CustomerGroupId { get; set; }

        [Key]
        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("CustomerGroupId")]
        public CustomerGroup CustomerGroup { get; set; }
    }
}
