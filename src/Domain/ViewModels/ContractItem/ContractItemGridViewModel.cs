using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemGridViewModel : ContractItemBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
