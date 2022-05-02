using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.ProposalService;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Proposal
{
    public class ProposalReportDetailViewModel : ProposalBaseViewModel
    {
        public string ContactName { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();

        public Guid Guid { get; set; }

        public IEnumerable<ProposalServiceGridViewModel> ProposalServices { get; set; }

        public DateTime CreatedDate { get; set; }

        public ProposalReportDetailViewModel()
        {
            this.ProposalServices = new HashSet<ProposalServiceGridViewModel>();
        }
    }
}
