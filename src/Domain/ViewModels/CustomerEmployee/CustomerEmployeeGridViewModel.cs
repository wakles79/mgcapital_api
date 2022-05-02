using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.CustomerEmployee
{
    public class CustomerEmployeeGridViewModel : CustomerEmployeeBaseViewModel
    {
        public string EmployeeName { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
