using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Contact
{
    public class ContactDetailsViewModel : ContactBaseViewModel
    {
        public Guid Guid { get; set; }

        public string FullName { get; set; }

        [MaxLength(13)]
        public string Phone { get; set; }

        [MaxLength(13)]
        public string Ext { get; set; }

        public string FullAddress { get; set; }

        public string Email { get; set; }

        public int? EntityId { get; set; }

        public string Type { get; set; }

        public bool? Default { get; set; }

        public string ContactType { get; set; }


    }
}
