using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
   public class ContactPhone : AuditableEntity
    {
        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public int ContactId { get; set; }

        /// <summary>
        /// "Work", "Home", "Fax", "Cell"
        /// </summary>
        [MaxLength(80)]
        public string Type { get; set; } 

        public bool Default { get; set; }


    }
}
