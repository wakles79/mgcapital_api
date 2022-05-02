using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemCreateViewModel : ContractItemBaseViewModel
    {
        public string ContractNumber { get; set; }

        public bool UpdatePrepopulatedItems { get; set; }
    }
}
