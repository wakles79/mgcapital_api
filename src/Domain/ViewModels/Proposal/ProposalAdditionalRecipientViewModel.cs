using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Proposal
{
    public class ProposalAdditionalRecipientViewModel
    {
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
