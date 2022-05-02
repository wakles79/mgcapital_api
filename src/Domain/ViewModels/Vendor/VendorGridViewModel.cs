using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Vendor
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
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
