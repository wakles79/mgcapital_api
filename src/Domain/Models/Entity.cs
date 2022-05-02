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
    ///     Contains the common properties of the entities.
    ///     The type of the PK is int.
    /// </summary>
    /// <typeparam name="TKey">The type of the object primary key</typeparam>
    public abstract class Entity : Entity<int>
    {
    }
}
