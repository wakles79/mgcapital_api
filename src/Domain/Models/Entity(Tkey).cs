// -----------------------------------------------------------------------
// <copyright file="Entity{TKey}.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.Domain.Entities;

namespace MGCap.Domain.Models
{
    /// <summary>
    ///     Contains the common properties of the entities.
    /// </summary>
    /// <typeparam name="TKey">The type of the object primary key</typeparam>
    public abstract class Entity<TKey> : BaseEntity, IEntity<TKey>
    {
        /// <summary>
        ///     Gets or sets the primary of the object.
        /// </summary>
        [IgnoreInsert]
        public TKey ID { get; set; }
    }
}
