// -----------------------------------------------------------------------
// <copyright file="InspectionsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// ---

using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.InspectionItem;
using MGCap.Domain.ViewModels.InspectionItemTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionsRepository : BaseRepository<Inspection, int>, IInspectionsRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public InspectionsRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        private int NextNumber(int companyId)
        {
            var maxNumber = 0;
            if (this.Entities != null && this.Entities.Count() > 0)
            {
                maxNumber = this.Entities
                                    ?.Where(ent => ent.CompanyId == companyId)
                                    ?.DefaultIfEmpty()
                                    ?.Max(ent => ent.Number) ?? 0;
            }
            return maxNumber + 1;
        }


        public override async Task<Inspection> AddAsync(Inspection obj)
        {
            return await Task.Run(() => { return Add(obj); });
        }

        public override Inspection Add(Inspection obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            obj.Number = this.NextNumber(obj.CompanyId);

            return base.Add(obj);
        }

        public async Task<DataSource<InspectionGridViewModel>> ReadAllDapperAsync(DataSourceRequestInspection request, int companyId, int? status = -1, int? buildingId = null, int? employeeId = null)
        {
            var result = new DataSource<InspectionGridViewModel>
            {
                Payload = new List<InspectionGridViewModel>(),
                Count = 0
            };

            string levelFilter = string.Empty;
            string where = string.Empty;

            if (request.LoggedEmployeId.HasValue)
            {
                levelFilter += @"DECLARE @loggedRoleLevel INT;

                                SELECT @loggedRoleLevel = [dbo].[Roles].Level 
                                FROM [dbo].[Roles] 
                                    INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                                WHERE [dbo].[Employees].Id = @loggedEmployeeId;";
                where += $@" AND (@loggedRoleLevel <=20 OR  I.BuildingId in (select BuildingId from BuildingEmployees where EmployeeId =@loggedEmployeeId))";
            }

            if (buildingId.HasValue)
            {
                where += $" AND I.BuildingId = {buildingId.Value}";
            }

            if (employeeId.HasValue)
            {
                where += $" AND I.EmployeeId = {employeeId.Value}";
            }

            if(request.BeforeSnoozeDate.HasValue)
            {
                where += $" AND (I.SnoozeDate <= @beforeSnoozeDate OR (I.SnoozeDate IS NULL OR I.DueDate IS NOT NULL))";
            }

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID DESC" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                {levelFilter}
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                  SELECT  
                        I.ID,
                        I.CreatedDate,
	                    I.CompanyId,
                        I.Guid,
                        I.Number,
                        I.SnoozeDate,
                        I.BuildingId,
                        I.EmployeeId,
                        I.Stars,
                        I.DueDate,
                        I.CloseDate,
                        I.ClosingNotes,
                        I.BeginNotes,
                        I.Score,
                        I.Status,
                        I.AllowPublicView,
                        B.Name AS BuildingName,
                        CONCAT(C.FirstName, ' ', C.MiddleName,' ', C.LastName) AS EmployeeName,
                        (SELECT COUNT(II.ID) FROM InspectionItems AS II WHERE II.InspectionId = I.ID) AS Items,
                        (SELECT COUNT(II.ID) FROM InspectionItems AS II WHERE II.InspectionId = I.ID AND II.Status = {(int)InspectionItemStatus.Closed}) AS ClosedItems,

                        (SELECT COUNT(IT.ID) FROM InspectionItemTasks AS IT INNER JOIN InspectionItems AS II ON II.ID = IT.InspectionItemId WHERE II.InspectionId = I.ID) AS Tasks,
						(SELECT COUNT(IT.ID) FROM InspectionItemTasks AS IT INNER JOIN InspectionItems AS II ON II.ID = IT.InspectionItemId WHERE II.InspectionId = I.ID AND IT.IsComplete = 1) AS CompletedTasks,
                        (SELECT COUNT(N.ID) FROM InspectionNotes N WHERE N.InspectionId = I.ID) AS NotesCount
                  FROM [dbo].[Inspections] AS I
                    LEFT JOIN [Buildings] B ON B.ID = I.BuildingId
                    LEFT JOIN [Employees] E ON E.ID = I.EmployeeId
                    LEFT JOIN [Contacts] C ON C.ID = E.ContactId
                  WHERE I.CompanyId=@CompanyId
                        AND CONCAT(I.Number,I.ClosingNotes,I.BeginNotes,B.Name,C.FirstName, C.MiddleName, C.LastName) LIKE '%' + ISNULL(@filter, '') + '%'
                        AND I.Status = CASE WHEN @status IS NULL THEN I.Status ELSE @status END {where}
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@filter", request.Filter);
            pars.Add("@status", status == -1 ? null : status);
            pars.Add("@loggedEmployeeId", request.LoggedEmployeId);
            pars.Add("@timezoneOffset", request.TimezoneOffset);

            pars.Add("@beforeSnoozeDate", request.BeforeSnoozeDate);

            var payload = await this._baseDapperRepository.QueryAsync<InspectionGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<InspectionReportDetailViewModel> GetInspectionDetailsDapperAsync(int? inspectionlId, Guid? guid)
        {
            var result = new InspectionReportDetailViewModel();
            var pars = new DynamicParameters();
            string whereQuery = "";

            if (guid.HasValue)
            {
                whereQuery = " i.Guid = @InspectionId";
                pars.Add("@InspectionId", guid);
            }
            else
            {
                whereQuery = " i.ID = @InspectionId";
                pars.Add("@InspectionId", inspectionlId);
            }

            string inspectionQuery = $@"SELECT  
                                            I.ID,
                                            I.CreatedDate,
	                                        I.CompanyId,
                                            I.Guid,
                                            I.Number,
                                            I.SnoozeDate,
                                            I.BuildingId,
                                            I.EmployeeId,
                                            I.Stars,
                                            I.DueDate,
                                            I.CloseDate,
                                            I.ClosingNotes,
                                            I.BeginNotes,
                                            I.Score,
                                            I.Status,
                                            I.AllowPublicView,
                                            B.Name AS BuildingName,
                                            CONCAT(C.FirstName, ' ', C.MiddleName,' ', C.LastName) AS EmployeeName,
                                            E.Email AS EmployeeEmail,
                                            (SELECT TOP 1 CONCAT(P.Ext, P.Phone) FROM ContactPhones P WHERE P.ContactId = C.ID ORDER BY P.[Default]) AS EmployeePhone,
                                            (SELECT COUNT(II.ID) FROM InspectionItems AS II WHERE II.InspectionId = I.ID) AS Items
                                    FROM [dbo].[Inspections] AS I
                                            LEFT JOIN [Buildings] B ON B.ID = I.BuildingId
                                            LEFT JOIN [Employees] E ON E.ID = I.EmployeeId
                                            LEFT JOIN [Contacts] C ON C.ID = E.ContactId
                                    WHERE { whereQuery }";

            var proposalData = await this._baseDapperRepository.QueryAsync<InspectionReportDetailViewModel>(inspectionQuery, pars);
            result = proposalData.FirstOrDefault();


            if (guid.HasValue)
            {
                pars.Add("@InspectionId", result.ID);
            }

            string InspectionQuery = $@"                    
                    SELECT 
                        II.[ID],
                        II.[CreatedDate],
                        --II.[CreatedBy],
                        II.[UpdatedDate],
                        II.[UpdatedBy],
                        II.[Number],
                        II.[Position],
                        II.[Description],
                        II.[Priority],
                        II.[Type],
                        II.[Latitude],
                        II.[Longitude],
                        II.[InspectionId],	
                        II.[Type],
                        CASE	
                            WHEN II.[Status] = {(int)InspectionItemStatus.Closed} THEN II.Status
                            WHEN WO.ID IS NOT NULL AND WO.[StatusId] != {(int)WorkOrderStatus.Closed} THEN WO.[StatusId] + 3
                            WHEN TI.[ID] IS NOT NULL AND (TI.[Status] = {(int)TicketStatus.Undefined} OR TI.[Status] = {(int)TicketStatus.Draft}) THEN {(int)InspectionItemStatus.Ticket}
                            ELSE {(int)InspectionItemStatus.Open}
                        END AS [Status],
                        ISNULL(IA.ID, -1) AS [ID],
						IA.[BlobName],
						IA.[FullUrl],
						IA.[Title],
						IA.[InspectionItemId],
						IA.[ImageTakenDate],
                        ISNULL(TA.ID, -1) AS [Id],
                        TA.InspectionItemId,
                        TA.[IsComplete],
                        TA.[Description],

                        ISNULL(IIN.[ID], -1) AS [ID],
				        IIN.[CreatedDate],
						IIN.[CreatedBy],
					    CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName],
				   	    IIN.[UpdatedDate],
					    IIN.[UpdatedBy],
					    IIN.[InspectionItemId],
						IIN.[EmployeeId],
						IIN.[Note]

                    FROM [dbo].[InspectionItems] AS II
                        LEFT JOIN InspectionItemTickets AS IT ON II.ID = IT.InspectionItemId
                        LEFT JOIN Tickets TI ON TI.ID = IT.TicketId
                        LEFT JOIN WorkOrders AS WO ON IT.entityId = WO.ID AND IT.DestinationType = {(int)TicketDestinationType.WorkOrder}
                        LEFT OUTER JOIN InspectionItemAttachments AS IA ON IA.InspectionItemId = II.ID
                        LEFT OUTER JOIN InspectionItemTasks AS TA ON TA.InspectionItemId = II.ID

						LEFT OUTER JOIN InspectionItemNotes AS IIN ON IIN.InspectionItemId = II.ID

						LEFT OUTER JOIN [dbo].[Employees] ON IIN.[EmployeeId] = [dbo].[Employees].[ID]
	                    LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
                    WHERE II.[InspectionId] = @InspectionId
                        ORDER BY Number";

            //var proposalServicesData = await this._baseDapperRepository.QueryAsync<InspectionItemGridViewModel>(proposalServicesQuery, pars);

            //if (proposalServicesData.Any())
            //    result.InspectionItem = proposalServicesData;

            var items = await _baseDapperRepository.QueryChildrenListAsync<InspectionItemGridViewModel, 
                InspectionItemAttachmentUpdateViewModel,
                InspectionItemTaskUpdateViewModel,
                InspectionItemNoteUpdateViewModel>(InspectionQuery,
                pars,
                System.Data.CommandType.Text);

            if (items.Any())
                result.InspectionItem = items;


            string InspectionNoteQuery = $@"                    
                                   SELECT TOP (1000)
						II.[ID]
                      , II.[CreatedDate]
                      , II.[CreatedBy]
                      ,II.[UpdatedDate]
                      ,II.[UpdatedBy]
                      ,II.[InspectionId]
                      ,II.[EmployeeId]
                      ,II.[Note],
					  CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName]
                  FROM [dbo].[InspectionNotes] II
				  		LEFT OUTER JOIN [dbo].[Employees] ON II.[EmployeeId] = [dbo].[Employees].[ID]
	                    LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
                      WHERE II.[InspectionId] = @InspectionId";


            var items2 = await _baseDapperRepository.QueryAsync<InspectionNoteGridViewModel>(InspectionNoteQuery,
                                                                                             pars,
                                                                                             System.Data.CommandType.Text);

            if (items.Any())
                result.InspectionNote = items2;


            return result;

        }

        public Task<IEnumerable<EmailLogEntry>> GetManagersEmailsAsync(int id)
        {
            var query = @"
                SELECT
                    CONCAT_WS(' ', IEC.FirstName, IEC.LastName) AS [Name],
                    IE.Email AS Email
                FROM Inspections AS I
                    INNER JOIN Employees AS IE ON I.EmployeeId = IE.ID
                    INNER JOIN Contacts AS IEC ON IE.ContactId = IEC.ID
                WHERE 
                    I.ID = @inspectionId

                UNION

                SELECT
                    CONCAT_WS(' ', BEC.FirstName, BEC.LastName) AS [Name],
                    BE.Email AS Email
                FROM Inspections AS I
                    INNER JOIN Buildings AS B ON I.BuildingId = B.ID
                    INNER JOIN BuildingEmployees AS BBE ON BBE.BuildingId = B.ID
                    INNER JOIN Employees AS BE ON BBE.EmployeeId = BE.ID
                    INNER JOIN Contacts AS BEC ON BE.ContactId = BEC.ID
                WHERE I.ID = @inspectionId
            ";

            var pars = new DynamicParameters();
            pars.Add("@inspectionId", id);

            return this._baseDapperRepository.QueryAsync<EmailLogEntry>(query, pars);
        }

        public async Task<DataSource<CalendarGridViewModel>> GetInspectionCalendar(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<CalendarGridViewModel>
            {
                Payload = new List<CalendarGridViewModel>(),
                Count = 0
            };

            string whereStr = string.Empty;

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST(I.[SnoozeDate] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST(I.[SnoozeDate] AS DATE) <= @dateTo ";
            }

            var query = $@"
                    SELECT 
                        I.id, 
                        I.SnoozeDate, 
                          CONCAT('INSPECTION # ', Number,', BUILDING = ' + B.Name, +  ', STATUS: ' +
			              (
							SELECT CASE
								 WHEN upper(i.Status) like 0 THEN
								  'Pending'
								 WHEN upper(i.Status) like 1 THEN
								  'Scheduled'
								 WHEN upper(i.Status) like 2 THEN
								  'Walkthrough'
								WHEN upper(i.Status) like 4 THEN
								  'Active'
								WHEN upper(i.Status) like 5 THEN
								  'Closed'
								 ELSE
								  'unknown'
									END
						  ) + 
                        ', BEGIN NOTES: '+ I.BeginNotes) as Description,
                        {(int)CalendarEventType.Inspection} as Type
                    FROM Inspections I
                        LEFT JOIN [Buildings] B ON B.ID = I.BuildingId
                    WHERE I.CompanyId=@CompanyId 
                        AND I.SnoozeDate IS NOT NULL AND DueDate IS NULL 
                        {whereStr}
            ";
            // AND CAST(I.SnoozeDate AS DATE) >= CAST(GETUTCDATE() AS DATE)
            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);

            var payload = await this._baseDapperRepository.QueryAsync<CalendarGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
