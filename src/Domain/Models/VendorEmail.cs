using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class VendorEmail : AuditableEntity
    {
        public int VendorId { get; set; }

        [MaxLength(128)]
        public string Email { get; set; }

        [MaxLength(80)]
        public string Type { get; set; } // -> "Work", "Home"

        public bool Default { get; set; }

        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }
    }
}
