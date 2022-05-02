using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Freshdesk;
using MGCap.Domain.ViewModels.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Google.Apis.Gmail.v1.Data;
using System.Threading;
using Google.Apis.Auth.OAuth2;
//using Google.Apis.Auth.AspNetCore;
using Google.Apis.Gmail.v1;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Services;

namespace MGCap.Presentation.Controllers
{
    public class ExternalTicketsController : BaseController
    {
        private readonly ITicketsApplicationService _ticketAppService;
        private readonly IOAuth2Service _oAuth2Service;
        protected readonly ICompanySettingsApplicationService _companySettingsService;
        //IGoogleAuthProvider _auth;

        public ExternalTicketsController(
            ITicketsApplicationService ticketAppService,
            IEmployeesApplicationService employeeAppService,
            IOAuth2Service oauth2Service,
            ICompanySettingsApplicationService companySettingsService,
            //IGoogleAuthProvider auth,
            IMapper mapper
            ) : base(employeeAppService, mapper)
        {
            this._ticketAppService = ticketAppService;
            this._oAuth2Service = oauth2Service;
            this._companySettingsService = companySettingsService;
            //this._auth = auth;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] TicketFreshdeskCreateViewModel ticketVm)
        {
            if (!ticketVm.Key.Equals("lHl9lxzm2JPa0HYydHb"))
            {
                return Unauthorized();
            }

            if (!this.ModelState.IsValid || ticketVm == null)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                await this._ticketAppService.AddExternalAsync(ticketVm);
                //await this._ticketAppService.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// It works like Register, but this is only with used with GMail integration
        /// </summary>
        /// <param name="ticketVm"></param>
        /// <returns></returns>
        [HttpPost]
        //[Route("/RegisterGmail")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterGmail([FromBody] GMailRequesterResponseViewModel ticketVm)
        {
            try
            {
                await this._ticketAppService.GMailAddExternalAsync(ticketVm);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterRequesterResponse([FromBody] FreshdeskRequesterResponseViewModel viewModel)
        {
            try
            {
                await this._ticketAppService.RegisterRequesterResponse(viewModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
                // TODO:
            }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> RegisterGMailRequesterResponse([FromBody] GMailRequesterResponseViewModel viewModel)
        //{
        //    try
        //    {
        //        await this._ticketAppService.GMailRegisterRequesterResponse(viewModel);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message);
        //        // TODO:
        //    }
        //}
    }
}
