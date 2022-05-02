using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractOfficeSpace
{
    public class ContractOfficeSpaceGridViewModel : ContractOfficeSpaceBaseViewModel
    {
        public string OfficeTypeName { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
