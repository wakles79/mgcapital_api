using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Contact : AuditableCompanyEntity
    {
        [MaxLength(80)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(80)]
        public string MiddleName { get; set; }

        [MaxLength(80)]
        [Required]
        public string LastName { get; set; }

        /// <summary>
        ///     E.g Mr., Ms., Dr.
        /// </summary>
        [MaxLength(80)]
        public string Salutation { get; set; }

        public string FullName => string.Join(" ", new List<string> { this.FirstName, this.MiddleName, this.LastName }.Where(s => !String.IsNullOrEmpty(s)));

        [MaxLength(1)]
        public string Gender { get; set; }

        /// <summary>
        /// Date of Birth
        /// </summary>
        public DateTime? DOB { get; set; }

        /// <summary>
        /// Same as Position, this could be seen as a repetitive field 
        /// (we already have it on the many-to-many relationship). But we keep it in case there is a "unrelated" contact
        /// </summary>
        [MaxLength(80)]
        public string Title { get; set; }

        /// <summary>
        /// Could be same as the customer reference 
        /// Same redundancy as `Title`
        /// </summary>
        [MaxLength(80)]
        public string CompanyName { get; set; }

        /// <summary>
        /// General Notes, can serve as a different contact way
        /// </summary>
        public string Notes { get; set; }

        public bool SendNotifications { get; set; }

        public virtual ICollection<ContactEmail> Emails { get; set; }

        public virtual ICollection<ContactPhone> Phones { get; set; }

        public virtual ICollection<ContactAddress> Addresses { get; set; }

        public Contact()
        {
            Emails = new HashSet<ContactEmail>();
            Phones = new HashSet<ContactPhone>();
            Addresses = new HashSet<ContactAddress>();
        }
    }
}
