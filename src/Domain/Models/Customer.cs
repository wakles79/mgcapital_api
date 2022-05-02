using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Models
{
    public class Customer : AuditableCompanyEntity
    {
        /// <summary>
        /// Displayable field for input ID's for the customers
        /// </summary>
        
        [MaxLength(32)]
        [Required]        
        public string Code { get; set; }

        [MaxLength(80)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Active, Not Active 
        /// </summary>
        public int StatusId { get; set; }

        public bool IsGenericAccount { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// It's a percentage, represents the minimum margin amount when it comes to purchasing.
        /// E.g if a Customer has this `MinimumProfitMargin= 20` and a Product (or Item) is worth
        /// $100, then the sales man knows that he/she has to sell it for at least $120
        /// </summary>
        public double MinimumProfitMargin { get; set; }
        /// <summary>
        /// Yes or No (the current system has this, we don't know why)
        /// </summary>
        public bool PONumberRequired { get; set; }

        #region Credit Info
        public double CreditLimit { get; set; }

        /// <summary>
        /// Options: D, R, Y
        /// </summary>
        public string CRHoldFlag { get; set; }

        /// <summary>
        /// Options: 10, COD, NET30
        /// </summary>
        public string CreditTerms { get; set; }

        /// <summary>
        /// Shows or not the prices on the "shipper" document
        /// Display : "Ship Docs"
        /// </summary>
        public bool ShowPricesOnShipper { get; set; }

        /// <summary>
        /// It's dollar amount backed by an insurance company for this company approved line of credit 
        /// </summary>
        public double? InsuredUpTo { get; set; }

        /// <summary>
        /// It's the company insuring the approved credit limit
        /// </summary>
        public string InsurerName { get; set; }

        public bool FinanceCharges { get; set; }

        [MaxLength(1)]
        public string PricingMethod { get; set; }

        [MaxLength(1)]
        public string PricingColumn { get; set; }

        public int PricingRow { get; set; }

        public bool FixedMarkupRate { get; set; }

        public int GracePeriodInDays { get; set; }

        public bool InformSalesRepMinMargin { get; set; }
        #endregion

        public IEnumerable<CustomerCertificate> Certificates { get; set; }

        public IEnumerable<CustomerPhone> Phones { get; set; }

        public IEnumerable<CustomerAddress> Addresses { get; set; }

        public IEnumerable<CustomerContact> Contacts { get; set; }

        public IEnumerable<CustomerEmployee> Employees { get; set; }

        public IEnumerable<CustomerCustomerGroup> Groups { get; set; }
        
        public Customer()
        {
            this.Certificates = new HashSet<CustomerCertificate>();
            this.Phones = new HashSet<CustomerPhone>();
            this.Addresses = new HashSet<CustomerAddress>();
            this.Employees = new HashSet<CustomerEmployee>();
            this.Groups = new HashSet<CustomerCustomerGroup>();
            this.Contacts = new HashSet<CustomerContact>();
        }
    }
}
