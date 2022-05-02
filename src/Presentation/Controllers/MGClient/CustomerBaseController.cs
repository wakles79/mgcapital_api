using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers.MGClient
{

    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class CustomerBaseController : Controller
    {
    }
}
