using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Enums;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class ConversationBaseViewModel : FreshdeskEntityBaseViewModel
    {
        [JsonProperty("attachments")]
        public IEnumerable<AttachmentsBaseViewModel> Attachments { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("body_text")]
        public string BodyText { get; set; }

        [JsonProperty("incoming")]
        public bool Incoming { get; set; }

        [JsonProperty("to_emails")]
        public IEnumerable<string> ToEmails { get; set; }

        [JsonProperty("private")]
        public bool Private { get; set; }

        [JsonProperty("source")]
        public FreshdeskConversationSource Source { get; set; }

        [JsonProperty("support_email")]
        public string SupportEmail { get; set; }

        [JsonProperty("ticket_id")]
        public long TicketId { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("cc_emails")]
        public IEnumerable<string> CcEmails { get; set; }

        [JsonProperty("bcc_emails")]
        public IEnumerable<string> BccEmails { get; set; }

        #region To work in the view
        public bool FromActivityLog { get; set; }

        public TicketActivityType ActivityType { get; set; } = TicketActivityType.None;

        public string ActivityTypeName { get; set; }

        public IDictionary<string,string> AppCustomFields { get; set; }
        #endregion

        public ConversationBaseViewModel()
        {
            this.Attachments = new HashSet<AttachmentsBaseViewModel>();
            this.ToEmails = new HashSet<string>();
            this.CcEmails = new HashSet<string>();
            this.BccEmails = new HashSet<string>();
            this.AppCustomFields = new Dictionary<string, string>();
        }
    }
}
