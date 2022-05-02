using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.PushNotifications;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IPushNotificationRepository
    {
        Task<int> InsertPushNotification(PushNotification notification);

        Task<int> InsertPushNotificationFilters(IEnumerable<PushNotificationFilter> filters);

        Task<DataSource<PushNotificationGridViewModel>> ReadAllByUserAsync(DataSourceRequestPushNotifications dataQuery, string userEmail, int companyId);

        Task<bool> MarkAsRead(PushNOtificationMarkAsReadViewModel notification, string userEmail, int companyId);

        /// <summary>
        /// Reads all WO having DueDate in Past Due or Due Today
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<IEnumerable<WorkOrdersByDueDateViewModel>> ReadAllByDueDateAsync(int companyId, int timezoneOffset = 300);

        Task<bool> Exists(string Content, string Data, string Heading, PushNotificationReason Reason);
    }
}
