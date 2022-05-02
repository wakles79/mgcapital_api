using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.PushNotifications
{
    public class PushNotificationFilterViewModel : EntityViewModel
    {
        public int PushNotificationId { get; set; }

        public string Key { get; set; }
        
        public string Field { get; set; }
        
        public string Value { get; set; }
        
        public string Relation { get; set; }
    }
}