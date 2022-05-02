using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IWorkOrderNotificationsApplicationService
    {
        Task SendNotificationsAsync(WorkOrder wo);

        Task<WorkOrderSummaryViewModel> SendSummaryNotificationsAsync();

        //void SendPushNotification(WorkOrder wo, IEnumerable<string> userEmails);
    }
}
