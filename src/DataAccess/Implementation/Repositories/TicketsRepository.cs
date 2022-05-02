// -----------------------------------------------------------------------
// <copyright file="TicketsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.InspectionItemTask;
using MGCap.Domain.ViewModels.Tag;
using MGCap.Domain.ViewModels.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Ticket"/>
    /// </summary>
    public class TicketsRepository : BaseRepository<Ticket, int>, ITicketsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TicketsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public TicketsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public override async Task<Ticket> SingleOrDefaultAsync(Func<Ticket, bool> filter)
        {
            return await this.Entities
                    .Include(t => t.Attachments)
                    .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        public async Task<DataSource<TicketGridViewModel>> ReadAllDapperAsync(DataSourceRequestTicket request, int companyId, int? employeeId)
        {
            var result = new DataSource<TicketGridViewModel>()
            {
                Count = 0,
                Payload = new List<TicketGridViewModel>()
            };

            string whereFilter = string.Empty;

            if (request.IsDeleted.HasValue)
            {
                if (request.IsDeleted.Value)
                {
                     whereFilter += " AND T.IsDeleted = 1 ";
                }
                else
                {
                     whereFilter += " AND T.IsDeleted = 0 ";
                }
            }
            else
            {
                 whereFilter += " AND T.IsDeleted = 0 ";
            }

            IEnumerable<int> tags = new List<int>();

            if (request.ShowSnoozed.HasValue)
            {
                if (request.ShowSnoozed == false)
                {
                    whereFilter += " AND (T.[SnoozeDate] <= GETUTCDATE() OR T.[SnoozeDate] IS NULL)";
                }
                else
                {
                    whereFilter += " AND T.[SnoozeDate] > GETUTCDATE()";
                }
            }

            if (request.Source.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.Source IN @sources";
            }

            if (request.Status.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.Status IN @statuses";
            }

            if (request.DestinationType.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.DestinationType IN @destinationTypes";
            }

            if (request.UserType.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.UserType IN @userTypes";
            }

            if (request.UserId.HasValue)
            {
                whereFilter += " AND T.UserId = @userId";
            }

            if (request.OnlyAssigned)
            {
                whereFilter += " AND T.[AssignedEmployeeId] = @employeeId";
            }

            if (request.BuildingId.HasValue)
            {
                whereFilter += " AND T.[BuildingId] = @buildingId";
            }

            if (!string.IsNullOrEmpty(request.StrTags))
            {
                string[] identifier = request.StrTags.Split(',');
                tags = identifier.Select(i => int.Parse(i));
                whereFilter += " AND (SELECT COUNT(TS.ID) FROM TicketTags TS  WHERE TS.TicketId = T.ID AND TS.TagId IN @tags) > 0 ";
            }

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? " UpdatedDate DESC " : this.GetReadAllSorts(request.SortField, request.SortOrder);

            string query = $@"
                SELECT
                    T.[ID],
                    T.[Guid],
                    T.[CompanyId],
                    T.[CreatedDate],
                    T.[UpdatedDate],
                    T.[SnoozeDate],
                    T.Number,
                    T.Source,
                    T.Status,
                    T.DestinationType,
                    T.DestinationEntityId,
                    T.Description,
                    T.FullAddress,
                    T.BuildingId,
                    T.UserId,
                    T.UserType,
                    T.RequesterFullName,
                    T.RequesterEmail,
                    T.RequesterPhone,
                    T.AssignedEmployeeId,
                    CONCAT(C.FirstName, ' ', C.LastName) AS AssignedEmployeeName,
                    T.PendingReview,
                    T.NewRequesterResponse,
                    ISNULL(B.[Name], '') AS BuildingName,
                    (SELECT COUNT(*) FROM [dbo].[TicketAttachments] WHERE [dbo].[TicketAttachments].TicketId = T.[ID] ) as AttachmentsCount,
					ISNULL(G.[ID], -1) AS [ID],
					G.[Description],
					G.[Type],
                    G.[HexColor]
                FROM [dbo].[Tickets] AS T
                    LEFT JOIN [dbo].[Buildings] AS B ON B.ID = T.BuildingId
                    LEFT JOIN [dbo].[Employees] AS [E] ON [E].[ID] = [T].[AssignedEmployeeId]
                    LEFT JOIN [dbo].[Contacts] AS [C] ON [C].[ID] = [E].[ContactId]
					LEFT OUTER JOIN [TicketTags] AS TT ON TT.[TicketId] = T.[ID]
					LEFT OUTER JOIN [Tags] AS G ON G.[ID] = TT.[TagId]
                WHERE T.CompanyId= @companyId {whereFilter} AND
                CONVERT(date, T.CreatedDate) BETWEEN ISNULL(@dateFrom, DATEADD(yy, -10, GETDATE())) AND ISNULL(@dateTo, DATEADD(dd, 1, GETDATE())) AND
                        CONCAT(
                            ISNULL(convert(nvarchar(255), T.Number), ''), 
                            B.[Name],
                            T.Description,
                            T.FullAddress,
                            T.RequesterFullName,
                            T.RequesterEmail,
                            T.RequesterPhone
                        )
                            LIKE '%' + ISNULL(@filter, '') + '%'
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY
            ";

            var statuses = request.Status.GetUniqueFlags();
            var sources = request.Source.GetUniqueFlags();
            var destinationTypes = request.DestinationType.GetUniqueFlags();
            var userTypes = request.UserType.GetUniqueFlags();

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            // Flags params
            pars.Add("@sources", sources);
            pars.Add("@statuses", statuses);
            pars.Add("@destinationTypes", destinationTypes);
            pars.Add("@userTypes", userTypes);
            pars.Add("@userId", request.UserId);
            pars.Add("@employeeId", employeeId);
            pars.Add("@buildingId", request.BuildingId);
            pars.Add("@tags", tags);

            var rows = await this._baseDapperRepository.QueryChildListAsync<TicketGridViewModel, TagBaseViewModel>(query, pars, System.Data.CommandType.Text);
            result.Count = rows.Count();
            result.Payload = rows;
            return result;
        }

        public async Task<TicketDetailsViewModel> GetTicketDetailsDapperAsync(int id = -1, Guid? guid = null)
        {
            // Related with the issue: not loading deleted ticket details on inbox
            //var whereStr = "WHERE T.IsDeleted = 0 ";
            var whereStr = string.Empty;
            var pars = new DynamicParameters();

            //if (id > 0)
            //{
            //    whereStr += " AND T.[ID] = @id ";
            //    pars.Add("@id", id);
            //}
            //else if (guid.HasValue)
            //{
            //    whereStr += " AND T.[Guid] = @guid; ";
            //    pars.Add("@guid", guid.Value);
            //}

            if (id > 0)
            {
                whereStr += " WHERE T.[ID] = @id ";
                pars.Add("@id", id);
            }
            else if (guid.HasValue)
            {
                if (whereStr != string.Empty && whereStr.Trim() != "")
                {
                    whereStr += " AND T.[Guid] = @guid; ";
                    pars.Add("@guid", guid.Value);
                }
                else
                {
                    whereStr += " WHERE T.[Guid] = @guid; ";
                    pars.Add("@guid", guid.Value);
                }
            }

            string entityNumberQuery = $@"
                CASE 
                    WHEN T.[DestinationType] = {(int)TicketDestinationType.WorkOrder} THEN
                        (SELECT WO.Number FROM WorkOrders AS WO WHERE T.[DestinationEntityId] = ID)
                    WHEN T.[DestinationType] = {(int)TicketDestinationType.CleaningItem} OR T.[DestinationType] = {(int)TicketDestinationType.FindingItem} THEN
                        (SELECT CR.Number FROM CleaningReports AS CR INNER JOIN CleaningReportItems AS CRI ON CRI.CleaningReportId = CR.ID WHERE CRI.ID = T.[DestinationEntityId])
                    ELSE
                        NULL
                END as EntityNumber
            ";

            string query = $@"
                SELECT
                     T.[ID],
                     T.[IsDeleted],
                     T.[Guid],
                     T.[CompanyId],
                     T.[CreatedDate],
                     T.[UpdatedDate],
                     T.[SnoozeDate],
                     T.Number,
                     T.Source,
                     T.Status,
                     T.DestinationType,
                     T.DestinationEntityId,
                     T.Description,
                     T.FullAddress,
                     T.BuildingId,
                     T.UserId,
                     T.UserType,
                     T.RequesterFullName,
                     T.RequesterEmail,
                     T.RequesterPhone,
                     T.Data AS StrData,
                     T.FreshdeskTicketId,
                     T.AssignedEmployeeId,
                     T.MessageId,
                     ISNULL(B.[Name], '') AS BuildingName,

					-- INSPECTION ITEM
					IM.[Type],
					IM.[Priority],

                     {entityNumberQuery},

                    -- TICKET ATTACHMENTS (CHILD LIsST)
	                ISNULL(TA.[ID], -1) AS [Id],
	                TA.[TicketId],
	                TA.[BlobName],
	                TA.[FullUrl],
	                TA.[Description],
                       
                    -- TICKET TASKS (CHILD LIST)
                    ISNULL(TS.ID, -1) AS [Id],
                    TS.InspectionItemId,
                    TS.[IsComplete],
                    TS.[Description]
                FROM
                   [dbo].[Tickets] AS T
                       LEFT JOIN [dbo].[Buildings] AS B ON B.ID = T.BuildingId
                       LEFT JOIN [dbo].[TicketAttachments] AS TA ON TA.TicketId = T.ID 
                       LEFT JOIN [dbo].[InspectionItemTickets] AS IT ON IT.TicketId = T.ID
                       LEFT JOIN [dbo].[InspectionItems] AS IM ON IM.ID = IT.InspectionItemId
                       LEFT OUTER JOIN [dbo].[InspectionItemTasks] AS TS ON TS.InspectionItemId = IT.InspectionItemId
                {whereStr}
            ";
            var result = await _baseDapperRepository.QueryChildrenListAsync<TicketDetailsViewModel,
                                                                          TicketAttachmentUpdateViewModel,
                                                                          InspectionItemTaskUpdateViewModel>(query, pars);
            return result.FirstOrDefault();
        }

        private PagedQueryTemplate QueryReadAll(DataSourceRequestTicket request)
        {
            // Possible 
            string whereFilter = " AND T.IsDeleted = 0 ";

            if (request.ShowSnoozed.HasValue)
            {
                if (request.ShowSnoozed == false)
                {
                    whereFilter += " AND (T.[SnoozeDate] <= GETUTCDATE() OR T.[SnoozeDate] IS NULL)";
                }
                else
                {
                    whereFilter += " AND T.[SnoozeDate] > GETUTCDATE()";
                }
            }


            if (request.Source.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.Source IN @sources";
            }

            if (request.Status.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.Status IN @statuses";
            }

            if (request.DestinationType.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.DestinationType IN @destinationTypes";
            }

            if (request.UserType.GetUniqueFlags().Any())
            {
                whereFilter += " AND T.UserType IN @userTypes";
            }

            if (request.UserId.HasValue)
            {
                whereFilter += " AND T.UserId = @userId";
            }

            if (request.OnlyAssigned)
            {
                whereFilter += " AND T.[AssignedEmployeeId] = @employeeId";
            }

            if (request.BuildingId.HasValue)
            {
                whereFilter += " AND T.[BuildingId] = @buildingId";
            }

            string selectFields = $@"
						 T.[ID],
						 T.[Guid],
						 T.[CompanyId],
						 T.[CreatedDate],
						 T.[UpdatedDate],
						 T.[SnoozeDate],
                         T.Number,
                         T.Source,
                         T.Status,
						 T.DestinationType,
						 T.DestinationEntityId,
						 T.Description,
						 T.FullAddress,
						 T.BuildingId,
						 T.UserId,
						 T.UserType,
						 T.RequesterFullName,
						 T.RequesterEmail,
                         T.RequesterPhone,
                         T.AssignedEmployeeId,
                         CONCAT(C.FirstName, ' ', C.LastName) AS AssignedEmployeeName,
                         T.PendingReview,
                         T.NewRequesterResponse,
						 ISNULL(B.[Name], '') AS BuildingName,
                         (SELECT COUNT(*) FROM [dbo].[TicketAttachments] WHERE [dbo].[TicketAttachments].TicketId = T.[ID] ) as AttachmentsCount
						 ";

            string fromTables = @"
                        [dbo].[Tickets] AS T
							LEFT JOIN [dbo].[Buildings] AS B ON B.ID = T.BuildingId
                            LEFT JOIN [dbo].[Employees] AS [E] ON [E].[ID] = [T].[AssignedEmployeeId]
                            LEFT JOIN [dbo].[Contacts] AS [C] ON [C].[ID] = [E].[ContactId]
							";

            string conditions = $@"
                        {whereFilter} AND T.CompanyId= @companyId AND
						CONVERT(date, T.CreatedDate) BETWEEN ISNULL(@dateFrom, DATEADD(yy, -10, GETDATE())) AND ISNULL(@dateTo, DATEADD(dd, 1, GETDATE())) AND
                                CONCAT(
                                    ISNULL(convert(nvarchar(255), T.Number), ''), 
                                    B.[Name],
                                    T.Description,
                                    T.FullAddress,
                                    T.RequesterFullName,
                                    T.RequesterEmail,
                                    T.RequesterPhone
                                )
                                    LIKE '%' + ISNULL(@filter, '') + '%'";


            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : this.GetReadAllSorts(request.SortField, request.SortOrder);

            orders += " T.CreatedDate DESC";

            return new PagedQueryTemplate
            {
                SelectFields = selectFields,
                FromTables = fromTables,
                Conditions = conditions,
                Orders = orders,
                PageNumber = request.PageNumber,
                RowsPerPage = request.PageSize
            };
        }


        private string GetReadAllSorts(string field, string order)
        {
            order = order.ToUpper();

            // HACK: don't wanna use switch statements
            var repl = new Dictionary<string, string>
            {
                ["BuildingName"] = "B.[Name]",
            };


            if (repl.TryGetValue(field, out string replacedField))
            {
                field = replacedField;
            }
            else
            {
                field = $"T.[{field}]";
            }

            return $"{field} {order}";
        }

        public override async Task<Ticket> AddAsync(Ticket obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            //obj.Number = this.GetNextNumber(obj.CompanyId);
            await Entities.AddAsync(obj);
            return obj;
        }

        //public int GetNextNumber(int companyId)
        //{
        //    var maxNumber = 0;
        //    if (this.Entities != null && this.Entities.Count() > 0)
        //    {
        //        maxNumber = this.Entities
        //                            ?.Where(ent => ent.CompanyId == companyId)
        //                            ?.DefaultIfEmpty()
        //                            ?.Max(ent => ent.Number) ?? 0;
        //    }
        //    return maxNumber + 1;
        //}

        public Task<int> GetPendingTicketsCountDapperAsync(int companyId)
        {
            var query = $@"
                SELECT
                    COUNT(T.ID)
                FROM Tickets AS T
                WHERE 
                    T.CompanyId = @companyId 
                    AND T.Status = {(int)TicketStatus.Draft} 
                    AND T.IsDeleted = 0
                    AND (T.[SnoozeDate] <= GETUTCDATE() OR T.[SnoozeDate] IS NULL)
            ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<int>(query, pars);
        }

        public async Task<DataSource<CalendarGridViewModel>> GetTicketCalendar(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<CalendarGridViewModel>
            {
                Payload = new List<CalendarGridViewModel>(),
                Count = 0
            };


            string whereStr = string.Empty;

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST(T.[SnoozeDate] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST(T.[SnoozeDate] AS DATE) <= @dateTo ";
            }


            var query = $@"
                SELECT 
                    T.id, 
                    T.snoozedate, 
                    CONCAT('TICKET # ', T.Number,', BUILDING = ' + B.Name, + ' STATUS: ' +
					(
							SELECT CASE
								 WHEN upper(T.Status) like 0 THEN
								  'Undefined'
								 WHEN upper(T.Status) like 1 THEN
								  'Draft'
								 WHEN upper(T.Status) like 2 THEN
								  'Converted'
								WHEN upper(T.Status) like 4 THEN
								  'Resolved'
								 ELSE
								  'unknown'
									END
					  ) 
                    +', DESCRIPTION: '+ T.Description) as Description,
                    {(int)CalendarEventType.Ticket} as Type
                FROM Tickets  T
                    LEFT JOIN [Buildings] B ON B.ID = T.BuildingId
                WHERE T.CompanyId = @companyId
                    AND T.SnoozeDate IS NOT NULL 
                    {whereStr}
            ";
            // and CAST(T.SnoozeDate AS DATE) >= CAST(GETUTCDATE() AS DATE)
            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);

            var payload = await this._baseDapperRepository.QueryAsync<CalendarGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<TicketToMergeGridViewModel>> ReadAllToMergeAsync(int companyId, int paramType, string value)
        {
            DataSource<TicketToMergeGridViewModel> source = new DataSource<TicketToMergeGridViewModel>()
            {
                Count = 0,
                Payload = new List<TicketToMergeGridViewModel>()
            };

            string strWhere = string.Empty;
            if (paramType == 0)
            {
                strWhere += " AND T.[Number] = @Value";
            }
            else if (paramType == 1)
            {
                strWhere += " AND T.[BuildingId] = @Value ";
            }
            else if (paramType == 2)
            {
                strWhere += " AND T.[RequesterFullName] LIKE '%'+ @Value +'%' ";
            }
            else if (paramType == 3)
            {
                strWhere += " AND T.[RequesterEmail] LIKE '%'+ @Value +'%' ";
            }

            string query = $@"
                SELECT
	                T.[ID],
                    T.[Number],
                    T.[Description],
                    T.[Status]
                FROM [dbo].[Tickets] AS T
                WHERE T.[CompanyId] = @CompanyId AND T.[ParentId] IS NULL {strWhere}
                ORDER BY T.[Number] DESC
            ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@Value", value);

            var rows = await this._baseDapperRepository.QueryAsync<TicketToMergeGridViewModel>(query, parameters);

            if (rows.Any())
            {
                source.Count = rows?.Count() ?? 0;
                source.Payload = rows.AsEnumerable();
            }

            return source;
        }

        /// <summary>
        /// Update by query to prevent [BeforeUpdate] this action only is called on freshdesk event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateStatus"></param>
        /// <returns></returns>
        public async Task<Ticket> UpdateByRequesterResponseAsync(int id, bool updateStatus)
        {
            string updateFields = string.Empty;

            if (updateStatus)
            {
                // [draft] is equals to [pending]
                updateFields += $" , [Status] = {(int) TicketStatus.Draft}, [SnoozeDate] = @date ";
            }

            string query = $"SELECT SnoozeDate FROM [Tickets] WHERE ID=@id ";
            query += $"SET @date = NULL ";
            query += $"UPDATE[dbo].[TIckets] ";
            query += $"SET[NewRequesterResponse] = 1  {updateFields} ";
            query += $"WHERE ID = @id";
            //string query = $"UPDATE [dbo].[TIckets] SET [NewRequesterResponse] = 1 {updateFields} WHERE ID = @id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.Add("@date", DateTime.Now, System.Data.DbType.DateTime);

            var rows = await this._baseDapperRepository.ExecuteAsync(query, parameters);

            return await this.Entities.SingleOrDefaultAsync(t => t.ID == id);
        }

        

        #region MG-15
        public override Ticket Add(Ticket obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            //obj.Number = this.GetNextNumber(obj.CompanyId);

            return base.Add(obj);
        }
        #endregion MG-15

        public void AddDapperAsync(Ticket obj, string userEmail, int companyId)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            obj.ID = _baseDapperRepository.Insert(obj, userEmail, companyId);//.AddAsync(obj);
            //return obj;
        }

        public Task<Ticket> FirstOrDefaultDapperAsync(string threadId, string messageId)
        {
            var query = $@"
                SELECT TOP 1 ID,
                             CreatedDate,
                             CreatedBy,
                             UpdatedDate,
                             UpdatedBy,
                             CompanyId,
                             Guid,
                             Number,
                             Source,
                             Status,
                             DestinationType,
                             DestinationEntityId,
                             Description,
                             FullAddress,
                             BuildingId,
                             UserId,
                             UserType,
                             RequesterFullName,
                             RequesterEmail,
                             RequesterPhone,
                             SnoozeDate,
                             IsDeleted,
                             FreshdeskTicketId,
                             ParentId,
                             AssignedEmployeeId,
                             PendingReview,
                             NewRequesterResponse,
                             HistoryId,
                             MessageId
                FROM Tickets AS T
                WHERE T.MessageId = @threadId
                   OR T.MessageId = @messageId
                ORDER BY T.ID DESC
                ";

            var pars = new DynamicParameters();
            pars.Add("@threadId", threadId);
            pars.Add("@messageId", messageId);
            return this._baseDapperRepository.QuerySingleOrDefaultAsync<Ticket>(query, pars);
        }

        public Task GMailUpdateByRequesterResponseAsync(int id, decimal historyId)
        {
            var query = @"
                UPDATE dbo.Tickets
                SET Status               = @status,
                    HistoryId            = @historyId,
                    SnoozeDate           = null,
                    NewRequesterResponse = 1
                WHERE ID = @id
            ";
            var pars = new DynamicParameters();
            pars.Add("@id", id);
            pars.Add("@status", (int)TicketStatus.Draft);
            pars.Add("@historyId", historyId);

            return this._baseDapperRepository.ExecuteAsync(query, pars);
        }

        public Ticket GetLastGmailHistoryId(int companyId)
        {
            var query = $@"
                SELECT
                    ID, 
                    CreatedDate, 
                    CreatedBy, 
                    UpdatedDate, 
                    UpdatedBy, 
                    CompanyId, 
                    Guid, 
                    Number, 
                    Source, 
                    Status, 
                    DestinationType, 
                    DestinationEntityId, 
                    Description, 
                    FullAddress, 
                    BuildingId, 
                    UserId, 
                    UserType, 
                    RequesterFullName, 
                    RequesterEmail, 
                    RequesterPhone, 
                    SnoozeDate, 
                    IsDeleted, 
                    FreshdeskTicketId, 
                    ParentId, 
                    AssignedEmployeeId, 
                    PendingReview, 
                    NewRequesterResponse, 
                    HistoryId, 
                    MessageId
                FROM
                    Tickets AS T
                WHERE
                    CompanyId=@companyId
                    AND HistoryId=(SELECT MAX(HistoryId)FROM Tickets WHERE CompanyId=@companyId)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@companyId", companyId);
            var result = this._baseDapperRepository.Query<Ticket>(query, parameters);
            if (result != null)
            {
                if (result.Count() > 0)
                    return result.First();
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public Task<int> AddDapperV2Async(Ticket newTicket)
        {
            var query = @"
                DECLARE @generated_keys table
                                        (
                                            [ID] int
                                        );

                INSERT INTO dbo.Tickets (CreatedDate,
                                         CreatedBy,
                                         UpdatedDate,
                                         UpdatedBy,
                                         CompanyId,
                                         Guid,
                                         [Number],
                                         [Source],
                                         Status,
                                         DestinationType,
                                         Description,
                                         FullAddress,
                                         BuildingId,
                                         UserId,
                                         UserType,
                                         RequesterFullName,
                                         RequesterEmail,
                                         RequesterPhone,
                                         SnoozeDate,
                                         FreshdeskTicketId,
                                         ParentId,
                                         AssignedEmployeeId,
                                         HistoryId,
                                         MessageId)
                OUTPUT INSERTED.[Id] INTO @generated_keys
                VALUES (GETUTCDATE(),
                        @createdBy,
                        GETUTCDATE(),
                        @updatedBy,
                        @companyId,
                        NEWID(),
                        0,
                        @source,
                        @status,
                        @destinationType,
                        @description,
                        @fullAddress,
                        @buildingId,
                        @userId,
                        @userType,
                        @requesterFullName,
                        @requesterEmail,
                        @requesterPhone,
                        @snoozeDate,
                        @freshdeskTicketId,
                        @parentId,
                        @assignedEmployeeId,
                        @historyId,
                        @messageId);

                SELECT TOP 1 t.[ID]
                FROM @generated_keys AS g
                        INNER JOIN Tickets AS t
                                    ON g.[ID] = t.[ID]
                WHERE @@ROWCOUNT > 0;

                            ";
            
            var pars = new DynamicParameters();
            pars.Add("@createdBy", newTicket.CreatedBy);
            pars.Add("@updatedBy", newTicket.UpdatedBy);
            pars.Add("@companyId", newTicket.CompanyId);
            pars.Add("@source", (int)newTicket.Source);
            pars.Add("@status", (int)newTicket.Status);
            pars.Add("@destinationType", (int)newTicket.DestinationType);
            pars.Add("@description", newTicket.Description);
            pars.Add("@fullAddress", newTicket.FullAddress);
            pars.Add("@buildingId", newTicket.BuildingId);
            pars.Add("@userId", newTicket.UserId);
            pars.Add("@userType", newTicket.UserType);
            pars.Add("@requesterFullName", newTicket.RequesterFullName);
            pars.Add("@requesterEmail", newTicket.RequesterEmail);
            pars.Add("@requesterPhone", newTicket.RequesterPhone);
            pars.Add("@snoozeDate", newTicket.SnoozeDate);
            pars.Add("@freshdeskTicketId", newTicket.FreshdeskTicketId);
            pars.Add("@parentId", newTicket.ParentId);
            pars.Add("@assignedEmployeeId", newTicket.AssignedEmployeeId);
            pars.Add("@historyId", newTicket.HistoryId);
            pars.Add("@messageId", newTicket.MessageId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<int>(query, pars);
        }
    }
}
