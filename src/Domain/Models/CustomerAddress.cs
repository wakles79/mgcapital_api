using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CustomerAddress
    {
        [Key]
        [Required]
        public int CustomerId { get; set; }

        [Key]
        [Required]
        public int AddressId { get; set; }

        /// <summary>
        /// "Mailing", "Pick up", "Remit To"
        /// </summary>
        [MaxLength(80)]
        public string Type { get; set; }

        [MaxLength(80)]
        public string Name { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        /// <summary>
        ///  Defines the default address for a given entity, for the same EntityId and Type can only be "one and only one" default address
        /// </summary>
        public bool Default { get; set; }
    }
}
