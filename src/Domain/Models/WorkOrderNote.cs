// <copyright file="WorkOrderNote.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class WorkOrderNote : AuditableEntity
    {
        public int WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrder { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public string Note { get; set; }
    }
}
