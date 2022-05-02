using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum PushNotificationDataType
    {
        WorkOrder = 1,
        Scheduled = 2,
        Ticket = 4,
        Inspection = 8,
    }

    [Flags]
    public enum PushNotificationReason
    {
        Work_Order_Created = 1,
        Work_Order_Updated = 2,
        Work_Order_Closed = 4,
        Work_Orders_Due_Today = 8,
        Work_Orders_Past_Due = 16,
        Ticket_Created = 32,
        Inspection_Walkthrough_Complete = 64,
        Inspection_Active = 128,
        Inspection_Closed = 256,
    }

    [Flags]
    public enum NotificationType
    {
        Email = 1,
        TextMessage = 2,
        PushNotification = 4,
        ScheduledPushNotification = 8
    }
}
