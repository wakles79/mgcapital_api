// <copyright file="InspectionNote.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;


namespace MGCap.Domain.Models
{
    public class InspectionNote : AuditableEntity
    {
        public int InspectionId { get; set; }

        [ForeignKey("InspectionId")]
        public Inspection Inspection { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public string Note { get; set; }
    }
}
