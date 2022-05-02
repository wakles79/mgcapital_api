using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class TicketReplyViewModel
    {
        /// <summary>
        /// Freshdesk Ticket Id
        /// </summary>
        [JsonProperty("ticket_id")]
        public int TicketId { get; set; }

        [JsonProperty("attachments")]
        public IEnumerable<AttachmentsBaseViewModel> Attachments { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("from_email")]
        public string FromEmail { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("cc_emails")]
        public IEnumerable<string> CcEmails { get; set; }

        [JsonProperty("bcc_emails")]
        public IEnumerable<string> BccEmails { get; set; }

        public TicketReplyViewModel()
        {
            this.BccEmails = new HashSet<string>();
            this.CcEmails = new HashSet<string>();
            this.Attachments = new HashSet<AttachmentsBaseViewModel>();
        }

    }
}
