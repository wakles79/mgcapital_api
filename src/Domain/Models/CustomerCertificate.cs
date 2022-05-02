using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CustomerCertificate : AuditableEntity
    {
        public int CustomerId { get; set; }

        [MaxLength(20)]
        [Required]
        public string Number { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        public string ResaleNumber => string.Join(" ", new List<string> { this.ExpirationDate.Year.ToString(), this.Number }.Where(s => !String.IsNullOrEmpty(s)));

    }
}
