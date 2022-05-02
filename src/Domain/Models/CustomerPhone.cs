using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CustomerPhone : AuditableEntity
    {
        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public int CustomerId { get; set; }

        /// <summary>
        /// "Work", "Home", "Fax", "Cell"
        /// </summary>
        [MaxLength(80)]
        public string Type { get; set; }

        public bool Default { get; set; }
    }
}
