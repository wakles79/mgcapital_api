using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Vendor
{
    public class VendorGridViewModel : VendorBaseViewModel
    {
        public Guid Guid { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string Email { get; set; }

        public string FullAddress { get; set; }
    }
}
