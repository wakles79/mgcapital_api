using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using MGCap.Domain.ViewModels.Freshdesk;

namespace MGCap.Domain.ViewModels.GMailApi
{
    public class GMailApiTicketBaseViewModel : GMailApiEntityBaseViewModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string BodyText { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Date { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DeliveredTo { get; set; }

        public string References { get; set; }

        public string InReplyTo { get; set; }

        public string ReplyTo { get; set; }

        public string ThreadTopic { get; set; }

        public string FromName { get; set; }

        public string HeaderMessageID { get; set; }

        public IEnumerable<AttachmentsBaseViewModel> Attachments { get; set; }
    }
}
