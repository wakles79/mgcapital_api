using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractNote
{
    public class ContractNoteBaseViewModel : AuditableEntityViewModel
    {
        public int ContractId { get; set; }

        public int EmployeeId { get; set; }

        public string Note { get; set; }
    }
}
