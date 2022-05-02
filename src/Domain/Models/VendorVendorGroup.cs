using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class VendorVendorGroup
    {
        [Key]
        [Required]
        public int VendorGroupId { get; set; }

        [Key]
        [Required]
        public int VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }

        [ForeignKey("VendorGroupId")]
        public VendorGroup VendorGroup { get; set; }
    }
}
