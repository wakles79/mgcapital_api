// <copyright file="WorkOrderSource.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MGCap.Domain.Models
{
    public class WorkOrderSource: Entity
    {
        /// <summary>
        ///     Gets or sets the WO Source
        /// </summary>
        [Required]
        public string Name { get; set; }

        public int Code { get; set; }
    }
}
