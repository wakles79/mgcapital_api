using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Employee
{
    public class EmployeeDetailsViewModel : EmployeeBaseViewModel
    {
        public Guid Guid { get; set; }

        [MaxLength(80)]
        public string FirstName { get; set; }

        [MaxLength(80)]
        public string MiddleName { get; set; }

        [MaxLength(80)]
        public string LastName { get; set; }

        public string FullName { get; set; }

        [MaxLength(80)]
        public string Salutation { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }

        public DateTime? DOB { get; set; }

        public string Notes { get; set; }

        public bool SendNotifications { get; set; }
    }
}
