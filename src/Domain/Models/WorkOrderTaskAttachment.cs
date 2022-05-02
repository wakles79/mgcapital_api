// <copyright file="WorkOrderTaskAttachment.cs" company="Axzes">
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
    public class WorkOrderTaskAttachment : Attachment
    {
        public string Title { get; set; }

        public DateTime ImageTakenDate { get; set; }

        public int WorkOrderTaskId { get; set; }

        [ForeignKey("WorkOrderTaskId")]
        public WorkOrderTask WorkOrderTask { get; set; }
    }
}
