using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class InspectionItemNote : AuditableEntity
    {
        public int InspectionItemId { get; set; }

        [ForeignKey("InspectionItemId")]
        public InspectionItem InspectionItem { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public string Note { get; set; }
    }
}
