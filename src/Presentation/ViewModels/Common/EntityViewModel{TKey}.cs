// -----------------------------------------------------------------------
// <copyright file="EntityViewModel.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

namespace MGCap.Presentation.ViewModels.Common
{
    /// <summary>
    ///     Contains the common property of every VM.
    /// </summary>
    /// <typeparam name="TKey">The type of the Entity key</typeparam>
    public abstract class EntityViewModel<TKey>
    {
        /// <summary>
        ///     Gets or sets the primary key of the object
        /// </summary>
        public TKey ID { get; set; }
    }
}
