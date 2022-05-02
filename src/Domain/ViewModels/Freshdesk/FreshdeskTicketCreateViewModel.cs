using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using MGCap.Domain.Enums;
using MGCap.Domain.Utils;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class FreshdeskTicketCreateViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("status")]
        public FreshdeskTicketStatus Status { get; set; }

        [JsonProperty("priority")]
        public FreshdeskTicketPriority Priority { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("cc_emails")]
        public IEnumerable<string> CcEmails { get; set; }

        [JsonProperty("custom_fields")]
        public IDictionary<string, string> CustomFields { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }

        public FreshdeskTicketCreateViewModel()
        {
            this.CcEmails = new HashSet<string>();
            this.CustomFields = new Dictionary<string, string>();
        }
    }
}
