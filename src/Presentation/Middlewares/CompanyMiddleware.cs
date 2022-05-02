using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MGCap.Business.Abstract.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGCap.Presentation.Filters;
using System.Net;

namespace MGCap.Presentation.Middlewares
{
    public class CompanyMiddleware
    {
        private readonly RequestDelegate _next;

        public CompanyMiddleware(
            RequestDelegate next)
        {
            this._next = next;
        }

        private readonly string[] _allowedPrefixes = new string[]
        {
            "/api/account/login",
            "/api/account/confirmemail",
            "/api/account/resetpassword",
            "/api/account/forgotpassword",
            "/api/account/getuseremail",
            "/api/workorderfrommail/readfromemail",
            "/api/externalworkorders/add",
            "/api/externalworkorders/sendsummarynotifications",
            "/api/workorders/publicget",
            "/api/workorderstatuslog/readall",
            "/api/workorders/readdailyreportbyoperationsmanager",
            "/api/employees/publicget",
            "/api/cleaningreports/publicgetcleaningreport",
            "/api/cleaningreports/addcleaningreportpublicnote",
            "/api/workorderactivitylog/readall",
            "/api/tickets/addexternal",
            "/api/tickets/pending",

            "/api/customeraccount",
            "/api/mobileappversion/latestversion",

            "/api/contracts/publicgetreportdetails",
            "/api/proposals/publicgetreportdetails",
            "/api/inspections/publicgetinspectiondetails",

            "/api/files/uploadattachments",
            "/api/files/deleteattachmentbyblobname",
            "/api/externaltickets/register",
            "/api/externaltickets/registerrequesterresponse"
        };

        // We need a "scoped" dependency, that's why we add empAppService here and not in CTOR
        public async Task InvokeAsync(HttpContext context, IEmployeesApplicationService empAppService)
        {
            var path = context.Request.Path.Value;
            var sPath = path.Split("/");
            var prefix = sPath.Length >= 2 ? sPath[1] : "";

            if (prefix == "api" && !(this._allowedPrefixes.Any(p => path.ToLower().StartsWith(p))))
            {
                if (!context.Request.Headers.Keys.Contains("CompanyId"))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("There is something wrong with request format");
                    return;
                }

                // Adds user permissions to items
                var permissions = await empAppService.GetPermissionsAsync();
                context.Items[PermissionsFilter.PermissionsItemsKey] = permissions;
            }

            // Call the next delegate/middleware in the pipeline
            await _next.Invoke(context);
        }
    }


    #region ExtensionMethod
    public static class CompanyExtension
    {
        public static IApplicationBuilder UseCompanyMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CompanyMiddleware>();
            return app;
        }
    }
    #endregion
}
