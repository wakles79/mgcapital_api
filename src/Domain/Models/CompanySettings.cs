// -----------------------------------------------------------------------
// <copyright file="CompanySettings.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CompanySettings : AuditableCompanyEntity
    {
        public double MinimumProfitMarginPercentage { get; set; }

        public double FederalInsuranceContributionsAct { get; set; }

        public double Medicare { get; set; }

        public double FederalUnemploymentTaxAct { get; set; }

        public double StateUnemploymentInsurance { get; set; }

        public double WorkersCompensation { get; set; }

        public double GeneralLedger { get; set; }

        public double StateTax { get; set; }

        // Freshdesk

        public string FreshdeskEmail { get; set; }

        public string FreshdeskDefaultAgentId { get; set; }

        public string FreshdeskDefaultApiKey { get; set; }

        // Logo 
        public string LogoBlobName { get; set; }

        public string LogoFullUrl { get; set; }

        // GMail
        public bool GmailEnabled { get; set; }

        public ulong? LastHistoryId { get; set; }

        public string GmailEmail { get; set; }

        public string EmailSignature { get; set; }
    }
}
