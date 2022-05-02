using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class AttachmentsBaseViewModel : FreshdeskEntityBaseViewModel
    {
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attachment_url")]
        public string AttachmentUrl { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("GmailId")]
        public string GmailId { get; set; }
    }
}
