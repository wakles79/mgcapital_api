using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.CustomerEmployee
{
    public class CustomerEmployeeBaseViewModel
    {
        public int EmployeeId { get; set; }

        public int CustomerId { get; set; }

        [MaxLength(128)]
        public string Type { get; set; }


    }
}
