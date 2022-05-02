using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PushNotifications
{
    public class PushNotificationGridViewModel : PushNotificationBaseViewModel, IGridViewModel
    {
        public bool Unread { get; set; }

        public int Count { get; set; }
    }
}
