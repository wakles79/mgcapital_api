using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketAttachmentCreateViewModel 
    {
        public IFormFile File { get; set; }

        public int TicketId { get; set; }
    }
}
