using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.WorkOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class ExternalWorkOrdersController : BaseEntityController<WorkOrder, int>
    {
        public new IWorkOrdersApplicationService AppService => base.AppService as IWorkOrdersApplicationService;
        protected IWorkOrderNotificationsApplicationService NotificationsAppService;
        private readonly IEmailSender EmailSender;
        public ExternalWorkOrdersController(
            IEmployeesApplicationService employeeAppService,
            IWorkOrdersApplicationService appService,
            IWorkOrderNotificationsApplicationService notificationsAppService,
            IEmailSender emailSender,
            IMapper mapper) : base(employeeAppService, appService, mapper)
        {
            this.NotificationsAppService = notificationsAppService;
            this.EmailSender = emailSender;
        }

        /// <summary>
        ///     Sends to operations managers and supervisors the
        ///     status of work orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SendSummaryNotifications()
        {
            WorkOrderSummaryViewModel summary = await this.NotificationsAppService.SendSummaryNotificationsAsync();
            return new JsonResult(summary);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] ExternalWorkOrderViewModel vm)
        {
            //return await this.Add<ExternalWorkOrderViewModel, WorkOrderUpdateViewModel>(vm);
            var receiver = "axzesllc@gmail.com";
            try
            {

                var strPayload = JsonConvert.SerializeObject(vm);
                this.EmailSender.SendEmailAsync(receiver, "MGCapital Create WO Attempt", strPayload);
                if (!this.ModelState.IsValid || vm == null)
                {
                    var strModelState = JsonConvert.SerializeObject(this.ModelState);
                    this.EmailSender.SendEmailAsync(receiver, "MGCapital WO Bad Request", strModelState);
                    return this.BadRequest(this.ModelState);
                }

                var obj = this.Mapper.Map<ExternalWorkOrderViewModel, WorkOrder>(vm);
                obj.Priority = WorkOrderPriority.Low;
                // Gets source from DB and assign source Id to work order
                var source = await this.AppService.GetWOSourceDapperAsync(WorkOrderSourceCode.LandingPage);
                obj.WorkOrderSourceId = source.ID;
                obj = await this.AppService.AddAsync(obj);
                await this.AppService.SaveChangesAsync();
                // Returns full work order details as response
                var result = await this.AppService.GetFullWorkOrderDapperAsync(obj.ID);

                var strResult = JsonConvert.SerializeObject(result);
                this.EmailSender.SendEmailAsync(receiver, "MGCapital WO Success", strResult);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                this.EmailSender.SendEmailAsync(receiver, "MGCapital Exception With WO External API", plainTextMessage: $"EXCEPTION: {ex.Message}", htmlMessage: $"EXCEPTION: <br> {ex.Message}");
                throw ex;
            }

        }
    }
}
