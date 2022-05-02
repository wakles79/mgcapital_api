using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MGCap.Domain.Enums;

namespace MGCap.Domain.Models
{
    public class WorkOrderEmployee
    {
        [Key]
        [Required]
        public int WorkOrderId { get; set; }

        [Key]
        [Required]
        public int EmployeeId { get; set; }

        [Key]
        [Required]
        public WorkOrderEmployeeType Type { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrder { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
