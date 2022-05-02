using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Department
{
    public class DepartmentBaseViewModel
    {
        public int ID { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
        
    }
}
