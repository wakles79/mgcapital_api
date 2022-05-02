using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PreCalendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class PreCalendarRepository : BaseRepository<PreCalendar, int>, IPreCalendarRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public PreCalendarRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
         ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<PreCalendarGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? buildingId = null)
        {
            var result = new DataSource<PreCalendarGridViewModel>
            {
                Payload = new List<PreCalendarGridViewModel>(),
                Count = 0
            };

            string levelFilter = string.Empty;
            string where = string.Empty;

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID DESC" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string whereStr = string.Empty;
            if (buildingId.HasValue)
            {
                whereStr += $" AND PC.BuildingId = {buildingId.Value}";
            }

            if (request.DateFrom.HasValue)
            {                
                request.DateFrom = new DateTime(request.DateFrom.Value.Year, request.DateFrom.Value.Month, 1);
                whereStr += $"  AND CAST(PC.[SnoozeDate] AS DATE) >= @dateFrom";
                // whereStr += $" AND CAST(PC.[SnoozeDate] AS DATE) >= (SELECT DATEADD(s, -1, DATEADD(mm, DATEDIFF(m, 0, @dateFrom) ) + 1, 0))) ";
            }

            if (request.DateTo.HasValue)
            {
                request.DateFrom = request.DateFrom.Value.AddMonths(1).AddDays(-1);
                whereStr += $" AND CAST(PC.[SnoozeDate] AS DATE) <= @dateTo ";
                //whereStr += $" AND CAST(PC.[SnoozeDate] AS DATE) <= (SELECT DATEADD(s, -1, DATEADD(mm, DATEDIFF(m, 0, @DateTo) + 1, 0))) ";
            }

            string query = $@"
                {levelFilter}
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                     SELECT TOP (1000) PC.[ID]
                          ,PC.[CreatedDate]
                          ,PC.[CreatedBy]
                          ,PC.[UpdatedDate]
                          ,PC.[UpdatedBy]
                          ,PC.[CompanyId]
                          ,PC.[Guid]
                          ,PC.[Quantity]
                          ,PC.[Type]
                          ,PC.[SnoozeDate]
                          ,PC.[BuildingId]
                          ,PC.[EmployeeId]
                          ,PC.[Periodicity]
                          ,PC.[Description]
                          ,IIF ( PC.snoozeDate  < (convert(char(6), GETUTCDATE(), 112) + '01'), 'Expired', 'Pending' ) AS Status
                          ,(SELECT BU.Name FROM Buildings as BU WHERE BU.ID =  PC.[BuildingId]) as BuildingName
	                      ,CONCAT(C.FirstName, ' ', C.MiddleName,' ', C.LastName) AS EmployeeName
                      FROM [PreCalendar] PC
                          LEFT JOIN [Employees] E ON E.ID = PC.EmployeeId
                          LEFT JOIN [Contacts] C ON C.ID = E.ContactId    		 
                    WHERE PC.CompanyId = @CompanyId 
                            {whereStr}
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";


            

            var pars = new DynamicParameters();
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@CompanyId", companyId);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@filter", request.Filter);
            pars.Add("@timezoneOffset", request.TimezoneOffset);

            var payload = await this._baseDapperRepository.QueryAsync<PreCalendarGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<PreCalendarDetailViewModel> SingleOrDefaultDapperAsync(int id)
        {
            //string query = @"
            //    SELECT
            //        PC.*,
            //        PCT.*,
            //        S.*
            //    FROM PreCalendar AS PC
            //        LEFT JOIN PreCalendarTasks AS PCT ON PCT.PreCalendarId = PC.ID
            //        LEFT JOIN Services AS S ON PCT.ServiceId = S.ID
            //    WHERE PC.ID = @id
            //";

            string query = @"
					SELECT
						PC.*
					FROM PreCalendar AS PC
					WHERE PC.ID = @id	
            ";


            var pars = new DynamicParameters();
            pars.Add("@id", id);




            var result = new PreCalendarDetailViewModel();

            var Data = await this._baseDapperRepository.QueryAsync<PreCalendarDetailViewModel>(query, pars);
            result = Data.FirstOrDefault();

            string queryPreCaledarTask = @"
                SELECT
                        PCT.*,
					    s.Name AS serviceName
                    FROM PreCalendarTasks AS PCT
                        LEFT JOIN Services AS S ON PCT.ServiceId = S.ID
                    WHERE PCT.PreCalendarId = @id	
            ";

            var items = await _baseDapperRepository.QueryAsync<PreCalendarTaskGridViewModel>(queryPreCaledarTask,
                                                                                 pars,
                                                                                 System.Data.CommandType.Text);

            if (items.Any())
                result.Tasks = items;

            return result;

        }

    }
}
