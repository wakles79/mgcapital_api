using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractListBoxViewModel : ListBoxViewModel
    {
        public int CustomerId { get; set; }

        public int BuildingId { get; set; }
    }
}
