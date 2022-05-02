using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]

    public class WorkOrderFromMailController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IEmailSender _emailSender;

        private readonly IWorkOrderFromMailApplicationService _workOrderFromEmailApplicationService;

        public WorkOrderFromMailController(IMapper mapper, IEmailSender emailSender, 
            IWorkOrderFromMailApplicationService workOrdersFromEmailApplicationService)
        {
            _mapper = mapper;
            _emailSender = emailSender;
            _workOrderFromEmailApplicationService = workOrdersFromEmailApplicationService;
        }

        [HttpGet]
        public async Task<ActionResult<int>> ReadFromEmail()
        {
            try
            {
                int count = await _workOrderFromEmailApplicationService.ReadInbox();
                return new JsonResult(count);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                await _emailSender.SendEmailAsync(
                    "matt@axzes.com",
                    "Exception while reading mgworkorder@gmail.com inbox",
                    plainTextMessage: ex.Message);

                return NoContent();
            }
        }
    }
}
