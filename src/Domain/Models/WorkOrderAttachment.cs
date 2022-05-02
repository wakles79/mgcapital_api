// <copyright file="WorkOrderAttachment.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class WorkOrderAttachment : Attachment
    {      
        public DateTime ImageTakenDate { get; set; }

        public int WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrder { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
