using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Employee
{
    public class EmployeeGridViewModel : EmployeeBaseViewModel
    {
        public Guid Guid { get; set; }

        public string FullName { get; set; }

        public string DepartmentName { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string RoleName { get; set; }
    }
}
