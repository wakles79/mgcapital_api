using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class FreshdeskAgentBaseViewModel
    {
        [JsonProperty("available")]
        public bool Available { get; set; }

        [JsonProperty("occasional")]
        public bool Occasional { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("ticket_scope")]
        public long TicketScope { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("last_active_at")]
        public DateTimeOffset LastActiveAt { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("contact")]
        public FreshdeskContactBaseViewModel Contact { get; set; }

        [JsonProperty("freshcaller_agent")]
        public bool FreshcallerAgent { get; set; }
    }
}
