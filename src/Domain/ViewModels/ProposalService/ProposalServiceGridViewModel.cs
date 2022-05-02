using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.ProposalService
{
    public class ProposalServiceGridViewModel : ProposalServiceBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }

        public string OfficeServiceTypeName { get; set; }
    }
}
