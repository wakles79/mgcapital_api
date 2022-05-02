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
    ///     The type of the PK is int.
    /// </summary>
    public abstract class EntityViewModel : EntityViewModel<int>
    {
    }
}
