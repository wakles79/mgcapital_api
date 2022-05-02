using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Customer
{
    public class CustomerDetailsViewModel : CustomerBaseViewModel
    {
        public Guid Guid { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string FullAddress { get; set; }

        public string StatusName { get; set; }

        public DateTime CustomerSince { get; set; }
    }
}
