// -----------------------------------------------------------------------
// <copyright file="Entity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Contains all the pertinent trackable fields
    ///     And the related Company
    ///     The type of the PK is int.
    /// </summary>
    public abstract class AuditableCompanyEntity : AuditableCompanyEntity<int>
    {
        public override void BeforeCreate(string userEmail = "", int companyId = -1)
        {
            base.BeforeCreate(userEmail, companyId);

            if (companyId > 0)
            {
                this.CompanyId = companyId;
            }

            this.Guid = System.Guid.NewGuid();
        }

        public override void BeforeUpdate(string userEmail = "")
        {
            base.BeforeUpdate(userEmail);
        }
    }
}
