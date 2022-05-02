using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ToolsController : Controller
    {
        private readonly IToolsApplicationService _toolsApplicationService;
        private readonly IAddressesApplicationService _addressesApplicationService;

        public ToolsController(
            IToolsApplicationService toolsApplicationService,
            IAddressesApplicationService addressesApplicationService
            )
        {
            _toolsApplicationService = toolsApplicationService;
            _addressesApplicationService = addressesApplicationService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Address>>> GeodecodeAddresses()
        {
            var result = await this._addressesApplicationService.GeodecodeAllAsync();
            await this._addressesApplicationService.SaveChangesAsync();
            return new JsonResult(result);
        }

        [HttpGet]
        public ActionResult GoogleStreetViewKey()
        {
            try
            {
                var result = _toolsApplicationService.GoogleStreetViewApiKey();
                return new JsonResult(new { apiKey = result });
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

        [HttpGet]
        public ActionResult<AzureStorageConnectionViewModel> AzureConnection()
        {
            try
            {
                var result = _toolsApplicationService.AzureStorageConnection();
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return NoContent();
            }
        }
    }
}
