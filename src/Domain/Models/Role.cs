using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Role : Entity
    {
        [MaxLength(50)]
        public string Name { get; set; }

        public int Level { get; set; }

        public RoleType Type { get; set; }
    }
}
