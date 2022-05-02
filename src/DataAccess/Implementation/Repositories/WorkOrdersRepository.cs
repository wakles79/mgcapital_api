// -----------------------------------------------------------------------
// <copyright file="WorkOrdersRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderBillingReport;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="WorkOrder"/>
    /// </summary>
    public class WorkOrdersRepository : BaseRepository<WorkOrder, int>, IWorkOrdersRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkOrdersRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public WorkOrdersRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        //private int NextNumber(int companyId)
        //{
        //    string query = @"
        //        SELECT
        //            ISNULL(MAX(W.Number), 0) + 1
        //        FROM 
        //            WorkOrders AS W
        //        WHERE
        //            W.CompanyId = @companyId
        //    ";

        //    var pars = new DynamicParameters();
        //    pars.Add("@companyId", companyId);

        //    return _baseDapperRepository.QuerySingleOrDefault<int>(query, pars);
        //}

        public override async Task<WorkOrder> AddAsync(WorkOrder obj)
        {
            if (obj.Priority == 0)
            {
                obj.Priority = WorkOrderPriority.Low;
            }
            return await Task.Run(() => { return Add(obj); });
        }

        public override WorkOrder Add(WorkOrder obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            //obj.Number = this.NextNumber(obj.CompanyId);
            this.AssignContactAndEmployee(obj);

            // If it's a cloned work order
            // sets last clone number
            if (obj.OriginWorkOrderId != null)
            {
                this.SetCloneNumber(obj);
            }

            return base.Add(obj);
        }

        public Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<WorkOrder> SingleOrDefaultAsync(Func<WorkOrder, bool> filter)
        {
            return await this.Entities
                            .Include(wo => wo.Notes)
                                .ThenInclude(n => n.Employee)
                                    .ThenInclude(e => e.Contact)
                            .Include(wo => wo.Tasks)
                                .ThenInclude(t => t.Service)
                            .Include(wo => wo.Building)
                            .Include(wo => wo.Attachments)
                            .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        public async Task<WorkOrder> SingleOrDefaultDapperAsync(int id)
        {
            string query = @"
                SELECT
                    W.*,
                    WT.*,
                    WN.*,
                    WA.*,
                    S.*,
                    E.*,
                    C.*
                FROM WorkOrders AS W
                    LEFT JOIN WorkOrderTasks AS WT ON WT.WorkOrderId = W.ID
                    LEFT JOIN WorkOrderNotes AS WN ON WN.WorkOrderId = W.ID
                    LEFT JOIN WorkOrderAttachments AS WA ON WA.WorkOrderId = W.ID
                    LEFT JOIN Services AS S ON WT.ServiceId = S.ID
                    LEFT JOIN Employees AS E ON WN.EmployeeId = E.ID
                    LEFT JOIN Contacts AS C ON E.ContactId = C.ID
                WHERE W.ID = @id
            ";

            var pars = new DynamicParameters();
            pars.Add("@id", id);

            WorkOrder result = null;
            List<Service> services = new List<Service>();

            using (var conn = this._baseDapperRepository.GetConnection())
            {
                await conn.QueryAsync<
                    WorkOrder,
                    WorkOrderTask,
                    WorkOrderNote,
                    WorkOrderAttachment,
                    Service,
                    Employee,
                    Contact,
                    WorkOrder>(query, (w, wt, wn, wa, s, e, c) =>
                    {
                        if (result == null)
                        {
                            result = w;
                        }

                        if (wt != null && !result.Tasks.Any(t => t.ID == wt.ID))
                        {
                            result.Tasks.Add(wt);
                        }

                        if (wn != null && !result.Notes.Any(n => n.ID == wn.ID))
                        {
                            if (c != null)
                            {
                                e.Contact = c;
                            }
                            if (e != null)
                            {
                                wn.Employee = e;
                            }
                            result.Notes.Add(wn);
                        }

                        if (wa != null && !result.Attachments.Any(a => a.ID == wa.ID))
                        {
                            result.Attachments.Add(wa);
                        }

                        if (s != null && !services.Any(service => service.ID == s.ID))
                        {
                            services.Add(s);
                        }

                        return w;
                    }, pars);

            }
            if (result != null)
            {
                // Assigns services after
                foreach (var task in result.Tasks)
                {
                    if (task.ServiceId != null)
                    {
                        task.Service = services.FirstOrDefault(s => s.ID == task.ServiceId);
                    }
                }

                // Assigns building after
                result.Building = this.GetBuilding(result.BuildingId);
            }

            this.DbContext.Attach(result);

            return result;

        }

        /// <summary>
        /// Fetches building POCO obj from DB
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public Building GetBuilding(int? buildingId)
        {
            string buildingQuery = "SELECT * FROM Buildings WHERE ID = @buildingId";
            var pars = new DynamicParameters();

            pars.Add("@buildingId", buildingId);

            return this._baseDapperRepository.QuerySingleOrDefault<Building>(buildingQuery, pars);
        }


        /// <summary>
        /// Util to obtain SQL sorts given fields in a
        /// query string
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        private string GetSorts(string sortField, string sortOrder)
        {
            string result = " [dbo].[WorkOrders].[ID] DESC ";
            if (string.IsNullOrEmpty(sortField))
            {
                return result;
            }

            string field = sortField.ToLower();

            var sortDict = new Dictionary<string, string>
            {
                ["status"] = "[dbo].[WorkOrders].[StatusId]",
                ["statusid"] = "[dbo].[WorkOrders].[StatusId]",
                ["statusname"] = "[dbo].[WorkOrders].[StatusId]",
                ["datesubmitted"] = "[dbo].[WorkOrders].[ID]",
                ["duedate"] = "[dbo].[WorkOrders].[DueDate]",
                ["number"] = "[dbo].[WorkOrders].[Number]",
                ["epochduedate"] = "[dbo].[WorkOrders].[DueDate]",
                ["epochdatesubmitted"] = "[dbo].[WorkOrders].[ID]",
                ["buildingname"] = "ISNULL(bldg.[Name], '')",
                ["building"] = "ISNULL(bldg.[Name], '')",
            };

            if (sortDict.ContainsKey(field))
            {
                field = sortDict[field];

                return $"{field} {sortOrder}";
            }

            return result;

        }

        public async Task<DataSource<WorkOrderGridViewModel>> ReadAllDapperAsync(
            DataSourceRequestWOReadAll request,
            int companyId,
            int? administratorId = null,
            int? statusId = null,
            int? buildingId = null,
            int? typeId = null,
            bool unscheduled = false
            )
        {
            var result = new DataSource<WorkOrderGridViewModel>
            {
                Payload = new List<WorkOrderGridViewModel>(),
                Count = 0
            };

            string orders = this.GetSorts(request.SortField, request.SortOrder);

            string clonePathQuery = @"
	            CASE 
		            WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN '' 
		            ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha]([dbo].[WorkOrders].[CloneNumber])) 
	            END
            ";

            string whereStr = $@"
                WHERE [dbo].[WorkOrders].[CompanyId] = @companyId
               AND ([dbo].[WorkOrders].SnoozeDate IS NULL or cast( [dbo].[WorkOrders].SnoozeDate AS date) = cast(  GETUTCDATE() AS date))
                AND CONCAT(
                    LEFT([dbo].[WorkOrders].[Description], 128),
                    [dbo].[WorkOrders].[Number],
                    adminContact.[FirstName], adminContact.[MiddleName], adminContact.[LastName],
                    [dbo].[WorkOrders].[Location],
                    [dbo].[WorkOrders].[FullAddress],
                    {clonePathQuery},
                    bldg.[Name]
                )
                LIKE CONCAT('%', @filter, '%')";

            var rolLevelFilter = string.Empty;
            // extra params
            bool isExpiredStr = request.IsExpired ?? false;
            int employeeId = -1;
            var statusIds = new HashSet<int>();

            if (statusId != null)
            {
                statusIds.Add(statusId.Value);
            }

            if (administratorId != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[AdministratorId] = @administratorId";
            }

            if (buildingId != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[BuildingId] = @buildingId";
            }

            if (request.IsActive != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[IsActive] = @isActive";
            }
            else
            {
                whereStr += " AND [dbo].[WorkOrders].[IsActive] = 1";
            }

            if (request.IsExpired.HasValue)
            {
                whereStr += $" AND [dbo].[WorkOrders].[IsExpired] = @isExpired ";
            }

            if (request.DueToday.HasValue && request.DueToday.Value == true)
            {
                whereStr += $" AND CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE) ";
            }

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) <= @dateTo ";
            }

            if (request.EmployeeId.HasValue)
            {
                employeeId = request.EmployeeId.Value;
                string draftIncludedStr = string.Empty;
                if (string.IsNullOrEmpty(request.Statuses) == false)
                {
                    if (request.Statuses.Contains(((int)WorkOrderStatus.Draft).ToString()))
                    {
                        draftIncludedStr = $" OR [dbo].[WorkOrders].[StatusId] = {(int)WorkOrderStatus.Draft} ";
                    }
                }

                rolLevelFilter = $@" 
                    DECLARE @roleLevel INT;

                    SELECT @roleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = @employeeId;";

                var employeeFilter = EmployeeFilter();

                whereStr += $" AND ({employeeFilter} {draftIncludedStr}) ";
            }

            if (string.IsNullOrEmpty(request.Statuses) == false)
            {
                try
                {
                    var parsedStatuses = request.Statuses.Split('_')?.Select(el => int.Parse(el))?.ToList();
                    statusIds.UnionWith(parsedStatuses);
                }
                catch
                {

                    // TODO: Do something with status ids parsing
                }
            }

            if (statusIds.Any())
            {
                whereStr += $" AND [dbo].[WorkOrders].[StatusId] IN @statusIds ";
            }

            if (typeId != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[Type] = @type";
            }

            if (request.ServiceId.Any())
            {
                whereStr += " AND ISNULL((SELECT COUNT(*) FROM[WorkOrderTasks] AS WT WHERE WT.WorkOrderId = [WorkOrders].ID AND WT.WorkOrderServiceId IN @service), 0) > 0";
            }

            if (request.IsBillable != null)
            {
                whereStr += " AND ISNULL((SELECT COUNT(*) FROM[WorkOrderTasks] AS WT WHERE WT.WorkOrderId = [WorkOrders].ID AND WT.WorkOrderServiceId IS NOT NULL), 0) > 0";
            }

            if (!unscheduled)
            {
                if (statusId.HasValue)
                {
                    if (statusId == (int)WorkOrderStatus.Draft)
                    {
                        whereStr += $" AND  [dbo].[WorkOrders].[Unscheduled] = @unscheduled";
                    }
                    else
                    {
                        whereStr += $" AND [dbo].[WorkOrders].[DueDate] IS NOT NULL AND [dbo].[WorkOrders].[Unscheduled] = @unscheduled";
                    }
                }
                else
                {
                    whereStr += $" AND [dbo].[WorkOrders].[DueDate] IS NOT NULL AND [dbo].[WorkOrders].[Unscheduled] = @unscheduled";
                }
            }
            else
            {
                whereStr += $" AND ([dbo].[WorkOrders].[Unscheduled] = @unscheduled OR ScheduleDate IS NULL)";
            }

            // Gets current logged employee's role level
            // This applies the following rule:
            // "Employees that aren't 'Master' or 'Office Staff' can't never SEE
            // a 'Draft' Work Order"
            if (request.LoggedEmployeId.HasValue)
            {
                rolLevelFilter += $@"
                    DECLARE @loggedRoleLevel INT;

                    SELECT @loggedRoleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = @loggedEmployeeId; 
                    ";

                whereStr += $@" AND (
                                CASE 
                                WHEN [dbo].[WorkOrders].[StatusId] = {(int)WorkOrderStatus.Draft} AND @loggedRoleLevel > {(int)EmployeeRole.Office_Staff} 
                                THEN 0 
                                ELSE 1 END
                            ) = 1";

                whereStr += $@" AND (
                                (@loggedRoleLevel = {(int)EmployeeRole.Master} OR @loggedRoleLevel = {(int)EmployeeRole.Office_Staff})
                                OR (@loggedRoleLevel > {(int)EmployeeRole.Office_Staff}
                                    AND (SELECT COUNT(WE.EmployeeId) FROM [WorkOrderEmployees] AS WE WHERE WE.[WorkOrderId] = [dbo].[WorkOrders].ID AND WE.EmployeeId = @loggedEmployeeId)> 0
                                    )
                            )";
            }

            string fromTables = @"
                FROM [dbo].[WorkOrders]
	                LEFT OUTER JOIN [dbo].[Contacts] as customerContact on customerContact.[ID] = [dbo].[WorkOrders].[CustomerContactId]
	                LEFT OUTER JOIN [dbo].[Employees] as [admin] on [admin].[ID] = [dbo].[WorkOrders].AdministratorId
	                LEFT OUTER JOIN [dbo].[Buildings] as bldg on bldg.[ID] = [dbo].[WorkOrders].[BuildingId]
	                LEFT OUTER JOIN [dbo].[Contacts] as adminContact on  adminContact.[ID] = [admin].ContactId
                    LEFT OUTER JOIN [dbo].[ConvertedTickets] as ConvertedTickets on ConvertedTickets.[DestinationEntityId] = [WorkOrders].id and ConvertedTickets.DestinationType = 1
