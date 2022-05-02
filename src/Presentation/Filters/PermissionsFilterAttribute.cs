using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MGCap.Presentation.Filters
{
    public class PermissionsFilter : ActionFilterAttribute
    {
        private readonly IEnumerable<string> _permissions = new List<string>();

        public static string PermissionsItemsKey = "45d30175db054aeb1c59b357f8d9d03b";

        public PermissionsFilter(params string[] permissions)
        {
            // Forcing lowercase
            this._permissions = permissions.Select(p => p.ToLowerInvariant());
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items.ContainsKey(PermissionsItemsKey))
            {
                if (context.HttpContext.Items[PermissionsItemsKey] is IEnumerable<string> userPermissions && userPermissions.Any())
                {
                    // Forcing lowercase
                    userPermissions = userPermissions.Select(p => p.ToLowerInvariant());
                    // Checks if _permissions is subset of userPermissions
                    bool isSubset = false;
                    foreach ( string permission in this._permissions)
                    {
                        if(userPermissions.Where(p => p == permission).Any())
                        {
                            isSubset = true;
                            break;
                        }
                    }
                    // bool isSubset = !this._permissions.Except(userPermissions).Any();
                    if (!isSubset)
                    {
                        context.Result = new ContentResult
                        {
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Content = "Forbidden"
                        };
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
