using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractOfficeSpace
{
    public class ContractOfficeSpaceBaseViewModel : EntityViewModel
    {
        public int ContractId { get; set; }

        public int OfficeTypeId { get; set; }

        public double SquareFeet { get; set; }
    }
}
