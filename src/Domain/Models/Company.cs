// -----------------------------------------------------------------------
// <copyright file="Company.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Represent a company in the system
    /// </summary>
    public class Company : AuditableEntity, IFranchiseEntity
    {
        [MaxLength(80)]
        public string Name { get; set; }
        public int? FranchiseId { get; set; }
        public Franchise Franchise { get; set; }
    }
}
