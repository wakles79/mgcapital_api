// -----------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

namespace MGCap.Domain.Entities
{
    /// <summary>
    ///     Contains the common properties of the entities.
    /// </summary>
    /// <typeparam name="TKey">The type of the object primary key</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        ///     Gets or sets the primary key of the object.
        /// </summary>
        TKey ID { get; set; }
    }

    public interface IEntity
    { }

}