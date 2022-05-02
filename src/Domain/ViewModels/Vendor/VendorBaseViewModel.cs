using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Vendor
{
    public class VendorBaseViewModel
    {
        public int ID { get; set; }
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

        public string DefaultGLAccountNumber1 { get; set; }

        public string DefaultGLAccountNumber2 { get; set; }

        public int LeadTime { get; set; }
    }
}
