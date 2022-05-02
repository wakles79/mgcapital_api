using MGCap.Domain.ViewModels.Freshdesk;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketForwardViewModel
    {
        public string StrReply { get; set; }

        public string To { get; set; }

        /// <summary>
        /// MgCapital Ticket Id
        /// </summary>
        public int TicketId { get; set; }

        /// <summary>
        /// GMail Message Id
        /// </summary>
        public string MessageId { get; set; }

        public IFormFileCollection Attachments { get; set; }
    }
}
