using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CustomerUser
{
    public class CustomerContactExistsViewModel
    {
        public int ContactEmailId { get; set; }

        public  int ContactId { get; set; }

        public int CustomerId { get; set; }

        public bool IsValid()
        {
            return ContactEmailId > 0 && ContactId > 0 && CustomerId > 0;
        }
    }
}
