using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MGCap.Domain.Enums;

namespace MGCap.Domain.Models
{
    public class BuildingEmployee
    {
        [Key]
        [Required]
        public int BuildingId { get; set; }

        [Key]
        [Required]
        public int EmployeeId { get; set; }

        [Key]
        [Required]
        public BuildingEmployeeType Type { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
