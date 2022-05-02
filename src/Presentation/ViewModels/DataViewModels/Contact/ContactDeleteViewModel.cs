using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Contact
{
    public class ContactDeleteViewModel
    {
        [Required]
        public int EntityId { get; set; }
        [Required]
        [MinLength(3)]
        public string ContactType { get; set; }
    }
}
