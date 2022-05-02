using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Domain.ViewModels.Contact
{
    public class ContactAssignViewModel 
    {
        public int ContactId { get; set; }

        public int EntityId { get; set; }

        [MaxLength(80)]
        public string Type { get; set; }

        public bool? Default { get; set; }

        public ContactType ContactType { get; set; }

        public DateTime? ShowHistoryFrom { get; set; }
    }
}
