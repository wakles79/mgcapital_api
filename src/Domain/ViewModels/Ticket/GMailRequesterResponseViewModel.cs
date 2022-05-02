using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MGCap.Domain.ViewModels.Ticket
{
    [Serializable]
    public class GMailRequesterResponseViewModel
    {
        public GMailPubSubMessage Message { get; set; }

        public string Subscription { get; set; }
    }

    [Serializable]
    public class GMailPubSubMessage
    {
        public string Data { get; set; }

        public string MessageId { get; set; } = string.Empty;

        public DateTime PublishTime { get; set; }

    }

    [Serializable]
    public class MessageData
    {
        public string EMailAddress { get; set; }

        public ulong HistoryId { get; set; }

    }
}
