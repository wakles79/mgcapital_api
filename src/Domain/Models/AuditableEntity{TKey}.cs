// -----------------------------------------------------------------------
// <copyright file="AuditableEntity{TKey}.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Entities;
using MGCap.Domain.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Contains all the pertinent trackable fields.
    /// </summary>
    /// <typeparam name="TKey">The type of the project primary key</typeparam>
    public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
    {
        /// <inheritdoc/>
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        
        /// <summary>
        ///     Gets updated date in Unix time
        /// </summary>
        [NotMapped]
        public int EpochCreatedDate => this.CreatedDate.ToEpoch();

        /// <inheritdoc/>
        [MaxLength(80)]
        [ScaffoldColumn(false)]
        public string CreatedBy { get; set; }

        /// <inheritdoc/>
        [ScaffoldColumn(false)]
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        ///     Gets updated date in Unix time
        /// </summary>
        [NotMapped]
        public int EpochUpdatedDate => this.UpdatedDate.ToEpoch();

        /// <inheritdoc/>
        [MaxLength(80)]
        [ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }

        public override void BeforeCreate(string userEmail = "", int companyId = -1)
        {
            this.CreatedBy = userEmail;
            this.UpdatedBy = userEmail;

            this.CreatedDate = DateTime.UtcNow;
            this.UpdatedDate = DateTime.UtcNow;
        }

        public override void BeforeUpdate(string userEmail = "")
        {
            this.UpdatedBy = userEmail;
            this.UpdatedDate = DateTime.UtcNow;
        }
    }
}
