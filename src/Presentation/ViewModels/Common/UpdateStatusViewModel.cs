// -----------------------------------------------------------------------
// <copyright file="UpdateStatusViewModel.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.Common
{
    /// <summary>
    ///     Common obj that has the new status for a given entity
    /// </summary>
    public class UpdateStatusViewModel : EntityViewModel
    {
        /// <summary>
        ///     Gets or sets the new status for a given entity
        /// </summary>
        public int StatusId { get; set; }
    }
}
