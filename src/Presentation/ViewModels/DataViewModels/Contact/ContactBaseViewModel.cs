using MGCap.Presentation.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.ViewModels.DataViewModels.Contact
{
    public class ContactBaseViewModel : EntityViewModel
    {
        [MaxLength(80)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(80)]
        public string MiddleName { get; set; }

        [MaxLength(80)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(80)]
        public string Salutation { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }

        public DateTime? DOB { get; set; }

        [MaxLength(80)]
        public string Title { get; set; }

        [MaxLength(80)]
        public string CompanyName { get; set; }

        public string Notes { get; set; }

        public bool SendNotifications { get; set; }
    }
}
