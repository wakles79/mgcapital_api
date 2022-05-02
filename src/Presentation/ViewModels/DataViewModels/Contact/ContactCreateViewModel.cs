using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Contact
{
    public class ContactCreateViewModel : ContactBaseViewModel
    {
        public int? EntityId { get; set; }

        public string Type { get; set; }

        public bool? Default { get; set; }

        public string ContactType { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Ext { get; set; }

    }
}
