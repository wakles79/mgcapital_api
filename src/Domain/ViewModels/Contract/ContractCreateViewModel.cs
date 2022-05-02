using MGCap.Domain.ViewModels.ContractOfficeSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Contract
{
    public class ContractCreateViewModel : ContractBaseViewModel
    {
        public IEnumerable<ContractOfficeSpaceCreateViewModel> OfficeSpaces { get; set; }

        public string BuildingCode { get; set; }

        public string CustomerCode { get; set; }
    }
}
