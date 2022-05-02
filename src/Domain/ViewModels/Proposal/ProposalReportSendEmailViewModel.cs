using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Proposal
{
    public class ProposalReportSendEmailViewModel : EntityViewModel
    {
        /// <summary>
        /// Full name and email of additional recipients
        /// </summary>
        public IEnumerable<ProposalAdditionalRecipientViewModel> AdditionalRecipients { get; set; }
    }
}
