// -----------------------------------------------------------------------
// <copyright file="InspectionItemTask.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class InspectionItemTask : AuditableEntity
    {
        public int InspectionItemId { get; set; }

        [ForeignKey("InspectionItemId")]
        public InspectionItem InspectionItem { get; set; }

        public string Description { get; set; }

        public bool IsComplete { get; set; }
    }
}
