using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.CalendarItemFrequency;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Presentation.Controllers
{
    public class CalendarController : BaseController
    {
        public readonly ICalendarApplicationService _AppService;
        public readonly IInspectionsApplicationService employeeAppService;
        public readonly ITicketsApplicationService service;

        public CalendarController(
            IEmployeesApplicationService employeeAppService,
            ICalendarApplicationService service,
            IMapper mapper
        ) : base(employeeAppService, mapper)
        {
            _AppService = service;
        }

        [HttpGet]
        [PermissionsFilter("ReadCalendar")]
        public async Task<ActionResult<DataSource<WorkOrderCalendarGridViewModel>>> ReadAll(DataSourceRequestCalendar request)
        {
            try
            {
                var result = await this._AppService.ReadAllActivitiesDapperAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet]
        [PermissionsFilter("ReadCalendar")]
        public async Task<ActionResult<DataSource<WorkOrderCalendarGridViewModel>>> ReadAllWoCalendar(DataSourceRequestCalendar request)
        {
            try
            {
                // to get all of the month
                DateTime baseDate = DateTime.UtcNow;
                if (request.DateFrom.HasValue)
                {
                    baseDate = new DateTime(
                        request.DateFrom.Value.Year,
                        request.DateFrom.Value.Month,
                        request.DateFrom.Value.Day
                        );
                }

                request.DateFrom = new DateTime(baseDate.Year, baseDate.Month, 1);
                request.DateTo = new DateTime(baseDate.Year, baseDate.Month, DateTime.DaysInMonth(baseDate.Year, baseDate.Month));

                var result = await this._AppService.ReadAllActivitiesDapperAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [PermissionsFilter("AddWorkOrderFromCalendar")]
        public async Task<IActionResult> AddCalendarItemFrequency([FromBody] CalendarItemFrequencyCreateViewModel vm)
        {
            if (vm == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }

            try
            {
                var obj = this.Mapper.Map<CalendarItemFrequencyCreateViewModel, CalendarItemFrequency>(vm);
                var result = await this._AppService.AddCalendarItemFrequencyAsync(obj);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }


        }

        [HttpGet]
        public async Task<IActionResult> ReadWorkOrderBySequence(int calendarItemFrequencyId)
        {
            try
            {
                var result = await this._AppService.ReadAllWorkOrderTaskBySequenceId(calendarItemFrequencyId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
