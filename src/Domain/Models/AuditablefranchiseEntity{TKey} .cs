// -----------------------------------------------------------------------
// <copyright file="Entity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations.Schema;
using MGCap.Domain.Entities;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Contains all the pertinent trackable fields.
    ///     And the related Company
    /// </summary>
    /// <typeparam name="TKey">The type of the project primary key</typeparam>
    public abstract class AuditableFranchiseEntity<TKey> : AuditableEntity<TKey>, IFranchiseEntity
    {
        public int? FranchiseId { get; set; }
        public Franchise Franchise { get; set; }
    }
}
