// -----------------------------------------------------------------------
// <copyright file="IAuditableEntity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.Entities
{
    /// <summary>
    ///     Contains all the pertinent trackable fields.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> that idicates
        ///     when the entity was created.
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the ID that idicates
        ///     the creator of the entity.
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> that idicates
        ///     when the entity was updated.
        /// </summary>
        DateTime UpdatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the ID that idicates
        ///     the editor of the entity.
        /// </summary>
        string UpdatedBy { get; set; }
    }
}
