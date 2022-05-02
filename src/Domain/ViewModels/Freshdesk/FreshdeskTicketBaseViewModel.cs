using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class FreshdeskTicketBaseViewModel : FreshdeskEntityBaseViewModel
    {
        [JsonProperty("attachments")]
        public IEnumerable<AttachmentsBaseViewModel> Attachments { get; set; }

        [JsonProperty("cc_emails")]
        public IEnumerable<string> CcEmails { get; set; }

        [JsonProperty("company_id")]
        public long? CompanyId { get; set; }

        [JsonProperty("custom_fields")]
        public IDictionary<string, object> CustomFields { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("description_text")]
        public string DescriptionText { get; set; }

        [JsonProperty("due_by")]
        public DateTime DueBy { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_config_id")]
        public long? EmailConfigId { get; set; }

        [JsonProperty("facebook_id")]
        public string FacebookId { get; set; }

        [JsonProperty("fr_due_by")]
        public DateTime FrDueBy { get; set; }

        [JsonProperty("fr_escalated")]
        public bool FrEscalated { get; set; }

        [JsonProperty("fwd_emails")]
        public IEnumerable<string> FwdEmails { get; set; }

        [JsonProperty("group_id")]
        public long? GroupId { get; set; }

        [JsonProperty("is_escalated")]
        public bool IsEscalated { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("priority")]
        public FreshdeskTicketPriority Priority { get; set; }

        public string PriorityName => Priority.ToString().SplitCamelCase();

        [JsonProperty("product_id")]
        public long? ProductId { get; set; }

        [JsonProperty("reply_cc_emails")]
        public IEnumerable<string> ReplyCcEmails { get; set; }

        [JsonProperty("requester_id")]
        public long? RequesterId { get; set; }

        [JsonProperty("responder_id")]
        public long? ResponderId { get; set; }

        [JsonProperty("source ")]
        public FreshdeskTicketSource Source { get; set; }

        public string SourceName => this.Source.ToString().SplitCamelCase();

        [JsonProperty("spam")]
        public bool Spam { get; set; }

        [JsonProperty("status")]
        public FreshdeskTicketStatus Status { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty("to_emails")]
        public IEnumerable<string> ToEmails { get; set; }

        [JsonProperty("twitter_id")]
        public string TwitterId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public ulong HistoryId { get; set; }

        public string MessageId { get; set; }

        public FreshdeskTicketBaseViewModel()
        {
            this.Attachments = new HashSet<AttachmentsBaseViewModel>();
            this.CcEmails = new HashSet<string>();
            this.FwdEmails = new HashSet<string>();
            this.ReplyCcEmails = new HashSet<string>();
            this.Tags = new HashSet<string>();
            this.ToEmails = new HashSet<string>();
            this.CustomFields = new Dictionary<string, object>();
        }
    }
}
