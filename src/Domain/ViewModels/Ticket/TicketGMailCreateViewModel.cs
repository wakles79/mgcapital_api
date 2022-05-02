using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketGMailCreateViewModel
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

        public ulong HistoryId { get; set; }

        public string MessageId { get; set; }
    }
}
