using MGCap.Domain.Enums;

namespace MGCap.Domain.Models
{
    public class WorkOrderNotificationTemplate : Entity
    {
        public int WorkOrderContactTypeId { get; set; }

        public int WorkOrderStatusId { get; set; }

        public string RichtextBodyTemplate { get; set; }

        public string PlainTextTemplate { get; set; }

        public string SubjectTemplate { get; set; }

        public NotificationType Type { get; set; }
    }
}
