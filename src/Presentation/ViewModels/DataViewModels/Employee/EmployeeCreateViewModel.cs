using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Employee
{
    public class EmployeeCreateViewModel : EmployeeBaseViewModel
    {
        [MaxLength(80)]
        public string FirstName { get; set; }

        [MaxLength(80)]
        public string MiddleName { get; set; }

        [MaxLength(80)]
        public string LastName { get; set; }

        public string FullName { get; set; }

        [MaxLength(80)]
        public string Salutation { get; set; }

        public string AddressLine1 { get; set; }

        [MaxLength(80)]
        public string AddressLine2 { get; set; }

        [MaxLength(80)]
        public string City { get; set; }

        [MaxLength(80)]
        public string State { get; set; }

        [MaxLength(80)]
        public string ZipCode { get; set; }

        [MaxLength(3)]
        public string CountryCode { get; set; }

        public string AddressType { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string PhoneType { get; set; }

        public bool SendNotifications { get; set; }

    }
}
