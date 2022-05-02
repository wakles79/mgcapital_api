using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Vendor : AuditableCompanyEntity
    {
        [MaxLength(32)]
        [Required]
        public string Code { get; set; }

        public int VendorTypeId { get; set; }

        public bool IsPerson { get; set; }

        [MaxLength(9)]
        public string SSN { get; set; }

        [MaxLength(80)]
        public string FEIN { get; set; }

        public string FederalId => IsPerson ? SSN : FEIN;

        /// <summary>
        /// Only display if IsPerson is True
        /// </summary>
        public bool Is1099 { get; set; }

        public bool IsSensitiveAccount { get; set; }

        [MaxLength(80)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///  General Notes, can serve as a different contact way
        /// </summary>
        public string Notes { get; set; }

        public int StatusId { get; set; }

        #region  Accounting fields

        public double CreditLimit { get; set; }

        /// <summary>
        /// These fields have 'D'
        /// </summary>
        [MaxLength(1)]
        public string TermsDaysOrProx { get; set; }

        public double TermsDiscPercent { get; set; }

        public int TermsDiscDays { get; set; }

        /// <summary>
        /// In some cases they have standard terms e.g 15, 30, 60, 90 but we've seen 22, 24.
        /// </summary>
        public int TermsNet { get; set; }

        [MaxLength(128)]
        public string AccountNumber { get; set; }

        [MaxLength(10)]
        public string DefaultGLAccountNumber1 { get; set; }

        [MaxLength(10)]
        public string DefaultGLAccountNumber2 { get; set; }
        #endregion

        /// <summary>
        /// Amount in days
        /// </summary>
        public int LeadTime { get; set; }

        public  ICollection<VendorPhone> Phones { get; set; }

        public  ICollection<VendorEmail> Emails { get; set; }

        public ICollection<VendorAddress> Addresses { get; set; }

        public ICollection<VendorVendorGroup> Groups { get; set; }

        public ICollection<VendorContact> Contacts { get; set;
        }
        public Vendor()
        {
            Phones = new HashSet<VendorPhone>();
            Emails = new HashSet<VendorEmail>();
            Addresses = new HashSet<VendorAddress>();
            Groups = new HashSet<VendorVendorGroup>();
            Contacts = new HashSet<VendorContact>();
        }
    }
}
