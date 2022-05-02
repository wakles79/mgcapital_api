using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Service: AuditableCompanyEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        ///     Could be hours, square feet etc
        /// </summary>
        [MaxLength(10)]
        public string UnitFactor { get; set; }
        public double UnitPrice { get; set; }
        /// <summary>
        ///     The minimum total that a service line could have
        /// </summary>
        public double MinPrice { get; set; }
    }
}
