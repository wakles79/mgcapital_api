using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PushNotifications
{
    public class PushNotificationDetailsViewModel : PushNotificationBaseViewModel
    {
        public int Converted { get; set; }

        public ICollection<PushNotificationFilterViewModel> Filters { get; set; }
    }
}
