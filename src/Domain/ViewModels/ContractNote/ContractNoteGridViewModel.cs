using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.ContractNote
{
    public class ContractNoteGridViewModel : ContractNoteBaseViewModel
    {
        public string EmployeeEmail { get; set; }

        public string EmployeeFullName { get; set; }

        public bool Me { get; set; }
    }
}
