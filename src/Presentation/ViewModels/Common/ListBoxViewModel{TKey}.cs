// -----------------------------------------------------------------------
// <copyright file="ListBoxViewModel{TKey}.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

namespace MGCap.Presentation.ViewModels.Common
{
    /// <summary>
    ///     Standard view model for all <select> html elements
    /// </summary>
    public abstract class ListBoxViewModel<TKey> : EntityViewModel<TKey>
    {
        public string Name { get; set; }
    }
}
