using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.ViewModels.Common;
namespace MGCap.Domain.ViewModels.CompanySettings
{
    public class CompanySettingsBaseViewModel : EntityViewModel
    {
        public int CompanyId { get; set; }

        public double MinimumProfitMarginPercentage { get; set; }

        public double FederalInsuranceContributionsAct { get; set; }

        public double Medicare { get; set; }

        public double FederalUnemploymentTaxAct { get; set; }

        public double StateUnemploymentInsurance { get; set; }

        public double WorkersCompensation { get; set; }

        public double GeneralLedger { get; set; }

        public double StateTax { get; set; }

        public string FreshdeskEmail { get; set; }

        public string FreshdeskDefaultAgentId { get; set; }

        public string FreshdeskDefaultApiKey { get; set; }

        public string LogoBlobName { get; set; }

        public string LogoFullUrl { get; set; }

        public bool GmailEnabled { get; set; }

        public ulong? LastHistoryId { get; set; }

        public string GmailEmail { get; set; }

        public string EmailSignature { get; set; }
    }
}
