using MGCap.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MGCap.Domain.Utils
{
    public static class WorkOrderUtils
    {
        public static Dictionary<WorkOrderStatus, string> ExternalStatuses = new Dictionary<WorkOrderStatus, string>
        {
            [WorkOrderStatus.Draft] = "Pending",
            [WorkOrderStatus.StandBy] = "Open",
            [WorkOrderStatus.Active] = "Open",
            [WorkOrderStatus.Closed] = "Closed",
            [WorkOrderStatus.Cancelled] = "Cancelled",
        };

        public static Dictionary<string, string> DebugEmails = new Dictionary<string, string>()
        {
            [WorkOrderContactType.BuildingOwner.Key] = "mgcapital.buildingowner@gmail.com",
            [WorkOrderContactType.Requester.Key] = "mgcapital.requester@gmail.com",
            [WorkOrderContactType.PropertyManager.Key] = "mgcapital.propertymanager@gmail.com",
            [WorkOrderContactType.OfficeStaff.Key] = "mgcapital.officestaff@gmail.com",
            [WorkOrderContactType.OperationsManager.Key] = "mgcapital.operationsmanager@gmail.com",
            [WorkOrderContactType.Supervisor.Key] = "mgcapital.supervisor@gmail.com"
        };

        public static WorkOrderContactType[] InternalContacts = new WorkOrderContactType[]
        {
            WorkOrderContactType.OperationsManager,
            WorkOrderContactType.Supervisor,
            WorkOrderContactType.OfficeStaff,
        };

        public static WorkOrderContactType[] ExternalContacts = new WorkOrderContactType[]
        {
            WorkOrderContactType.BuildingOwner,
            WorkOrderContactType.PropertyManager,
            WorkOrderContactType.Requester,
        };

        public static IEnumerable<WorkOrderContactType> Contacts = InternalContacts.Concat(ExternalContacts);
        public static string GetExternalStatusName(WorkOrderStatus status)
        {
            if (ExternalStatuses.ContainsKey(status))
            {
                return ExternalStatuses[status];
            }
            return string.Empty;
        }

        public static string GetDebugEmail(string typeSlug)
        {
            if (DebugEmails.ContainsKey(typeSlug))
            {
                return DebugEmails[typeSlug];
            }
            return "axzesllc@gmail.com";
        }

        public static string GetSubject(
            WorkOrderContactType contactType,
            WorkOrderType woType,
            int woNumber,
            string externalStatus,
            string buildingName)
        {
            // Property Managers case
            if (contactType.Key.Equals(WorkOrderContactType.PropertyManager.Key))
            {
                // Closed
                if (externalStatus == ExternalStatuses[WorkOrderStatus.Closed])
                {
                    return $"{(WorkOrderType)woType} #{woNumber} has been closed";
                }

                // Cancelled
                if (externalStatus == ExternalStatuses[WorkOrderStatus.Cancelled])
                {
                    return $"{(WorkOrderType)woType} #{woNumber} has been cancelled";
                }

                // Opened
                if (externalStatus == ExternalStatuses[WorkOrderStatus.Active])
                {
                    return $"New {(WorkOrderType)woType} for {buildingName}";
                }
            }

            // If is an employee
            if (contactType.Key != WorkOrderContactType.Requester.Key)
            {
                return $"{(WorkOrderType)woType} #{woNumber} [{externalStatus}]";
            }


            // Depending on external status
            if (externalStatus == ExternalStatuses[WorkOrderStatus.Closed])
            {
                return "MG Capital has closed your request!";
            }

            return "MG Capital has been alerted of your request!";
        }
    }
}
