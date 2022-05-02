using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Contact
{
    public class ContactGridViewModel : ContactBaseViewModel
    {
        public Guid Guid { get; set; }

        public string FullName { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string FullAddress { get; set; }

        public string Email { get; set; }
    }
}
