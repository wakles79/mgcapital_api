using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MGCap.Domain.ViewModels.DataViewModels.Customer
{
    public class CustomerGridViewModel : CustomerBaseViewModel
    {
        public Guid Guid { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string FullAddress { get; set; }

        public int ContactsTotal { get; set; }
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
