using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Domain.ViewModels.Contact
{
    public class ContactCreateViewModel : ContactBaseViewModel
    {
        public int entityId { get; set; }

        [MaxLength(80)]
        public string Type { get; set; }

        public bool? Default { get; set; }

        public string ContactType { get; set; }

        
    }
}
