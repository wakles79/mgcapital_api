// -----------------------------------------------------------------------
// <copyright file="ICompanyEntity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using System;

namespace MGCap.Domain.Entities
{
    /// <summary>
    ///     Entities that belongs to a given company
    /// </summary>
    public interface ICompanyEntity
    {
        /// <summary>
        ///     Gets or sets the Company related PK
        /// </summary>
        int CompanyId { get; set; }

        /// <summary>
        ///     Gets or sets a unique identifier among companies
        /// </summary>
        Guid Guid { get; set; }

        Company Company { get; set; }

    }
}
