using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ContactAddress 
    {
        [Key]
        [Required]
        public int ContactId { get; set; }

        [Key]
        [Required]
        public int AddressId { get; set; }

        /// <summary>
        /// "Mailing", "Pick up", "Remit To"
        /// </summary>
        [MaxLength(80)]
        public string Type { get; set; }

        public string Name { get; set; }

        [ForeignKey("ContactId")]
        public  Contact Contact { get; set; }

        [ForeignKey("AddressId")]
        public  Address Address { get; set; }

        /// <summary>
        ///  Defines the default address for a given entity, for the same EntityId and Type can only be "one and only one" default address
        /// </summary>
        public bool Default { get; set; }

    }
}
