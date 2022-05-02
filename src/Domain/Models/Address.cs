using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Address : AuditableEntity
    {
        [MaxLength(80)]
        public string AddressLine1 { get; set; }

        [MaxLength(80)]
        public string AddressLine2 { get; set; }

        [MaxLength(80)]
        public string City { get; set; }

        [MaxLength(80)]
        public string State { get; set; }

        [MaxLength(32)]
        public string ZipCode { get; set; }

        [MaxLength(3)]
        public string CountryCode { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        /// <summary>
        ///     Computed SQL column that summarize the address
        /// </summary>
        public string FullAddress { get; set; }        
    }
}
