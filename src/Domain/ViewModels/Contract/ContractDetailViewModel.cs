using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractDetailViewModel : ContractBaseViewModel
    {
        public Guid Guid { get; set; }

        public string CustomerName { get; set; }

        public string BuildingName { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();

        public int EpochExpirationDate => this.ExpirationDate.HasValue ? this.ExpirationDate.Value.ToEpoch() : 0;
    }
}
