using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Revenue
{
    public class RevenueBaseViewModel : EntityViewModel
    {   
        public int? ContractId { get; set; }

        public int? BuildingId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime Date { get; set; }

        public double SubTotal { get; set; }

        public double Tax { get; set; }

        public double Total { get; set; }

        public string Description { get; set; }

        public string Reference { get; set; }

        public string TransactionNumber { get; set; }
    }
}
