// -----------------------------------------------------------------------
// <copyright file="AuditableEntity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Contains all the pertinent trackable fields
    ///     The type of the PK is int.
    /// </summary>
    public abstract class AuditableEntity : AuditableEntity<int>
    {
        public override void BeforeCreate(string userEmail = "", int companyId = -1)
        {
            base.BeforeCreate(userEmail, companyId);

            this.CreatedBy = userEmail;
            this.UpdatedBy = userEmail;

            this.CreatedDate = DateTime.UtcNow;
            this.UpdatedDate = this.CreatedDate;
        }

        public override void BeforeUpdate(string userEmail = "")
        {
            base.BeforeUpdate(userEmail);

            this.UpdatedBy = userEmail;
            this.UpdatedDate = DateTime.UtcNow;
        }
    }
}
