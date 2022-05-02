using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketCopyFreshdeskImageViewModel
    {
        public int TicketId { get; set; }

        public string Url { get; set; }

        public string FileType { get; set; }

        public string FileName { get; set; }
    }
}
