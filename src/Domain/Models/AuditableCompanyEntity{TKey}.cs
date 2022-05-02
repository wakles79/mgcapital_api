// -----------------------------------------------------------------------
// <copyright file="Entity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
using MGCap.Domain.Entities;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Contains all the pertinent trackable fields.
    ///     And the related Company
    /// </summary>
    /// <typeparam name="TKey">The type of the project primary key</typeparam>
    public abstract class AuditableCompanyEntity<TKey> : AuditableEntity<TKey>, ICompanyEntity
    {
        public AuditableCompanyEntity(){
            Guid = System.Guid.NewGuid();
        }
        /// <summary>
        ///     Represents the PK of the Company table
        ///     where the object belongs
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Globally unique identifier of the object
        /// </summary>
        public Guid Guid { get; set; }

        [IgnoreInsert]
        public Company Company { get; set; }
    }
}
