using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Contact
{
    public class ContactUpdateViewModel : ContactBaseViewModel
    {
        public Guid Guid { get; set; }

        public int? EntityId { get; set; }

        public string Type { get; set; }

        public bool? Default { get; set; }

        public string ContactType { get; set; }
    }
}
