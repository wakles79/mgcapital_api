using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemUpdateOrderViewModel
    {
        [Required]
        public int PreviousContractItemId { get; set; }

        [Required]
        public int PreviousContractItemOrder { get; set; }

        [Required]
        public int NextContractItemId { get; set; }

        [Required]
        public int NextContractItemOrder { get; set; }
    }
}
