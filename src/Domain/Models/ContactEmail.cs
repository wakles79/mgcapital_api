using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ContactEmail : AuditableEntity
    {
        public int ContactId { get; set; }

        [MaxLength(128)]
        public string Email { get; set; }

        [MaxLength(80)]
        public string Type { get; set; } // -> "Work", "Home"

        public bool Default { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }
    }
}
