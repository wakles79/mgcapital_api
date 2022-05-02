using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.CalendarItemFrequency;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class CalendarApplicationService : ICalendarApplicationService
    {
        private readonly int CompanyId;
        public string UserEmail;

        private readonly IInspectionsRepository InspectionsRepository;
        private readonly ITicketsRepository TicketsRepository;
        private readonly IWorkOrdersRepository WorkOrdersRepository;
        private readonly ICalendarItemFrequenciesRepository CalendarItemFrequenciesRepository;

        public CalendarApplicationService(
            IHttpContextAccessor httpContextAccessor,
            IInspectionsRepository inspectionsRepository,
            ITicketsRepository ticketsRepository,
            IWorkOrdersRepository workOrdersRepository,
            ICalendarItemFrequenciesRepository calendarItemFrequenciesRepository
        )
        {
            this.InspectionsRepository = inspectionsRepository;
            this.TicketsRepository = ticketsRepository;
            this.WorkOrdersRepository = workOrdersRepository;
            this.CalendarItemFrequenciesRepository = calendarItemFrequenciesRepository;

            string companyIdStr = httpContextAccessor?.HttpContext?.Request?.Headers["CompanyId"];
            this.CompanyId = string.IsNullOrEmpty(companyIdStr) ? 1 : int.Parse(companyIdStr);
            this.UserEmail = httpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";
        }

        public async Task<DataSource<WorkOrderCalendarGridViewModel>> ReadAllActivitiesDapperAsync(DataSourceRequestCalendar request)
        {
            var result = await this.WorkOrdersRepository.ReadAllCalendarDapperAsync(request, this.CompanyId);

            //result.Payload = (inspections.Payload).Concat(tickets.Payload);
            //result.Count = inspections.Payload.Count() + tickets.Payload.Count();

            return result;
        }

        #region Calendar Item
        public async Task<CalendarItemFrequencySummaryViewModel> AddCalendarItemFrequencyAsync(CalendarItemFrequency calendarItemFrequency)
        {
            CalendarItemFrequencySummaryViewModel result = new CalendarItemFrequencySummaryViewModel();
            result.AddedDates = new List<DateTime>();

            switch (calendarItemFrequency.Frequency)
            {
                case CalendarFrequency.OneTimeOnly:
                    result.AddedDates = this.GetOneTimeOnlyDates(calendarItemFrequency.StartDate);
                    break;
                case CalendarFrequency.Weekly:
                    result.AddedDates = this.GetWeeklyDates(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity);
                    break;
                case CalendarFrequency.Monthly:
                    result.AddedDates = this.GetDatesByMonthPeriods(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, 1);
                    break;
                case CalendarFrequency.BiMonthly:
                    result.AddedDates = this.GetDatesByMonthPeriods(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, 2);
                    break;
                case CalendarFrequency.Quarterly:
                    result.AddedDates = this.GetDatesByMonthPeriods(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, 3);
                    break;
                case CalendarFrequency.QuarterlyCustom:
                    result.AddedDates = this.GetQuarterlyCustomDates(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, calendarItemFrequency.Months.ToList());
                    break;
                case CalendarFrequency.SemiAnnually:
                    result.AddedDates = this.GetDatesByMonthPeriods(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, 6);
                    break;
                case CalendarFrequency.Annual:
                    result.AddedDates = this.GetDatesByMonthPeriods(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, 12);
                    break;
                case CalendarFrequency.BiAnnually:
                    result.AddedDates = this.GetDatesByYearPeriods(calendarItemFrequency.StartDate, calendarItemFrequency.Quantity, 2);
                    break;
            }

            calendarItemFrequency.BeforeCreate(this.UserEmail, this.CompanyId);
            calendarItemFrequency.CompanyId = this.CompanyId;
            var obj = await this.CalendarItemFrequenciesRepository.AddAsync(calendarItemFrequency);


            await this.CalendarItemFrequenciesRepository.SaveChangesAsync();

            result.ID = obj.ID;
            result.Frequency = obj.Frequency;
            result.ItemType = obj.ItemType;
            result.StartDate = obj.StartDate;
            result.Quantity = obj.Quantity;
            result.Months = obj.Months;

            return result;
        }

        private IEnumerable<DateTime> GetOneTimeOnlyDates(DateTime startDate)
        {
            return new List<DateTime>()
            {
                new DateTime(startDate.Year,startDate.Month,startDate.Day,17,00,00)
            };
        }
        private IEnumerable<DateTime> GetWeeklyDates(DateTime startDate, int quantity)
        {
            var newDates = new List<DateTime>();
            newDates.Add(startDate);
            for (int i = 1; i < quantity; i++)
            {
                DateTime newDate = newDates[newDates.Count - 1].AddDays(7);
                newDates.Add(new DateTime(newDate.Year, newDate.Month, newDate.Day, 17, 00, 00));
            }
            return newDates;
        }
        private IEnumerable<DateTime> GetQuarterlyCustomDates(DateTime startDate, int quantity, List<int> months)
        {
            months.Sort();
            var newDates = new List<DateTime>();
            int year = startDate.Year;
            int day = startDate.Day;

            for (int i = 0; i < months.Count(); i++)
            {
                if (newDates.Count() == quantity)
                {
                    break;
                }

                DateTime newDate = new DateTime(year, months[i], day);
                if (DateTime.Compare(newDate, startDate) >= 0)
                {
                    newDates.Add(new DateTime(newDate.Year, newDate.Month, newDate.Day, 17, 00, 00));
                }

                if (i == (months.Count - 1))
                {
                    i = -1;
                    year++;
                }
            }

            return newDates;
        }
        private IEnumerable<DateTime> GetDatesByMonthPeriods(DateTime startDate, int quantity, int frequency)
        {
            var newDates = new List<DateTime>();
            newDates.Add(startDate);
            for (int i = 1; i < quantity; i++)
            {
                DateTime lastDate = newDates[newDates.Count - 1];
                DateTime newDate = new DateTime(lastDate.AddMonths(frequency).Year, lastDate.AddMonths(frequency).Month, lastDate.Day);
                newDates.Add(new DateTime(newDate.Year, newDate.Month, newDate.Day, 17, 00, 00));
            }
            return newDates;
        }
        private IEnumerable<DateTime> GetDatesByYearPeriods(DateTime startDate, int quantity, int frequency)
        {
            var newDates = new List<DateTime>();
            newDates.Add(startDate);
            for (int i = 1; i < quantity; i++)
            {
                DateTime lastDate = newDates[newDates.Count - 1];
                DateTime newDate = new DateTime(lastDate.AddYears(frequency).Year, lastDate.AddYears(frequency).Month, lastDate.Day);
                newDates.Add(new DateTime(newDate.Year, newDate.Month, newDate.Day, 17, 00, 00));
            }
            return newDates;
        }
        #endregion

        public Task<IEnumerable<WorkOrderTaskSummaryViewModel>> ReadAllWorkOrderTaskBySequenceId(int calendarItemFrequencyId)
        {
            return this.WorkOrdersRepository.ReadAllWorkOrderSequence(calendarItemFrequencyId);
        }
    }
}
