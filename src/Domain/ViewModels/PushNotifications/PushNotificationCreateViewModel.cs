using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PushNotifications
{
    public class PushNotificationCreateViewModel
    {
        public WorkOrderEmailDetailsViewModel WorkOrder { get; set; }

        public IList<WorkOrderContactViewModel> Recipients { get; set; }

        public IEnumerable<WorkOrderNotificationTemplate> Templates { get; set; }
    }

    public class PushNotificationTicketCreateViewModel
    {
        public string Requester { get; set; }

        public int TicketNumber { get; set; }

        public string BuildingName { get; set; }

        public ICollection<string> Recipients { get; set; }
    }

    public class PushNotificationInspectionCreateViewModel
    {
        public string BuildingName { get; set; }

        public InspectionStatus InspectionStatus { get; set; }

        public int InspectionNumber { get; set; }

        public ICollection<string> Recipients { get; set; }

        public PushNotificationReason PushNotificationReason
        {
            get
            {
                if (this.InspectionStatus == InspectionStatus.Closed)
                {
                    return PushNotificationReason.Inspection_Closed;
                }
                if (this.InspectionStatus == InspectionStatus.WalkthroughComplete)
                {
                    return PushNotificationReason.Inspection_Walkthrough_Complete;
                }
                return PushNotificationReason.Inspection_Active;
            }
        }
    }
}
