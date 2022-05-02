using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Permission : Entity
    {
        [MaxLength(50)]
        public string Name { get; set; }

        public ApplicationModule Module { get; set; }

        public ActionType Type { get; set; }
    }
}
