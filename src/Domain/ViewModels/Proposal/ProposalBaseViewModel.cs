using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Proposal
{
    public class ProposalBaseViewModel : EntityViewModel
    {
        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public int ContactId { get; set; }

        [MaxLength(128)]
        public string Location { get; set; }

        public ProposalStatus Status { get; set; }

        public DateTime? StatusChangedDate { get; set; }

        public int? BillTo { get; set; }

        public string BillToName { get; set; }

        public string BillToEmail { get; set; }
    }
}
