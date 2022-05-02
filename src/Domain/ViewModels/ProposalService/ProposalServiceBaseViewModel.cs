using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ProposalService
{
    public class ProposalServiceBaseViewModel : EntityViewModel
    {
        public int ProposalId { get; set; }

        public int BuildingId { get; set; }

        public string BuildingName { get; set; }

        public int OfficeServiceTypeId { get; set; }

        public int Quantity { get; set; }

        public string RequesterName { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public double Rate { get; set; }

        public DateTime? DateToDelivery { get; set; }
    }
}