";

            string query = $@"
                {rolLevelFilter}
            -- payload query
            SELECT 
	            [dbo].[WorkOrders].[ID],
                [dbo].[WorkOrders].[Guid],
                ISNULL([dbo].[WorkOrders].[CalendarItemFrequencyId], 0) AS [CalendarItemFrequencyId],
	            [dbo].[WorkOrders].[CreatedDate] AS DateSubmitted,
	            ISNULL([dbo].[WorkOrders].[AdministratorId], 0) as AdministratorId,
                (SELECT COUNT(W.ID) FROM [dbo].[WorkOrders] AS W WHERE W.[ID] > [dbo].[WorkOrders].[ID] AND W.[WorkOrderScheduleSettingId] = [dbo].[WorkOrders].[WorkOrderScheduleSettingId]) AS [SequencePosition],
                (SELECT COUNT(W.ID) FROM [dbo].[WorkOrders] AS W WHERE W.[WorkOrderScheduleSettingId] = [dbo].[WorkOrders].[WorkOrderScheduleSettingId]) AS [ElementsInSequence],
                ISNULL([dbo].[WorkOrders].[WorkOrderScheduleSettingId], 0) AS [SequenceId],
	            [dbo].[WorkOrders].[DueDate],
	            CASE WHEN [dbo].[WorkOrders].[StatusId] = 0  AND [dbo].[WorkOrders].[Location] is null THEN [dbo].[WorkOrders].[FullAddress] ELSE [dbo].[WorkOrders].[Location] END as [Location],
	            [dbo].[WorkOrders].[RequesterEmail] as RequesterEmail,
	            [dbo].[WorkOrders].[RequesterFullName],
	            [dbo].[WorkOrders].[Number] as Number,
	            ISNULL([dbo].[WorkOrders].[BuildingId], 0) as BuildingId,
	            ISNULL([dbo].[WorkOrders].[StatusId],0 ) as StatusId,
	            [dbo].[WorkOrders].[CompanyId],
	            (SELECT COUNT(*) FROM WorkOrderNotes WHERE WorkOrderNotes.WorkOrderId = [dbo].[WorkOrders].[ID]) as NotesCount,
	            (SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID]) as TasksCount,
                (SELECT COUNT(*) FROM WorkOrderTasks AS WT WHERE WT.WorkOrderId = [dbo].[WorkOrders].[ID] AND WT.[WorkOrderServiceId] IS NOT NULL) as TasksBillableCount,
	            (SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID] AND WorkOrderTasks.IsComplete=1) as TasksDoneCount,
	            (SELECT COUNT(*) FROM WorkOrderAttachments WHERE WorkOrderAttachments.WorkOrderId = [dbo].[WorkOrders].[ID]) as AttachmentsCount,
	            LEFT([dbo].[WorkOrders].[Description], 128) as [Description],
	            CONCAT_WS(' ', adminContact.[FirstName], adminContact.[MiddleName], adminContact.[LastName]) as AdministratorFullName,
	            ISNULL(bldg.[Name], '') as BuildingName,
	            [dbo].[WorkOrders].[IsExpired],
                [dbo].[WorkOrders].[IsActive] as IsActive,
                [dbo].[WorkOrders].[ClosingNotes] as ClosingNotes,
                [dbo].[WorkOrders].[FollowUpOnClosingNotes] as FollowUpOnClosingNotes,
                [ConvertedTickets].[TicketId] as TicketId,

                -- CLONING FIELDS
                [dbo].[WorkOrders].[OriginWorkOrderId],
                {clonePathQuery} AS [ClonePath],
                [dbo].[WorkOrders].[SendRequesterNotifications],
                [dbo].[WorkOrders].[SendPropertyManagersNotifications]

            {fromTables}
            {whereStr}

            ORDER BY {orders}

            OFFSET @pageSize * @pageNumber ROWS
            FETCH NEXT @pageSize ROWS ONLY;";

            var queryCount = $"SELECT COUNT(*) {fromTables} {whereStr};";

            var pars = new DynamicParameters();
            pars.Add("@buildingId", buildingId);
            pars.Add("@administratorId", administratorId);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            pars.Add("@employeeId", employeeId);
            pars.Add("@isExpired", isExpiredStr);
            pars.Add("@statusIds", statusIds);
            pars.Add("@loggedEmployeeId", request.LoggedEmployeId);
            pars.Add("@isActive", request.IsActive);
            pars.Add("@timezoneOffset", request.TimezoneOffset);
            pars.Add("@type", typeId);
            pars.Add("@unscheduled", unscheduled);
            pars.Add("@service", request.ServiceId);
            pars.Add("@IsBillable", request.IsBillable);



            using (var conn = _baseDapperRepository.GetConnection())
            {
                using (var multi = await conn.QueryMultipleAsync($"{query} {queryCount}", pars))
                {
                    var response = multi.Read<WorkOrderGridViewModel>();
                    if (response?.Any() == true)
                    {
                        result.Count = multi.ReadSingleOrDefault<int>();
                        result.Payload = response;
                    }
                }
            }

            return result;
        }

        public async Task<DataSource<WorkOrderGridViewModel>> ReadAllAppDapperAsync(
            DataSourceRequestWOReadAll request,
            int companyId,
            int? administratorId = null,
            int? statusId = null,
            int? buildingId = null,
            int? supervisorId = null,
            int? operationsManagerId = null,
            int? number = null,
            int? typeId = null)
        {
            var result = new DataSource<WorkOrderGridViewModel>
            {
                Payload = new List<WorkOrderGridViewModel>(),
                Count = 0
            };

            string orders = this.GetSorts(request.SortField, request.SortOrder);

            string clonePathQuery = @"
	            CASE 
		            WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN '' 
		            ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha]([dbo].[WorkOrders].[CloneNumber])) 
	            END
            ";

            string whereStr = $@"
                WHERE [dbo].[WorkOrders].[CompanyId] = @companyId
                AND ([dbo].[WorkOrders].SnoozeDate IS NULL or cast( [dbo].[WorkOrders].SnoozeDate AS date) = cast(  GETUTCDATE() AS date))
                AND CONCAT(
                    LEFT([dbo].[WorkOrders].[Description], 128),
                    [dbo].[WorkOrders].[Number],
                    adminContact.[FirstName], adminContact.[MiddleName], adminContact.[LastName],
                    [dbo].[WorkOrders].[Location],
                    [dbo].[WorkOrders].[FullAddress],
                    {clonePathQuery},
                    bldg.[Name]
                )
                LIKE CONCAT('%', @filter, '%')";

            var rolLevelFilter = string.Empty;
            // extra params
            bool isExpiredStr = request.IsExpired ?? false;
            int employeeId = -1;
            var statusIds = new HashSet<int>();

            if (statusId != null)
            {
                statusIds.Add(statusId.Value);
            }

            if (administratorId != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[AdministratorId] = @administratorId";
            }

            if (buildingId != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[BuildingId] = @buildingId";
            }

            if (request.IsActive != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[IsActive] = @isActive";
            }
            else
            {
                whereStr += " AND [dbo].[WorkOrders].[IsActive] = 1";
            }

            if (request.IsExpired.HasValue)
            {
                whereStr += $" AND [dbo].[WorkOrders].[IsExpired] = @isExpired ";
            }

            if (request.DueToday.HasValue && request.DueToday.Value == true)
            {
                whereStr += $" AND CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE) ";
            }

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) <= @dateTo ";
            }

            if (number.HasValue)
            {
                whereStr += $" AND [dbo].[WorkOrders].[Number] = {number} ";
            }

            if (typeId != null)
            {
                whereStr += " AND [dbo].[WorkOrders].[Type] = @type";
            }

            whereStr += $" AND [dbo].[WorkOrders].[DueDate] IS NOT NULL AND [dbo].[WorkOrders].[Unscheduled] = 0 ";

            //Filter by Employee and Rol Level
            // Gets current logged employee's role level
            // This applies the following rule:
            // "Employees that aren't 'Master' or 'Office Staff' can't never SEE
            // a 'Draft' Work Order"
            //Include Draf for loggedRoleLevel > 20
            string draftIncludedStr = string.Empty;
            if (string.IsNullOrEmpty(request.Statuses) == false)
            {
                if (request.Statuses.Contains(((int)WorkOrderStatus.Draft).ToString()))
                {
                    draftIncludedStr = $" OR (dbo.WorkOrders.StatusId = 0 AND @loggedRoleLevel <= {(int)EmployeeRole.Office_Staff})";
                }
            }

            //Declare variables for Employees ID and Roles  
            rolLevelFilter = $@" 
                    DECLARE @roleLevel INT;
                    SELECT @roleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = @employeeId;

                    DECLARE @loggedRoleLevel INT;
                    SELECT @loggedRoleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = @loggedEmployeeId;";

            //By defaul show Employee's Work Order.
            //if SupervisorId or OperationManager are not null them show theirs Owrk Order
            if ((supervisorId != null && supervisorId > 0) || (operationsManagerId != null && operationsManagerId > 0))
            {
                string supervisorQuery = string.Empty;
                string operationsManagerQuery = string.Empty;

                if (supervisorId.HasValue)
                {
                    supervisorQuery = $@" OR ( wo_e.EmployeeId = { supervisorId} and wo_e.Type={(int)WorkOrderEmployeeType.Supervisor})";
                }
                if (operationsManagerId.HasValue)
                {
                    operationsManagerQuery = $@" OR ( wo_e.EmployeeId = { operationsManagerId} and wo_e.Type={(int)WorkOrderEmployeeType.OperationsManager})";
                }

                whereStr += $@" AND (dbo.WorkOrders.ID in (select distinct dbo.WorkOrders.ID from dbo.WorkOrders
                        join WorkOrderEmployees wo_e on
                        ( wo_e.WorkOrderId = dbo.WorkOrders.ID and ( 1 = 2 {supervisorQuery} {operationsManagerQuery} )
                                     
                        )
                    )) ";

            }
            else if (request.EmployeeId.HasValue)
            {
                employeeId = request.EmployeeId.Value;
                whereStr += $@" AND (dbo.WorkOrders.ID in (select distinct dbo.WorkOrders.ID from dbo.WorkOrders
                        join WorkOrderEmployees wo_e on
                        (
                            wo_e.WorkOrderId = dbo.WorkOrders.ID
                            and (
                                    @roleLevel <= {(int)EmployeeRole.Office_Staff} OR wo_e.EmployeeId = @loggedEmployeeId
                                    {draftIncludedStr}
                                )
                        )
                    )) ";
            }

            if (string.IsNullOrEmpty(request.Statuses) == false)
            {
                try
                {
                    var parsedStatusIds = request.Statuses.Split('_')?.Select(el => int.Parse(el))?.ToList();
                    statusIds.UnionWith(parsedStatusIds);
                }
                catch
                {
                }
            }

            if (statusIds.Any())
            {
                whereStr += $" AND [dbo].[WorkOrders].[StatusId] IN @statusIds ";
            }

            string fromTables = @"
                FROM [dbo].[WorkOrders]
	                LEFT OUTER JOIN [dbo].[Contacts] as customerContact on customerContact.[ID] = [dbo].[WorkOrders].[CustomerContactId]
	                LEFT OUTER JOIN [dbo].[Employees] as [admin] on [admin].[ID] = [dbo].[WorkOrders].AdministratorId
	                LEFT OUTER JOIN [dbo].[Buildings] as bldg on bldg.[ID] = [dbo].[WorkOrders].[BuildingId]
	                LEFT OUTER JOIN [dbo].[Contacts] as adminContact on  adminContact.[ID] = [admin].ContactId
            ";

            string query = $@"
                {rolLevelFilter}
            -- payload query
            SELECT 
	            [dbo].[WorkOrders].[ID],
                [dbo].[WorkOrders].[Guid],
	            [dbo].[WorkOrders].[CreatedDate] AS DateSubmitted,
	            ISNULL([dbo].[WorkOrders].[AdministratorId], 0) as AdministratorId,
	            [dbo].[WorkOrders].[DueDate],
	            CASE WHEN [dbo].[WorkOrders].[StatusId] = 0  AND [dbo].[WorkOrders].[Location] is null THEN [dbo].[WorkOrders].[FullAddress] ELSE [dbo].[WorkOrders].[Location] END as [Location],
	            [dbo].[WorkOrders].[RequesterEmail] as RequesterEmail,
	            [dbo].[WorkOrders].[RequesterFullName],
	            [dbo].[WorkOrders].[Number] as Number,
	            ISNULL([dbo].[WorkOrders].[BuildingId], 0) as BuildingId,
	            ISNULL([dbo].[WorkOrders].[StatusId],0 ) as StatusId,
	            [dbo].[WorkOrders].[CompanyId],
	            (SELECT COUNT(*) FROM WorkOrderNotes WHERE WorkOrderNotes.WorkOrderId = [dbo].[WorkOrders].[ID]) as NotesCount,
	            (SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID]) as TasksCount,
                (SELECT COUNT(*) FROM WorkOrderTasks AS WT INNER JOIN Services AS Serv ON WT.ServiceId = Serv.ID WHERE WT.WorkOrderId = [dbo].[WorkOrders].[ID]) as TasksBillableCount,
	            (SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID] AND WorkOrderTasks.IsComplete=1) as TasksDoneCount,
	            (SELECT COUNT(*) FROM WorkOrderAttachments WHERE WorkOrderAttachments.WorkOrderId = [dbo].[WorkOrders].[ID]) as AttachmentsCount,
	            LEFT([dbo].[WorkOrders].[Description], 128) as [Description],
	            CONCAT_WS(' ', adminContact.[FirstName], adminContact.[MiddleName], adminContact.[LastName]) as AdministratorFullName,
	            ISNULL(bldg.[Name], '') as BuildingName,
	            [dbo].[WorkOrders].[IsExpired],
                [dbo].[WorkOrders].[IsActive] as IsActive,
                [dbo].[WorkOrders].[ClosingNotes] as ClosingNotes,
                [dbo].[WorkOrders].[FollowUpOnClosingNotes] as FollowUpOnClosingNotes,

                -- CLONING FIELDS
                [dbo].[WorkOrders].[OriginWorkOrderId],
                {clonePathQuery} AS [ClonePath],
                [dbo].[WorkOrders].[SendRequesterNotifications],
                [dbo].[WorkOrders].[SendPropertyManagersNotifications]

            {fromTables}
            {whereStr}

            ORDER BY {orders}

            OFFSET @pageSize * @pageNumber ROWS
            FETCH NEXT @pageSize ROWS ONLY;";

            var queryCount = $"SELECT COUNT(*) {fromTables} {whereStr};";

            var pars = new DynamicParameters();
            pars.Add("@buildingId", buildingId);
            pars.Add("@administratorId", administratorId);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            pars.Add("@employeeId", employeeId);
            pars.Add("@isExpired", isExpiredStr);
            pars.Add("@statusIds", statusIds);
            pars.Add("@loggedEmployeeId", request.LoggedEmployeId);
            pars.Add("@isActive", request.IsActive);
            pars.Add("@timezoneOffset", request.TimezoneOffset);
            pars.Add("@type", typeId);

            using (var conn = _baseDapperRepository.GetConnection())
            {
                using (var multi = await conn.QueryMultipleAsync($"{query} {queryCount}", pars))
                {
                    var response = multi.Read<WorkOrderGridViewModel>();
                    if (response?.Any() == true)
                    {
                        result.Count = multi.ReadSingleOrDefault<int>();
                        result.Payload = response;
                    }
                }
            }

            return result;
        }

        public override async Task<WorkOrder> UpdateAsync(WorkOrder obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            this.AssignContactAndEmployee(obj);

            await Task.Factory.StartNew(() =>
            {
                this.Entities.Update(obj);
            });

            return obj;
        }

        public override WorkOrder Update(WorkOrder obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }


            this.AssignContactAndEmployee(obj);
            this.Entities.Update(obj);

            return obj;
        }

        private void AssignContactAndEmployee(WorkOrder obj)
        {
            var buildingId = obj.BuildingId;
            if (buildingId == null)
            {
                return;
            }

            // Selected ids of contacts type "Property Manager"associated to the building 
            string queryPM = @"
                SELECT 
                    BC.ContactId as PropertyManagersId
                FROM 
                    [dbo].[Buildings] as B 
                    INNER JOIN [dbo].[BuildingContacts] AS BC ON B.ID = BC.BuildingId AND B.ID = @buildingId
                WHERE 
                    BC.[Type] = 'Property Manager' ";
            var pars = new DynamicParameters();
            pars.Add("@buildingId", buildingId);

            // Add the list of property managers Id to the corresponding property like a string
            var result2 = _baseDapperRepository.Query<int>(queryPM, pars);
            if (result2 != null)
            {
                obj.PropertyManagersId = string.Join(",", result2.Select(r => r.ToString()));
            }
        }

        private void SetCloneNumber(WorkOrder obj)
        {
            var referenceId = obj.OriginWorkOrderId;
            if (referenceId == null)
            {
                return;
            }

            string query = @"
                SELECT
                    ISNULL(MAX(W.CloneNumber), 0) + 1
                FROM 
                    WorkOrders AS W
                WHERE
                    W.OriginWorkOrderId = @referenceId
            ";

            var pars = new DynamicParameters();
            pars.Add("@referenceId", referenceId);

            var result = _baseDapperRepository.QuerySingleOrDefault<int>(query, pars);

            obj.CloneNumber = result;
        }

        public override async Task RemoveAsync(WorkOrder obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!this.Exists(obj.ID))
            {
                throw new ArgumentException("The given object does not exist in DB", nameof(obj));
            }
            obj.IsActive = false;
            await base.UpdateAsync(obj);
        }

        /// <summary>
        /// Returns a full WorkOrderSource List.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>WorkOrderSource List</returns>
        public async Task<DataSource<WOSourcesListBoxViewModel>> ReadAllWOSourceCboDapperAsync(DataSourceRequest request)
        {
            var result = new DataSource<WOSourcesListBoxViewModel>
            {
                Payload = new List<WOSourcesListBoxViewModel>(),
                Count = 0
            };

            string query = $@"
                    -- payload query
                    SELECT [ID],
                           [Name],
                           [Code]
                    FROM [dbo].[WorkOrderSources]";

            var payload = await _baseDapperRepository.QueryAsync<WOSourcesListBoxViewModel>(query, null);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<WorkOrderContactViewModel>> GetWOContactsDapperAsync(WorkOrder wo)
        {
            var query = $@"

                -- Office Staff
                SELECT 
                    CONCAT(eContact.FirstName, ' ', eContact.LastName) AS FullName,
                    E.Email AS Email,
                    'Office Staff' AS Type,
                    eContact.SendNotifications AS SendNotifications
                FROM Employees AS E
					INNER JOIN Roles AS R ON E.RoleId = R.ID
                    INNER JOIN Contacts AS eContact ON eContact.ID = E.ContactId
                WHERE E.CompanyId = @companyId AND R.[Level] <= 20 -- Only Office Staff or higher
						AND eContact.SendNotifications = 1

                UNION

                -- Building Owner
                SELECT 
                    CONCAT(cContact.FirstName, ' ', cContact.LastName) AS FullName,
                    cContactEmail.Email AS Email,
                    'Building Owner' AS Type,
                    cContact.SendNotifications AS SendNotifications
                FROM Buildings AS B                     
                    INNER JOIN [Customers] AS C ON C.[ID] = B.[CustomerId]
                    INNER JOIN CustomerContacts AS CC ON C.ID = CC.CustomerId AND CC.[Default] = 1
                    INNER JOIN Contacts AS cContact ON cContact.ID = CC.ContactId
                    INNER JOIN ContactEmails AS cContactEmail ON cContactEmail.ContactId = cContact.ID
                WHERE B.ID = @buildingId AND cContact.SendNotifications = 1

                UNION

                -- Operations Manager, Supervisor, Subcontractor Operations Manager
                SELECT 
                    CONCAT(C.FirstName, ' ', C.LastName) AS FullName,
                    E.Email AS Email,
                    CASE
                        WHEN WE.[Type] = 0 THEN ''
                        WHEN WE.[Type] = 1 THEN 'Supervisor'
                        WHEN WE.[Type] = 2 THEN 'Operations Manager'
                        WHEN WE.[Type] = 4 THEN 'Operations Manager'
                        WHEN WE.[Type] = 8 THEN ''
                    END AS [Type],
                    C.SendNotifications AS SendNotifications
                FROM [WorkOrderEmployees] AS [WE]
                    INNER JOIN [Employees] AS E ON WE.[EmployeeId] = E.[ID]
                    INNER JOIN [Contacts] AS C ON E.[ContactId] = C.[ID]
                WHERE [WE].WorkOrderId = @WorkOrderId AND WE.[Type] IN ({(int)WorkOrderEmployeeType.Supervisor},{(int)WorkOrderEmployeeType.OperationsManager},{(int)WorkOrderEmployeeType.TemporaryOperationsManager})

                UNION
			
                -- Property Manager
                SELECT 
                    CONCAT(bContact.FirstName, ' ', bContact.LastName) AS FullName,
                    bContactEmail.Email AS Email,
                    'Property Manager' AS Type,
                    bContact.SendNotifications AS SendNotifications
                FROM Buildings AS B 
                    INNER JOIN BuildingContacts AS BC ON BC.BuildingId = B.ID AND BC.Type = 'Property Manager'
                    INNER JOIN Contacts AS bContact ON bContact.ID = BC.ContactId
                    INNER JOIN ContactEmails AS bContactEmail ON bContactEmail.ContactId = bContact.ID
                WHERE B.ID = @buildingId AND bContact.SendNotifications = 1
            ";

            var pars = new DynamicParameters();
            pars.Add("@buildingId", wo.BuildingId);
            pars.Add("@companyId", wo.CompanyId);
            pars.Add("@WorkOrderId", wo.ID);

            var result = await _baseDapperRepository.QueryAsync<WorkOrderContactViewModel>(query, pars);

            // HACK: Appending Requester from WorkOrder POCO entity
            result = result.Append(new WorkOrderContactViewModel
            {
                FullName = wo.RequesterFullName,
                Type = "Requester",
                Email = wo.RequesterEmail,
                SendNotifications = true
            });
            return result;
        }

        public async Task<WorkOrderEmailDetailsViewModel> GetWODetailsDapperAsync(WorkOrder wo, int companyId, string userEmail)
        {
            // HACK: If WO is added
            if (wo.ID <= 0)
            {
                return new WorkOrderEmailDetailsViewModel
                {
                    RequesterEmail = wo.RequesterEmail,
                    RequesterFullName = wo.RequesterFullName,
                    Description = wo.Description,
                    StatusId = wo.StatusId,
                    WONumber = wo.Number,
                    Guid = wo.Guid,
                };
            }

            var query = $@"
                SELECT 
                    WO.ID,
                    WO.Guid,
                    WO.Number AS WONumber,
                    WO.StatusId,
                    WO.Description,
                    B.Name AS BuildingName,
                    WO.RequesterFullName,
                    WO.RequesterEmail,
	                (
		                SELECT TOP 1 
			                CONCAT(C.[FirstName], ' ', C.[LastName]) 
		                FROM 
			                Employees AS E
			                INNER JOIN Contacts AS C ON E.ContactId = C.ID
		                WHERE 
			                E.Email = @email AND E.CompanyId = @companyId
	                ) AS EmployeeWhoClosedWO

                FROM WorkOrders AS WO
                    LEFT JOIN Buildings AS B ON WO.BuildingId = B.ID
                WHERE WO.ID = @workOrderId
            ";

            var pars = new DynamicParameters();
            pars.Add("@workOrderId", wo.ID);
            pars.Add("@companyId", companyId);
            pars.Add("@email", userEmail);

            var result = await _baseDapperRepository.QuerySingleOrDefaultAsync<WorkOrderEmailDetailsViewModel>(query, pars);

            return result;
        }

        public async Task<DataSource<WorkOrderDashboardViewModel>> GetDashboardDataDapperAsync(
            int companyId,
            int timezoneOffset = 300,
            int? employeeId = null)
        {
            var result = new DataSource<WorkOrderDashboardViewModel>
            {
                Payload = new List<WorkOrderDashboardViewModel>()
            };
            string whereStr = " [dbo].[WorkOrders].IsActive = 1 AND ";
            whereStr += $" [dbo].[WorkOrders].[DueDate] IS NOT NULL AND";
            var rolLevelFilter = string.Empty;

            rolLevelFilter = $@" 
                    DECLARE @roleLevel INT;

                    SELECT @roleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = {employeeId} ";

            var employeeFilter = EmployeeFilter();

            whereStr += $" {employeeFilter} ";

            string query = $@"
                            {rolLevelFilter}                            

                            SELECT * FROM 
                                (
	                                SELECT 
                                        (
                                            SELECT 
                                                COUNT([Id]) AS 'Quantity-DueTodayTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                            WHERE 
                                                CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE)
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND [dbo].[WorkOrders].StatusId IN ({(int)WorkOrderStatus.StandBy}, {(int)WorkOrderStatus.Active})
                                                AND {whereStr}
                                        ) AS 'quantity', 
                                    
		                                (
                                            SELECT 
                                                COUNT([Id]) AS 'Footer-CompletedTodayTotal'
		                                     FROM 
                                                [dbo].[WorkOrders]
		                                     WHERE 
                                                CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE)
                                                AND [dbo].[WorkOrders].StatusId = {(int)WorkOrderStatus.Closed} 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'FooterValue', 
		                                0 AS 'criteria'
		                            
                                    UNION ALL

                                    SELECT  
                                        (
                                            SELECT 
                                                COUNT([Id]) AS 'Quantity-OverdueTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                            WHERE [dbo].[WorkOrders].IsExpired = 1 
                                                AND [dbo].[WorkOrders].StatusId IN ({(int)WorkOrderStatus.StandBy}, {(int)WorkOrderStatus.Active}) 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'quantity', 

                                        (
                                            SELECT 
                                                COUNT([Id]) AS 'Footer-YesterdayOverdueTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                            WHERE 
                                                [dbo].[WorkOrders].IsExpired = 1 
                                                AND [dbo].[WorkOrders].StatusId IN ({(int)WorkOrderStatus.StandBy}, {(int)WorkOrderStatus.Active}) 
                                                AND CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset - 1436, GETUTCDATE()) AS DATE) 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'FooterValue', 
                                        1 AS 'criteria'

                                    UNION ALL
                                        
                                    SELECT 
                                        (
                                            SELECT 
                                                COUNT([Id]) AS 'Quantity-StandByTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                            WHERE 
                                                [dbo].[WorkOrders].StatusId = {(int)WorkOrderStatus.StandBy} 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'quantity', 
                                        
                                        (
                                            SELECT 
                                                COUNT([dbo].[WorkOrders].[Id]) AS 'Footer-AssignedThisWeekTotal'
                                            FROM [dbo].[WorkOrders]
                                                LEFT OUTER JOIN [dbo].WorkOrderStatusLog AS L ON L.WorkOrderId = [dbo].[WorkOrders].ID
                                            WHERE 
                                                L.StatusId = {(int)WorkOrderStatus.StandBy} 
                                                AND CAST(L.CreatedDate AS DATE) >= DATEADD(d, 1 - DATEPART(DW, GETUTCDATE()), CAST(GETUTCDATE() AS DATE)) 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr} 
                                        ) AS 'FooterValue', 
                                        2 As 'criteria'
                                    
                                    UNION ALL

                                    SELECT 
                                        (
                                            SELECT 
                                                COUNT([Id]) AS 'Quantity-OpenTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                            WHERE 
                                                [dbo].[WorkOrders].StatusId != {(int)WorkOrderStatus.Closed} and [dbo].[WorkOrders].StatusId != {(int)WorkOrderStatus.Cancelled} and [dbo].[WorkOrders].StatusId != {(int)WorkOrderStatus.Draft}  and [dbo].[WorkOrders].StatusId != {(int)WorkOrderStatus.StandBy} 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'quantity', 

                                        (
                                            SELECT 
                                                COUNT([dbo].[WorkOrders].[Id]) AS 'Footer-ClosedTodayTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                                LEFT OUTER JOIN [dbo].WorkOrderStatusLog AS L ON L.WorkOrderId = [dbo].[WorkOrders].ID
                                            WHERE 
                                                [dbo].[WorkOrders].StatusId = {(int)WorkOrderStatus.Closed} 
                                                AND CAST(L.CreatedDate AS DATE) = CAST(GETUTCDATE() AS DATE) 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'FooterValue', 
                                        3 AS 'criteria'

                                    UNION ALL

                                    SELECT
                                        (
                                            SELECT 
                                                COUNT([Id]) AS 'Quantity-DraftTotal'
                                            FROM 
                                                [dbo].[WorkOrders]
                                            WHERE 
                                                [dbo].[WorkOrders].StatusId = {(int)WorkOrderStatus.Draft} 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'quantity', 

                                        (
                                            SELECT 
                                                COUNT([dbo].[WorkOrders].[Id]) as 'Footer-DraftThisWeekTotal'
                                            FROM [dbo].[WorkOrders]
                                                LEFT OUTER JOIN [dbo].WorkOrderStatusLog AS L ON L.WorkOrderId = [dbo].[WorkOrders].ID
                                            WHERE 
                                                L.StatusId = {(int)WorkOrderStatus.Draft} 
                                                AND CAST(L.CreatedDate AS DATE) >= DATEADD(d, 1 - DATEPART(DW, GETUTCDATE()), CAST(GETUTCDATE() AS DATE)) 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'FooterValue', 
                                        4 AS 'criteria'

                                    UNION ALL

                                    SELECT 
                                        0 AS 'quantity', 
                                        (
                                            SELECT 
                                                COUNT([dbo].[WorkOrders].[Id]) AS 'FooterValue-CreatedThisWeekTotal'
                                            FROM [dbo].[WorkOrders]
                                                LEFT OUTER JOIN [dbo].WorkOrderStatusLog AS L ON L.WorkOrderId = [dbo].[WorkOrders].ID
                                            WHERE 
                                                CAST(L.CreatedDate AS DATE) >= DATEADD(d, 1 - DATEPART(DW, GETUTCDATE()), CAST(GETUTCDATE() AS DATE)) 
                                                AND [dbo].[WorkOrders].CompanyId = @companyId 
                                                AND {whereStr}
                                        ) AS 'FooterValue', 
                                        5 AS 'criteria'

                                ) AS result";

            var pars = new DynamicParameters();
            pars.Add("@employeeId", employeeId);
            pars.Add("@companyId", companyId);
            pars.Add("@timezoneOffset", timezoneOffset);

            var payload = await _baseDapperRepository.QueryAsync<WorkOrderDashboardViewModel>(query, pars);
            result.Payload = payload;
            return result;
        }

        public Task<IEnumerable<int>> GetEmployeesIdsDapperAsync(int workOrderId, WorkOrderEmployeeType type = WorkOrderEmployeeType.Any)
        {
            var filter = "WHERE [dbo].[WorkOrderEmployees].[WorkOrderId]= @workOrderid";
            var types = type.GetUniqueFlags();
            if (types.Any())
            {
                filter += " AND [dbo].[WorkOrderEmployees].[Type] IN @types";
            }

            string query = $@"
                    SELECT EmployeeId FROM [dbo].[WorkOrderEmployees]
                    {filter}
            ";

            var pars = new DynamicParameters();
            pars.Add("@workOrderId", workOrderId);
            pars.Add("@types", types);

            return this._baseDapperRepository.QueryAsync<int>(query, pars);
        }

        public async Task<WorkOrderUpdateViewModel> GetFullWorkOrderDapperAsync(int workOrderId = -1, Guid? workOrderGuid = null)
        {
            var pars = new DynamicParameters();
            string whereStr = string.Empty;

            if (workOrderId > 0)
            {
                whereStr = " AND [dbo].[WorkOrders].[ID] = @workOrderId ";
                pars.Add("@workOrderId", workOrderId);
            }
            else if (workOrderGuid.HasValue)
            {
                whereStr = " AND [dbo].[WorkOrders].[GUID] = @workOrderGuid ";
                pars.Add("@workOrderGuid", workOrderGuid.Value);
            }
            else
            {
                throw new ArgumentException();
            }

            var query = string.Format(@"
                SELECT 
	                -- WO BASE VIEW MDDEL FIELDS                        
	                [dbo].[WorkOrders].[ID],
                    [dbo].[WorkOrders].[Guid],
	                [dbo].[WorkOrders].[BuildingId],
	                [dbo].[WorkOrders].[Location],
	                [dbo].[WorkOrders].[AdministratorId],
	                [dbo].[WorkOrders].[Priority],
	                [dbo].[WorkOrders].[SendRequesterNotifications],
	                [dbo].[WorkOrders].[SendPropertyManagersNotifications],
	                [dbo].[WorkOrders].[StatusId],
	                [dbo].[WorkOrders].[Number],
	                [dbo].[WorkOrders].[Description],
	                [dbo].[WorkOrders].[DueDate],
	                [dbo].[WorkOrders].[Type],
	                [dbo].[WorkOrders].[WorkOrderSourceId],
	                [dbo].[WorkOrders].[BillingName],
	                [dbo].[WorkOrders].[BillingEmail],
	                [dbo].[WorkOrders].[BillingNote],
                    [dbo].[WorkOrders].[ClosingNotes],
                    [dbo].[WorkOrders].[ClosingNotesOther],
                    [dbo].[WorkOrders].[FollowUpOnClosingNotes],
                    [dbo].[WorkOrders].[AdditionalNotes],
                    [dbo].[WorkOrders].[OriginWorkOrderId],
                    [dbo].[WorkOrders].[SnoozeDate],
                    [dbo].[WorkOrders].[BillingDateType],
                    [dbo].[WorkOrders].[ClientApproved],
                    [dbo].[WorkOrders].[ScheduleCategoryId],
                    [dbo].[WorkOrders].[ScheduleDate],
                    [dbo].[WorkOrders].[ScheduleDateConfirmed],
                    [dbo].[WorkOrders].[ScheduleSubCategoryId],
                    [dbo].[WorkOrders].[Unscheduled],
                    [dbo].[WorkOrders].[WorkOrderScheduleSettingId],

	                -- CLONING FIELDS
                    CASE 
		                WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN '' 
		                ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha]([dbo].[WorkOrders].[CloneNumber])) 
	                END AS [ClonePath],
                    CASE
                       WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN ''
                       ELSE (SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId])
	                END AS [OriginWorkOrderNumber], 

	                -- WO REQUESTER VIEW MODEL FIELDS
	                [dbo].[WorkOrders].[FullAddress],
	                [dbo].[WorkOrders].[RequesterFullName],
	                [dbo].[WorkOrders].[RequesterEmail],

	                -- WO UPDATE VIEW MODEL FIELDS
	                [dbo].[Buildings].[Name] AS [BuildingName],
	                [dbo].[WorkOrders].[CreatedDate],
	                [dbo].[WorkOrders].[IsExpired],
	                [dbo].[WorkOrders].[ClosingNotes],

	                -- WO NOTES (CHILD LIsST)
	                ISNULL([dbo].[WorkOrderNotes].[ID], -1) AS [Id],
	                [dbo].[WorkOrderNotes].[WorkOrderId],
	                [dbo].[WorkOrderNotes].[Note],
	                [dbo].[WorkOrderNotes].[EmployeeId],
	                [dbo].[WorkOrderNotes].[CreatedDate],
	                [dbo].[WorkOrderNotes].[UpdatedDate],
	                [dbo].[Employees].[Email] AS [EmployeeEmail],
	                CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName],

	                -- WO ATTACHMENTS (CHILD LIsST)
	                ISNULL([dbo].[WorkOrderAttachments].[ID], -1) AS [Id],
	                [dbo].[WorkOrderAttachments].[WorkOrderId],
	                [dbo].[WorkOrderAttachments].[EmployeeId],
	                [dbo].[WorkOrderAttachments].[BlobName],
	                [dbo].[WorkOrderAttachments].[FullUrl],
	                [dbo].[WorkOrderAttachments].[Description],
	                [dbo].[WorkOrderAttachments].[ImageTakenDate]
                FROM
	                [dbo].[WorkOrders]
	                LEFT OUTER JOIN [dbo].[Buildings] ON [dbo].[WorkOrders].[BuildingId] = [dbo].[Buildings].[ID]
	                LEFT OUTER JOIN [dbo].[WorkOrderTasks] ON [dbo].[WorkOrders].[ID] = [dbo].[WorkOrderTasks].[WorkOrderId]
	                LEFT OUTER JOIN [dbo].[Services] ON [dbo].[WorkOrderTasks].[ServiceId] = [dbo].[Services].[ID]
	                LEFT OUTER JOIN [dbo].[WorkOrderNotes] ON [dbo].[WorkOrders].[ID] = [dbo].[WorkOrderNotes].[WorkOrderId]
	                LEFT OUTER JOIN [dbo].[Employees] ON [dbo].[WorkOrderNotes].[EmployeeId] = [dbo].[Employees].[ID]
	                LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
	                LEFT OUTER JOIN [dbo].[WorkOrderAttachments] ON [dbo].[WorkOrders].[ID] = [dbo].[WorkOrderAttachments].[WorkOrderId]
                WHERE 
	                1 = 1 {0} ", whereStr);

            var result = await _baseDapperRepository.QueryChildrenListAsync<WorkOrderUpdateViewModel,
                                                                            WorkOrderNoteUpdateViewModel,
                                                                            WorkOrderAttachmentUpdateViewModel>(query, pars);
            return result.FirstOrDefault();
        }

        protected string EmployeeFilter()
        {
            return string.Format(@"
                    (
                        CASE 
                        WHEN @roleLevel <= {0} THEN 1 
                        ELSE 
                            (
                                SELECT COUNT(Q.EID) 
                                FROM
                                    (SELECT @employeeID AS EID 
                                        INTERSECT 
                                    (SELECT wo_e.EmployeeId AS EID FROM WorkOrderEmployees AS wo_e INNER JOIN WorkOrders ON wo_e.WorkOrderId = [dbo].[WorkOrders].[ID]))
                                AS Q
                            )
                        END
                    ) > 0", (int)EmployeeRole.Office_Staff);
        }

        // AT BOTTOM TO AVOID MERGE CONFLICTS
        public Task<WorkOrderSource> GetWOSourceDapperAsync(WorkOrderSourceCode code)
        {
            string query = @"
                SELECT  [ID]
                      ,[Name]
                      ,[Code]
                 FROM [dbo].[WorkOrderSources]
                 WHERE [Code] = @code
            ";
            var pars = new DynamicParameters();
            pars.Add("@code", (int)code);
            return this._baseDapperRepository.QuerySingleOrDefaultAsync<WorkOrderSource>(query, pars);
        }

        public async Task<DataSource<WorkOrderDailyReportViewModel>> DailyReportByOperationsManagerDapperAsync(DataSourceRequestWOReadAll request, int companyId, int? operationsManagerId = null)
        {
            var result = new DataSource<WorkOrderDailyReportViewModel>
            {
                Payload = new List<WorkOrderDailyReportViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string whereStr = $@" WHERE [woEmployees].[Type] = {(int)WorkOrderEmployeeType.OperationsManager} AND 
                                  [woEmployees].EmployeeId = CASE WHEN @operationsManagerId IS NULL THEN [woEmployees].EmployeeId ELSE @operationsManagerId END AND
                                  [dbo].[WorkOrders].[IsActive] = 1";
            whereStr += $" AND [dbo].[WorkOrders].[DueDate] IS NOT NULL ";

            // extra params
            bool isExpiredStr = request.IsExpired ?? false;
            var statusIds = new HashSet<int>();

            if (request.IsExpired.HasValue)
            {
                whereStr += $" AND [dbo].[WorkOrders].[IsExpired] = @isExpired ";
            }

            if (request.DueToday.HasValue && request.DueToday.Value == true)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) = CAST(GETUTCDATE() AS DATE) ";
            }

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST([dbo].[WorkOrders].[DueDate] AS DATE) <= @dateTo ";
            }

            // Considering both "statuses as string" and "statusIds" as query string
            if (string.IsNullOrEmpty(request.Statuses) == false)
            {
                try
                {
                    var parsedStatuses = request.Statuses.Split('_')?.Select(el => int.Parse(el))?.ToList();

                    statusIds.UnionWith(parsedStatuses);
                }
                catch (Exception ex)
                {

                    // TODO: Do something with status ids parsing
                    //throw ex;
                }
            }

            if (request.StatusIds?.Any() == true)
            {
                statusIds.UnionWith(request.StatusIds);
            }

            if (statusIds.Any())
            {
                whereStr += $" AND [dbo].[WorkOrders].[StatusId] IN @statusIds ";
            }

            if (request.BuildingIds?.Any() == true)
            {
                whereStr += $" AND [dbo].[WorkOrders].[BuildingId] IN @buildingIds ";
            }

            string query = $@"
                    -- payload query
                SELECT *, [Count] = COUNT(*) OVER() 
                FROM (
	                SELECT 
		            [dbo].[WorkOrders].[ID],
                    [dbo].[WorkOrders].[Guid],
		            [dbo].[WorkOrders].[CreatedDate] AS DateSubmitted,
		            ISNULL([dbo].[WorkOrders].[AdministratorId], 0) as AdministratorId,
		            [dbo].[WorkOrders].[DueDate],
		            CASE WHEN [dbo].[WorkOrders].[StatusId] = 0  AND [dbo].[WorkOrders].[Location] is null THEN [dbo].[WorkOrders].[FullAddress] ELSE [dbo].[WorkOrders].[Location] END as [Location],
		            ISNULL([dbo].[WorkOrders].[RequesterEmail], '') as RequesterEmail,
		            ISNULL([dbo].[WorkOrders].[RequesterFullName], '') as RequesterFullName,
		            [dbo].[WorkOrders].[Number] as Number,
		            ISNULL([dbo].[WorkOrders].[BuildingId], 0) as BuildingId,
		            ISNULL([dbo].[WorkOrders].[StatusId],0 ) as StatusId,
		            [dbo].[WorkOrders].[CompanyId],
		            (SELECT COUNT(*) FROM WorkOrderNotes WHERE WorkOrderNotes.WorkOrderId = [dbo].[WorkOrders].[ID]) as NotesCount,
		            (SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID]) as TasksCount,
		            (SELECT COUNT(*) FROM WorkOrderTasks WHERE WorkOrderTasks.WorkOrderId = [dbo].[WorkOrders].[ID] AND WorkOrderTasks.IsComplete=1) as TasksDoneCount,
		            (SELECT COUNT(*) FROM WorkOrderAttachments WHERE WorkOrderAttachments.WorkOrderId = [dbo].[WorkOrders].[ID]) as AttachmentsCount,
		            LEFT([dbo].[WorkOrders].[Description], 128) as [Description],
		            CONCAT(operationsManagerContact.[FirstName]+' ',operationsManagerContact.[MiddleName]+' ',operationsManagerContact.[LastName]+' ') as OperationsManagerFullName,
		            ISNULL(bldg.[Name], '') as BuildingName,
		            [dbo].[WorkOrders].[IsExpired],

                    -- CLONING FIELDS
                    [dbo].[WorkOrders].[OriginWorkOrderId],
	                CASE 
			            WHEN ISNULL([dbo].[WorkOrders].[OriginWorkOrderId], 0) = 0 THEN '' 
		                ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = [dbo].[WorkOrders].[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha]([dbo].[WorkOrders].[CloneNumber])) 
	                END AS [ClonePath]

	                FROM [dbo].[WorkOrders]
		            LEFT OUTER JOIN [dbo].[Contacts] as customerContact on customerContact.[ID] = [dbo].[WorkOrders].[CustomerContactId]
		            LEFT OUTER JOIN [dbo].[Buildings] as bldg on bldg.[ID] = [dbo].[WorkOrders].[BuildingId]
		            LEFT OUTER JOIN [dbo].WorkOrderEmployees as [woEmployees] on woEmployees.WorkOrderId = [dbo].[WorkOrders].ID
		            LEFT OUTER JOIN [dbo].[Employees] as [employee] on [employee].[ID] = [woEmployees].EmployeeId
		            LEFT OUTER JOIN [dbo].[Contacts] as operationsManagerContact on  operationsManagerContact.[ID] = [employee].ContactId
				            
                    {whereStr} 

                ) payload 
                WHERE CompanyId = @companyId                           
                          AND ISNULL([Description], '') + 
                          CAST(Number AS nvarchar(50)) +
                          RequesterFullName + 
                          [Location] + 
                          BuildingName
                                LIKE '%' + ISNULL(@filter, '') + '%'
                    ORDER BY {orders} BuildingId, [Location], DateSubmitted DESC, ID 
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@operationsManagerId", operationsManagerId);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            pars.Add("@isExpired", isExpiredStr);
            pars.Add("@statusIds", statusIds);
            pars.Add("@buildingIds", request.BuildingIds);

            var payload = await _baseDapperRepository.QueryAsync<WorkOrderDailyReportViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<WorkOrderWithExpirationViewModel>> GetWorkOrderWithExpirationDapperAsync(int companyId, int timezoneOffset = 300)
        {

            var query = string.Format(@"
                SELECT w.[ID], 
	                   w.[Guid],	
	                   w.[StatusId],
	                   w.[DueDate], 
	                   w.[IsExpired],
	                   CASE 
			                WHEN CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE) THEN 1 ELSE 0 
	                   END AS DueToday,
	                   w.[Number],
	                   CASE 
			                WHEN ISNULL(w.[OriginWorkOrderId], 0) = 0 THEN '' 
		                    ELSE CONCAT((SELECT [WO].[Number] FROM [dbo].[WorkOrders] AS WO WHERE [WO].[ID] = w.[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha](w.[CloneNumber])) 
	                   END AS [ClonePath],
	                   b.[ID] as BuildingId,
	                   b.[Name] as BuildingName

                FROM 
	                WorkOrders AS w 
	                INNER JOIN Buildings AS b ON b.ID = w.BuildingId
                WHERE 
                    w.IsActive = 1 AND
	                (StatusId = 1 OR StatusId = 2) AND
	                (IsExpired = 1 OR CAST(DueDate AS DATE) = CAST(GETUTCDATE() AS DATE)) AND w.CompanyId = @companyId
                     AND w.[DueDate] IS NOT NULL");

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@timezoneOffset", timezoneOffset);
            var result = await _baseDapperRepository.QueryAsync<WorkOrderWithExpirationViewModel>(query, pars);

            var buildingIds = result?.Select(w => w.BuildingId);

            // Safety check
            if (buildingIds != null && buildingIds.Any())
            {
                var employeesQuery = @"
                    SELECT EmployeeId, BuildingId, [Type] FROM
                    BuildingEmployees WHERE BuildingId IN @buildingIds
                ";

                var employeePars = new DynamicParameters();
                employeePars.Add("@buildingIds", buildingIds);

                // Gets all building-employees that belongs a any building
                var buildingEmployees = await _baseDapperRepository.QueryAsync<BuildingEmployee>(employeesQuery, employeePars);

                // Assigns EmployeeId to any work order
                foreach (var wo in result)
                {
                    foreach (var be in buildingEmployees)
                    {
                        if (wo.BuildingId == be.BuildingId)
                        {
                            wo.EmployeeIds.Add(be.EmployeeId);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<WorkOrderEmployeeContactViewModel>> GetSupervisorsAndOperationsManagersDapperAsync(int companyId)
        {
            string query = $@"
                -- Operations Manager
                SELECT 
                    CONCAT(omContact.FirstName, ' ', omContact.LastName) AS FullName,
                    E.Email AS Email,
                    E.ID AS EmployeeId,
                    'Operations Manager' AS Type
                FROM Buildings AS B 
                    INNER JOIN BuildingEmployees AS BE ON BE.BuildingId = B.ID AND BE.Type = {(int)BuildingEmployeeType.OperationsManager}
                    INNER JOIN Employees AS E ON BE.EmployeeId = E.ID 
                    INNER JOIN Contacts AS omContact ON omContact.ID = E.ContactId
                WHERE omContact.SendNotifications = 1 AND E.CompanyId = @companyId

                UNION

                -- Supervisor
                SELECT 
                    CONCAT(sContact.FirstName, ' ', sContact.LastName) AS FullName,
                    E.Email AS Email,
                    E.ID AS EmployeeId,
                    'Supervisor' AS Type
                FROM Buildings AS B 
					INNER JOIN BuildingEmployees AS BE ON BE.BuildingId = B.ID AND BE.Type = {(int)BuildingEmployeeType.Supervisor}
                    INNER JOIN Employees AS E ON BE.EmployeeId = E.ID 
                    INNER JOIN Contacts AS sContact ON sContact.ID = E.ContactId
                WHERE sContact.SendNotifications = 1 AND E.CompanyId = @companyId
            ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            var result = await _baseDapperRepository.QueryAsync<WorkOrderEmployeeContactViewModel>(query, pars);

            return result;

        }

        public Task<DataSource<WorkOrderBillingReportGridViewModel>> ReadBillingReportDapperAsync(DataSourceRequestBillingReport request, int companyId)
        {
            PagedQueryTemplate queryTemplate = this.QueryBillingReport(request);

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            pars.Add("@customerId", request.CustomerId ?? null);
            pars.Add("@timezoneOffset", request.TimezoneOffset);
            pars.Add("@buildingIds", request.BuildingIds);

            return this._baseDapperRepository.QueryPagedAsync<WorkOrderBillingReportGridViewModel>(queryTemplate, pars);
        }

        private PagedQueryTemplate QueryBillingReport(DataSourceRequestBillingReport request)
        {
            string additionalJoin = string.Empty;

            string logDate = $@"SELECT TOP 1 L.[CreatedDate] FROM WorkOrderStatusLog AS L 
                        WHERE L.[StatusId] = {(int)WorkOrderStatus.Closed} AND 
                            L.[WorkOrderId] = WO.[ID] 
                        ORDER BY L.[CreatedDate] DESC";

            string selectFields = $@"
						 T.[ID],
                         T.[Description] AS TaskName,
                         T.[Note] AS BillingNote,
                         T.[GeneralNote] AS TaskNote,
                         CASE 
                             WHEN WS.[ID] IS NOT NULL THEN WS.[Name]
                             WHEN TS.[ID] IS NOT NULL THEN TS.[Name]
                         END AS ServiceName,
                         T.[CreatedDate] AS TaskCreatedDate,    
                         T.[IsComplete] AS IsTaskChecked,    
                         ({logDate}) AS WorkOrderCompletedDate,
                         T.[LastCheckedDate] AS CompletedDate,
                         T.[DiscountPercentage],
                         T.[Quantity],
		                 ISNULL(B.[Name], '') as BuildingName,
                         ISNULL(B.[Code], '') as BuildingCode,
                         CASE 
                             WHEN WS.[ID] IS NOT NULL THEN WS.[UnitFactor]
                             WHEN TS.[ID] IS NOT NULL THEN TS.[UnitFactor]
                         END AS [UnitFactor],
                         T.[UnitPrice] AS TUnitPrice,
                         T.[Rate] AS [TaskRate],
                         CASE 
                             WHEN WS.[ID] IS NOT NULL THEN WS.[Rate]
                             WHEN TS.[ID] IS NOT NULL THEN  TS.[UnitPrice] 
                         END  AS ServicePrice,
                        CASE
                            WHEN TS.[ID] IS NOT NULL AND WS.ID IS NULL THEN 1
                            ELSE 0
                        END AS [OldVersion],
						 WO.ID AS WorkOrderId,
						 WO.CreatedDate AS WorkOrderCreatedDate,
						 WO.[Guid] AS WorkOrderGuid,
						 WO.Number,
                         WO.Location,
                         WO.ClosingNotes,
                         WO.Description AS [WorkOrderDescription],
						 WO.BillingName AS BuildingBillingFullName,
						 WO.BillingEmail AS BuildingBillingEmail,
						 WO.BillingNote AS BuildingNoteToBilling,
                         ISNULL(WO.[RequesterEmail], '') as RequesterEmail,
                        -- CLONING FIELDS
                        WO.[OriginWorkOrderId],
	                    CASE 
		                    WHEN ISNULL(WO.[OriginWorkOrderId], 0) = 0 THEN '' 
		                    ELSE CONCAT((SELECT WOP.[Number] FROM [dbo].[WorkOrders] AS WOP WHERE WOP.[ID] = WO.[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha](WO.[CloneNumber])) 
	                    END AS [ClonePath],
                        (
                            SELECT 
                                TOP 1 CT.[TicketId] 
                            FROM [ConvertedTickets] AS CT WHERE CT.DestinationType = {(int)TicketDestinationType.WorkOrder} AND CT.DestinationEntityId = T.WorkOrderId
                        ) AS [TicketId]";

            string fromTables = $@"
                        [dbo].[WorkOrderTasks] AS T
							INNER JOIN [dbo].[WorkOrders] AS WO ON WO.ID = T.WorkOrderId
                            LEFT JOIN [dbo].[Services] AS TS ON TS.ID = T.ServiceId
                            LEFT JOIN [dbo].[WorkOrderServices] AS WS ON WS.ID = T.WorkOrderServiceId
							LEFT JOIN [dbo].[Buildings] AS B ON B.ID = WO.BuildingId
                            { additionalJoin } ";

            string conditions = $@"
                        AND ((TS.[ID] IS NOT NULL)OR(WS.[ID] IS NOT NULL)) AND WO.CompanyId = @companyId
                        AND ((@customerId IS NOT NULL AND B.[CustomerId] = @customerId) OR @customerId IS NULL)
                        AND WO.IsActive = 1 
                        AND WO.StatusId = {(int)WorkOrderStatus.Closed} 
                        AND CAST(DATEADD(minute, -@timezoneOffset, ({logDate})) AS DATE) BETWEEN ISNULL(@dateFrom, DATEADD(yy, -10, GETUTCDATE())) AND ISNULL(@dateTo, DATEADD(dd, 1, GETUTCDATE()))
                        AND CONCAT(ISNULL(convert(nvarchar(255), WO.Number), ''), T.[Description], TS.[Name], B.[Name], WO.BillingName, WO.BillingEmail) LIKE '%' + ISNULL(@filter, '') + '%'";

            if (request.BuildingIds.Any())
            {
                conditions += $" AND WO.[BuildingId] IN @buildingIds ";
            }

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : this.GetTasksSorts(request.SortField.ToLower(), request.SortOrder);

            orders += $" ({logDate}) DESC";

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

        private string GetTasksSorts(string field, string order)
        {
            order = order.ToUpper();

            // HACK: don't wanna use switch statements
            var repl = new Dictionary<string, string>
            {
                ["taskname"] = "T.[Description]",
                ["istaskchecked"] = "T.[IsComplete]",
                ["number"] = "WO.[Number]",
                ["buildingbillingfullname"] = "WO.[BillingName]",
                ["taskcreateddate"] = "T.[CreatedDate]",
                ["workordercreateddate"] = "W.[CreatedDate]",
                ["completeddate"] = "T.[LastCheckedDate]",
            };


            if (repl.TryGetValue(field, out string replacedField))
            {
                field = replacedField;
            }
            else
            {
                field = $"T.[{field}]";
            }

            return $"{field} {order},";
        }

        public async Task<IEnumerable<WorkOrder>> ReadAllByIDs(IEnumerable<int> ids)
        {
            var result = this.Entities
                .Include(w => w.Tasks)
                .Where(w => ids.Contains(w.ID)).ToList();

            return result.AsEnumerable();
        }

        // CALENDAR
        public async Task<DataSource<CalendarGridViewModel>> ReadAllSnozeedDapperAsync(DataSourceRequest request, int companyId)
        {
            var result = new DataSource<CalendarGridViewModel>
            {
                Payload = new List<CalendarGridViewModel>(),
                Count = 0
            };

            string whereStr = string.Empty;

            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST(WO.[SnoozeDate] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST(WO.[SnoozeDate] AS DATE) <= @dateTo ";
            }

            string query = $@"
                SELECT  
	                WO.[ID],
	                WO.[SnoozeDate],
	                CONCAT('WORK ORDER # ', NUMBER,', BUILDING = ' + B.Name, + ' STATUS: ' +
					  (
							SELECT CASE
								 WHEN upper(WO.StatusId) like 0 THEN
								  'Draft'
								 WHEN upper(WO.StatusId) like 1 THEN
								  'StandBy'
								 WHEN upper(WO.StatusId) like 2 THEN
								  'Closed'
								WHEN upper(WO.StatusId) like 4 THEN
								  'Cancelled'
								 ELSE
								  'unknown'
									END
					  ) 
                    +', DESCRIPTION: '+ WO.Description) as Description,
                    {(int)CalendarEventType.WorkOrder} as Type
                FROM WorkOrders AS WO
                    LEFT JOIN [Buildings] B ON B.ID = WO.BuildingId 
                WHERE WO.[CompanyId] = @companyId
                     AND WO.[SnoozeDate] IS NOT NULL
                    AND WO.[DueDate] IS NOT NULL 
                     { whereStr}";
            // and CAST(WO.[SnoozeDate] AS DATE) >= CAST(GETUTCDATE() AS DATE)
            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);

            var payload = await this._baseDapperRepository.QueryAsync<CalendarGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<WorkOrderCalendarGridViewModel>> ReadAllCalendarDapperAsync(DataSourceRequestCalendar request, int companyId)
        {
            var result = new DataSource<WorkOrderCalendarGridViewModel>
            {
                Payload = new List<WorkOrderCalendarGridViewModel>(),
                Count = 0
            };

            DynamicParameters parameters = new DynamicParameters();

            request.TypeId = request.TypeId.HasValue ? (request.TypeId == -1 ? null : request.TypeId) : null;

            string where = string.Empty;
            if (request.DateFrom.HasValue)
            {
                where += $"AND CAST(CASE WHEN WO.[ScheduleDate] IS NULL THEN WO.[DUEDATE] ELSE WO.[ScheduleDate] END AS DATE) >= CAST(@dateFrom AS DATE)  ";
                //where += $" AND (CAST(WO.[ScheduleDate] AS DATE) >= @dateFrom) ";
            }

            if (request.DateTo.HasValue)
            {
                where += $"AND CAST(CASE WHEN WO.[ScheduleDate] IS NULL THEN WO.[DUEDATE] ELSE WO.[ScheduleDate] END AS DATE) <= CAST(@dateTo AS DATE)   ";
                //where += $" AND (CAST(WO.[ScheduleDate] AS DATE) <= @dateTo) ";
            }

            if (request.ApprovedStatus.HasValue)
            {
                if (request.ApprovedStatus.Value != 3)
                {
                    where += $" AND WO.ClientApproved = @ApprovedStatus";
                }
            }

            if (request.ScheduleDateConfirmed.HasValue)
            {
                if (request.ScheduleDateConfirmed.Value != 3)
                {
                    where += $" AND WO.ScheduleDateConfirmed = @ScheduleDateConfirmed ";
                }
            }
            if (request.ScheduleCategory != null)
            {
                where += $" AND WO.ScheduleCategoryId in @scheduleCategory";
                parameters.Add("@scheduleCategory", request.ScheduleCategory.ToList());

                if (request.ScheduleSubCategory != null)
                {
                    if (request.ScheduleSubCategory.Length > 0)
                    {
                        where += $" AND WO.ScheduleSubCategoryId in @scheduleSubCategory";
                        parameters.Add("@scheduleSubCategory", request.ScheduleSubCategory.ToList());
                    }
                }
            }



            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT
                        WO.[ID],
                        WO.[OriginWorkOrderId],
                        CASE 
                            WHEN ISNULL(WO.[OriginWorkOrderId], 0) = 0 THEN '' 
                            ELSE CONCAT((SELECT [W].[Number] FROM [dbo].[WorkOrders] AS W WHERE [W].[ID] = WO.[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha](WO.[CloneNumber])) 
                        END AS [ClonePath],
                        WO.[Number],
                        WO.[Description],
                        CASE 
                            WHEN WO.[StatusId] = 0  AND WO.[Location] IS NULL THEN WO.[FullAddress] 
                            ELSE WO.[Location] 
                        END AS [Location],
                        ISNULL(B.[Name], '') AS [BuildingName],
                        WO.[RequesterFullName],
                        WO.[CreatedDate] AS DateSubmitted,
                        WO.[DueDate],
                        WO.[ScheduleDate],
                        WO.[IsExpired],
                        WO.[SendRequesterNotifications],
                        WO.[SendPropertyManagersNotifications],
                        WO.[Type],
                        WO.[ScheduleCategoryId],
                        WO.[ScheduleSubCategoryId],
                        WO.[ClientApproved],
                        WO.[BillingDateType],
                        WO.[ScheduleDateConfirmed],
                        (Select description from [dbo].[ScheduleSettingCategories] SC where SC.ID = WO.[ScheduleCategoryId]) as ScheduleCategoryName,
                        (Select Name from [dbo].[ScheduleSettingSubCategories] SSC where SSC.ID = WO.[ScheduleSubCategoryId]) as ScheduleSubCategoryName,
                        (Select Color from [dbo].[ScheduleSettingCategories] SSC where SSC.ID = WO.[ScheduleCategoryId]) as Color,
                        WO.[Guid],
                        WO.[Unscheduled],
                        (SELECT COUNT(W.ID) FROM [dbo].[WorkOrders] AS W WHERE W.[ID] > WO.[ID] AND W.[WorkOrderScheduleSettingId] = WO.[WorkOrderScheduleSettingId]) AS [SequencePosition],
                        (SELECT COUNT(W.ID) FROM [dbo].[WorkOrders] AS W WHERE W.[WorkOrderScheduleSettingId] = WO.[WorkOrderScheduleSettingId]) AS [ElementsInSequence],
                        ISNULL(WO.[WorkOrderScheduleSettingId], 0) AS [WorkOrderScheduleSettingId],
                        ISNULL(WO.[CalendarItemFrequencyId], 0) AS [CalendarItemFrequencyId],
                        (SELECT COUNT(*) FROM WorkOrderNotes N WHERE N.WorkOrderId = WO.[ID]) as [NotesCount],
                        (SELECT COUNT(*) FROM WorkOrderTasks T WHERE T.WorkOrderId = WO.[ID]) as [TasksCount],
                        (SELECT COUNT(*) FROM WorkOrderTasks T WHERE T.WorkOrderId = WO.[ID] AND T.IsComplete=1) as [TasksDoneCount],
                        (SELECT SUM(T.Rate * T.Quantity) FROM [WorkOrderTasks] AS T WHERE T.WorkOrderId = WO.[ID] AND T.WorkOrderServiceId IS NOT NULL) as TotalBill,
                        ISNULL(WO.[StatusId], 0) as StatusId
                    FROM [dbo].[WorkOrders] AS WO 
                        LEFT OUTER JOIN [dbo].[Buildings] AS B on B.[ID] = WO.[BuildingId]
                    WHERE WO.[CompanyId] = @CompanyId 
                        AND WO.[IsActive] = 1
                        AND ((@BuildingId IS NOT NULL AND B.[ID] = @BuildingId) OR (@BuildingId IS NULL))
                        AND ((@CustomerId IS NOT NULL AND B.[CustomerId] = @CustomerId) OR (@CustomerId IS NULL))
                        AND ((@TypeId IS NOT NULL AND WO.[Type] = @TypeId) OR (@TypeId IS NULL))
                    {where}
                ) payload ORDER BY ID DESC
            ";

            parameters.Add("@CompanyId", companyId);

            parameters.Add("@dateFrom", request.DateFrom);
            parameters.Add("@dateTo", request.DateTo);

            parameters.Add("@DueDateStatus", request.DueDateStatus);
            parameters.Add("@ScheduleDateConfirmed", request.ScheduleDateConfirmed);
            parameters.Add("@ApprovedStatus", request.ApprovedStatus);

            parameters.Add("@BuildingId", request.BuildingId);
            parameters.Add("@CustomerId", request.CustomerId);
            parameters.Add("@TypeId", request.TypeId);

            parameters.Add("@pageNumber", request.PageNumber);
            parameters.Add("@pageSize", request.PageSize);

            var payload = await this._baseDapperRepository.QueryAsync<WorkOrderCalendarGridViewModel>(query, parameters);
            result.Payload = payload;
            result.Count = payload.FirstOrDefault()?.Count ?? 0;

            return result;
        }

        public async Task<IEnumerable<WorkOrderTaskSummaryViewModel>> ReadAllWorkOrderSequence(int calendarItemFrequencyId)
        {
            IEnumerable<WorkOrderTaskSummaryViewModel> result = new
                    List<WorkOrderTaskSummaryViewModel>();
            string query = $@"
                SELECT
	                WO.[ID],
	                WO.[OriginWorkOrderId],
	                WO.[Number],
	                WO.[ScheduleDate],
                    WO.[DueDate],
	                WO.[ScheduleDateConfirmed],
                    WO.[BillingDateType],
	                WO.[ClientApproved] AS [ClientApproved],
                    WO.[SendRequesterNotifications],
                    WO.[SendPropertyManagersNotifications],
                    WO.[StatusId],
                    WO.[IsExpired],
                    WO.[Unscheduled],
                    (SELECT COUNT(*) FROM WorkOrderNotes N WHERE N.WorkOrderId = WO.[ID]) as [NotesCount],
                    (SELECT COUNT(*) FROM WorkOrderTasks T WHERE T.WorkOrderId = WO.[ID]) as [TasksCount],
                    (SELECT COUNT(*) FROM WorkOrderTasks T WHERE T.WorkOrderId = WO.[ID] AND T.IsComplete=1) as [TasksDoneCount],
                    (SELECT SUM(UNITPRICE) FROM WorkOrderTasks WT where WT.WorkOrderId =  WO.[ID] ) as TotalBill,
	                CASE 
		                WHEN ISNULL(WO.[OriginWorkOrderId], 0) = 0 THEN '' 
		                ELSE CONCAT((SELECT [W].[Number] FROM [dbo].[WorkOrders] AS W WHERE [W].[ID] = WO.[OriginWorkOrderId]), '-', [dbo].[ConvertNumberToAlpha](WO.[CloneNumber])) 
                    END AS [ClonePath],
                    (SELECT COUNT(W.ID) FROM [dbo].[WorkOrders] AS W WHERE W.[ID] > WO.[ID] AND W.[WorkOrderScheduleSettingId] = WO.[WorkOrderScheduleSettingId]) AS [SequencePosition],
                    (SELECT COUNT(W.ID) FROM [dbo].[WorkOrders] AS W WHERE W.[WorkOrderScheduleSettingId] = WO.[WorkOrderScheduleSettingId]) AS [ElementsInSequence],
                    ISNULL(WO.[WorkOrderScheduleSettingId], 0) AS [SequenceId],
	                ISNULL(WT.ID, -1) AS [ID],
	                WT.[WorkOrderId],
	                WT.[IsComplete],
	                WT.[Description],
	                WT.[ServiceId],
	                WT.[UnitPrice],
	                WT.[Quantity],
	                WT.[DiscountPercentage],
	                WT.[Note],
	                WT.[LastCheckedDate]
                FROM [dbo].[WorkOrders] AS WO
	                LEFT OUTER JOIN [dbo].[WorkOrderTasks] WT ON WT.[WorkOrderId] = WO.[ID]
                WHERE WO.[WorkOrderScheduleSettingId] = @CalendarFrequencyId AND WO.[IsActive] = 1
                ORDER BY WO.[ScheduleDate] ASC
                ";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@CalendarFrequencyId", calendarItemFrequencyId);

            var rows = await this._baseDapperRepository.QueryChildListAsync<WorkOrderTaskSummaryViewModel, WorkOrderTaskBaseViewModel>(query, pars);
            if (rows.Any())
            {
                result = rows;
            }
            return result;
        }

        public async Task UnassignEmployeesByWorkOrderIdAsync(int workOrderId)
        {
            string query = "DELETE FROM [WorkOrderEmployees] WHERE WorkOrderId = @WorkOrderId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@WorkOrderId", workOrderId);
            await this._baseDapperRepository.ExecuteAsync(query, parameters);
        }
    }
}
