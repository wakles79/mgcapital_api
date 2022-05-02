using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Customer
{
    public class CustomerCsvViewModel
    {
        public string CustomerCode { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int ContactsTotal { get; set; }
    }
}
