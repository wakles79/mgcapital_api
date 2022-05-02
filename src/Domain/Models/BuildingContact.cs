using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class BuildingContact
    {
        [Key]
        [Required]
        public int BuildingId { get; set; }

        [Key]
        [Required]
        public int ContactId { get; set; }

        /// <summary>
        /// "Requester", "Manager", "Cleaning Guy"
        /// </summary>
        [MaxLength(80)]
        public string Type { get; set; }

        /// <summary>
        /// Defines the default address for a given entity, for the same EntityId and Type can only be "one and only one" default address
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Show History Data from this date
        /// </summary>
        public DateTime? ShowHistoryFrom { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }
    }
}
