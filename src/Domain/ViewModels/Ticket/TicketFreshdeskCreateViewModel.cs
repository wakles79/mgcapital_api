using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketFreshdeskCreateViewModel
    {
        public int TicketId { get; set; }

        public string Description { get; set; }

        [Required]
        [MaxLength(200)]
        public string RequesterFullName { get; set; }

        [Required]
        [MaxLength(128)]
        public string RequesterEmail { get; set; }

        public string Key { get; set; }
         
    }
}
