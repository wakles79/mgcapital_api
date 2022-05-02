using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.PushNotifications;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IPushNotificationService
    {
        Task CreateNotification(PushNotificationCreateViewModel viewModel);

        Task CreateScheduledNotifications(IEnumerable<WorkOrderNotificationTemplate> templates);

        Task<DataSource<PushNotificationGridViewModel>> ReadAllByUserAsync(DataSourceRequestPushNotifications dataQuery);

        Task<bool> MarkAsRead(PushNOtificationMarkAsReadViewModel notification);

        Task CreateNewTicketNotification(PushNotificationTicketCreateViewModel viewModel);

        Task CreateNewInspectionNotification(PushNotificationInspectionCreateViewModel viewModel);
    }
}
