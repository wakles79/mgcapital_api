using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Freshdesk
{
    public class FreshdeskEntityBaseViewModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
