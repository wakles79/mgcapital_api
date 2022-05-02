using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CustomerUser
{
    public class CustomerUserDetailsViewModel : EntityViewModel
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string FullName => string.Format("{0} {1} {2}", FirstName, MiddleName, LastName).RemoveDuplicatedSpaces();
    }
}
