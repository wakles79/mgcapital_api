using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class FreshdeskTicketReplyAttachedFilesViewModel
    {
        public string ReplyModel { get; set; }

        public IFormFileCollection Attachments { get; set; }

        /// <summary>
        /// MgCap Ticket Id
        /// </summary>
        public int TicketId { get; set; }
    }
}
