using MGCap.Domain.Enums;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Business.Implementation.OneSignal
{
    /// <summary>
    /// This file is located into this project due to its dependancy with OneSignalRestAPIv3 NuGet package
    /// </summary>

    public class PushNotificationSaveViewModel
    {
        public NotificationCreateResult Result { get; set; }

        public NotificationCreateOptions Options { get; set; }

        public PushNotificationDataType DataType { get; set; }

        public PushNotificationReason Reason { get; set; }
    }
}
