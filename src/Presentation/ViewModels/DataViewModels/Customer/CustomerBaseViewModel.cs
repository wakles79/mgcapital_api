using MGCap.Presentation.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Customer
{
    public class CustomerBaseViewModel : EntityViewModel
    {
        [MaxLength(32)]
        [Required]
        public string Code { get; set; }

        [MaxLength(80)]
        [Required]
        public string Name { get; set; }

        public int StatusId { get; set; }

        public bool IsGenericAccount { get; set; }

        public string Notes { get; set; }

        public double MinimumProfitMargin { get; set; }

        public bool PONumberRequired { get; set; }

        #region Credit Info
        public double CreditLimit { get; set; }

        public string CRHoldFlag { get; set; }

        public string CreditTerms { get; set; }

        public bool ShowPricesOnShipper { get; set; }

        public double? InsuredUpTo { get; set; }

        public string InsurerName { get; set; }

        public bool FinanceCharges { get; set; }

        public int GracePeriodInDays { get; set; }
        #endregion
    }
}
