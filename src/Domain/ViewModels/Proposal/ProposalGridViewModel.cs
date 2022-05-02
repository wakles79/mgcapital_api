using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Proposal
{
    public class ProposalGridViewModel : ProposalBaseViewModel
    {
        public Guid Guid { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }

        public int LineItems { get; set; }

        public string ContactName { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();

        public DateTime CreatedDate { get; set; }

        public double Value { get; set; }
    }
}
