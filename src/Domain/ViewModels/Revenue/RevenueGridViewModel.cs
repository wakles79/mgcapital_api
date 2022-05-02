using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Revenue
{
    public class RevenueGridViewModel : RevenueBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }

        public string BuildingName { get; set; }

        public string CustomerName { get; set; }

        public string ContractNumber { get; set; }
    }
}
