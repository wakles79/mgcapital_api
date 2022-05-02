using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class VendorContact
    {
        [Key]
        [Required]
        public int VendorId { get; set; }

        [Key]
        [Required]
        public int ContactId { get; set; }

        /// <summary>
        /// "President", "Accounting", "CEO"
        /// </summary>
        [MaxLength(80)]
        public string Type { get; set; }

        /// <summary>
        /// Defines the default address for a given entity, for the same EntityId and Type can only be "one and only one" default address
        /// </summary>
        public bool Default { get; set; }

        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }

    }
}
