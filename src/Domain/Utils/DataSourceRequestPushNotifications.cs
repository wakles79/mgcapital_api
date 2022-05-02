using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestPushNotifications : DataSourceRequest
    {
        public PushNotificationReason? Reason { get; set; }

        public bool? Unread { get; set; }
    }
}
