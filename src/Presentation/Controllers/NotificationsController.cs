using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PushNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class NotificationsController : Controller
    {
        private readonly IPushNotificationService PushNotificationService;

        public NotificationsController(IPushNotificationService pushNotificationService)
        {
            this.PushNotificationService = pushNotificationService;
        }

        [HttpGet]
        public async Task<ActionResult<DataSource<PushNotificationGridViewModel>>> ReadAllByUser(DataSourceRequestPushNotifications datQuety)
        {
            try
            {
                var result = await this.PushNotificationService.ReadAllByUserAsync(datQuety);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> MarkAsRead([FromBody] PushNOtificationMarkAsReadViewModel notification)
        {
            try
            {
                bool result = await this.PushNotificationService.MarkAsRead(notification);
                if (result)
                {
                    return new JsonResult(notification.Guid);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return NoContent();
            }
        }
    }
}