using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Extensions
{
    public static class ControllerExtensions
    {
        public static void LogError(this Controller controller, Exception ex, object pars = null)
        {
            string actionName = controller.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = controller.ControllerContext.RouteData.Values["controller"].ToString();
            var request = controller.ControllerContext.HttpContext.Request;

            var logError = new {
                Url = string.Format("{0}/{1}", controllerName.ToLowerInvariant(), actionName.ToLowerInvariant()),
                Pars = pars ?? request.QueryString.Value
            };

            Log.Error(ex, JsonConvert.SerializeObject(logError));
        }
    }
}
