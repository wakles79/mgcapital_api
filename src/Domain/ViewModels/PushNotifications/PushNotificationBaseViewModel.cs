using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;

namespace MGCap.Domain.ViewModels.PushNotifications
{
    public class PushNotificationBaseViewModel : EntityViewModel
    {
        public Guid OneSignalId { get; set; }

        public string Heading { get; set; }

        public string Content { get; set; }

        public int Completed_At { get; set; }

        public PushNotificationDataType DataType { get; set; }

        public PushNotificationReason Reason { get; set; }

        public string Data { get; set; }
    }
}
