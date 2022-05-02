using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractItem
{
    public class ContractItemUpdateViewModel : ContractItemBaseViewModel
    {
        public bool UpdatePrepopulatedItems { get; set; }
    }
}
