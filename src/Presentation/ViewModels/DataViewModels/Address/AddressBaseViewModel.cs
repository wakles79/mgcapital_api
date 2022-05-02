using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Address
{
    public class AddressBaseViewModel
    {
        public int EntityId { get; set; }

        public int AddressId { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public bool Default { get; set; }

        [MaxLength(80)]
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
    }
}
