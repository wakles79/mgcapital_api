using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class EmployeePermission
    {
        [Key]
        [Required]
        public int EmployeeId { get; set; }

        [Key]
        [Required]
        public int PermissionId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
    }
}
