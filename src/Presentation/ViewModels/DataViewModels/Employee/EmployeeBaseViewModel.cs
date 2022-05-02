using MGCap.Presentation.ViewModels.Common;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Employee
{
    public class EmployeeBaseViewModel : EntityViewModel
    {
        [MaxLength(128)]
        public string Email { get; set; }

        public int EmployeeStatusId { get; set; }

        public int? DepartmentId { get; set; }

        public int ContactId { get; set; }

    }
}
