using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    [Table("PushNotifications")]
    public class PushNotification : Entity
    {
        public Guid OneSignalId { get; set; }

        [MaxLength(80)]
        public string Heading { get; set; }

        [MaxLength(250)]
        public string Content { get; set; }

        public int Converted { get; set; }

        public int CompletedAt { get; set; }

        public PushNotificationDataType DataType { get; set; }

        public PushNotificationReason Reason { get; set; }

        public string Data { get; set; }

        public ICollection<PushNotificationFilter> Filters { get; set; }

        public PushNotification()
        {
            Filters = new HashSet<PushNotificationFilter>();
        }
    }
}
