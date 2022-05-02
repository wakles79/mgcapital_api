using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.PushNotifications;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class PushNotificationRepository : IPushNotificationRepository
    {
        private readonly IBaseDapperRepository DapperRepository;

        public PushNotificationRepository(IBaseDapperRepository dapperRepository)
        {
            this.DapperRepository = dapperRepository;
        }

        public async Task<int> InsertPushNotification(PushNotification notification)
        {
            int newId = await this.DapperRepository.InsertAsync(notification);
            return newId;
        }

        public async Task<int> InsertPushNotificationFilters(IEnumerable<PushNotificationFilter> filters)
        {
            int count = await this.DapperRepository.InsertRangeAsync(filters);
            return count;
        }

        public async Task<DataSource<PushNotificationGridViewModel>> ReadAllByUserAsync(DataSourceRequestPushNotifications dataQuery, string userEmail, int companyId)
        {
            var queryTemplate = this.QueryReadAllByUser(dataQuery);
            queryTemplate.PageNumber = dataQuery.PageNumber;
            queryTemplate.RowsPerPage = dataQuery.PageSize;

            var pars = new DynamicParameters();
            pars.Add("@UserEmail", userEmail);
            pars.Add("@CompanyId", companyId);

            var result = await this.DapperRepository.QueryPagedAsync<PushNotificationGridViewModel>(queryTemplate, pars);

            return result;
        }

        public async Task<IEnumerable<WorkOrdersByDueDateViewModel>> ReadAllByDueDateAsync(int companyId, int timezoneOffset = 300)
        {
            try
            {
                string query = @"
                    SELECT
	                    [dbo].[Employees].[Id],
	                    [dbo].[Employees].[Email] AS [UserEmail],
	                    (
		                    SELECT 
			                    COUNT([dbo].[WorkOrderEmployees].[WorkOrderId]) 
		                    FROM 
			                    [dbo].[WorkOrders]
			                    INNER JOIN [dbo].[WorkOrderEmployees] 
				                    ON [dbo].[WorkOrderEmployees].[WorkOrderId] = [dbo].[WorkOrders].[Id] 
					                    AND [dbo].[WorkOrderEmployees].[EmployeeId] = [dbo].[Employees].[Id]
		                    WHERE 
			                    [dbo].[WorkOrders].[StatusId] IN (1, 2) AND CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE)
	                    ) AS [DueToday],
	                    (
		                    SELECT 
			                    COUNT([dbo].[WorkOrderEmployees].[WorkOrderId]) 
		                    FROM 
			                    [dbo].[WorkOrders]
			                    INNER JOIN [dbo].[WorkOrderEmployees] 
				                    ON [dbo].[WorkOrderEmployees].[WorkOrderId] = [dbo].[WorkOrders].[Id] 
					                    AND [dbo].[WorkOrderEmployees].[EmployeeId] = [dbo].[Employees].[Id]
		                    WHERE 
			                    [dbo].[WorkOrders].[StatusId] IN (1, 2) AND CASE WHEN DueDate = '0001-01-01 00:00:00.0000000' THEN  '3000-01-01' ELSE CAST(DATEADD(minute, -@timezoneOffset, DueDate) AS DATE) END = CAST(DATEADD(minute, -@timezoneOffset, GETUTCDATE()) AS DATE)
	                    ) AS [PastDue]
                    FROM
	                    [dbo].[Employees]
	                    INNER JOIN [dbo].[Roles] ON [dbo].[Roles].[Id] = [dbo].[Employees].[RoleId]
                    WHERE
	                    [dbo].[Employees].[CompanyId] = @companyId AND
	                    [dbo].[Roles].[Level] = @roleLevel";

                var pars = new DynamicParameters();
                pars.Add("@companyId", companyId);
                pars.Add("@timezoneOffset", timezoneOffset);
                pars.Add("@roleLevel", (int)EmployeeRole.Operation_Manager);

                var result = await this.DapperRepository.QueryAsync<WorkOrdersByDueDateViewModel>(query, pars);

                return result;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                throw ex;
            }
        }

        public async Task<bool> MarkAsRead(PushNOtificationMarkAsReadViewModel notification, string userEmail, int companyId)
        {
            try
            {
                string queryEmployeeId = @"
                    SELECT 
                        ISNULL([dbo].[Employees].[Id], -1) AS [Id] 
                    FROM 
                        [dbo].[Employees] 
                    WHERE 
                        [dbo].[Employees].[Email] = @email AND [dbo].[Employees].[CompanyId] = @companyId ";

                    string queryNotificationId = @"
                    SELECT 
	                    ISNULL([dbo].[PushNotifications].[Id], -1) AS [Id]
                    FROM 
	                    [dbo].[PushNotifications]
                    WHERE
	                    [dbo].[PushNotifications].[OneSignalId] = @oneSignalId ";

                var pars = new DynamicParameters();
                pars.Add("@email", userEmail);
                pars.Add("@companyId", companyId);
                pars.Add("@oneSignalId", notification.Guid);

                Tuple<int, int> result = await this.DapperRepository.MultiQueryAsync<int, int>(queryEmployeeId, queryNotificationId, pars);

                if (result.Item1 > 0)
                {
                    var entity = new PushNotificationConvert
                    {
                        EmployeeId = result.Item1,
                        PushNotificationId = result.Item2
                    };


                    await this.DapperRepository.InsertAsync(entity);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return false;
            }
        }


        public async Task<bool> Exists(string Content, string Data, string Heading, PushNotificationReason Reason)
        {
            try
            {
                string queryNotifications = @"
                    SELECT * 
                    FROM 
                        [dbo].[PushNotifications] 
                    WHERE 
                        Content=@content and Data=@data and Heading=@heading and Reason=@reason";

                var pars = new DynamicParameters();
                pars.Add("@content", Content);
                pars.Add("@data", Data);
                pars.Add("@heading", Heading);
                pars.Add("@reason", Reason);

                var result = await this.DapperRepository.QueryAsync<PushNotification>(queryNotifications, pars);
                  
                if (result.Count() > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                return false;
            }
        }
        #region Utils

        private PagedQueryTemplate QueryReadAllByUser(DataSourceRequestPushNotifications dataQuery)
        {
            string preQuery = @"
                DECLARE @employeeId AS INT

                SELECT @employeeId = ISNULL((SELECT [Id] FROM [dbo].[Employees] WHERE [Email] = @UserEmail AND [CompanyId] = @companyId), -1) ;";

            string selectFields = @"
                [dbo].[PushNotifications].[Id], 
                [dbo].[PushNotifications].[Heading],
                [dbo].[PushNotifications].[Content],
                [dbo].[PushNotifications].[CompletedAt] AS [Completed_At],
                [dbo].[PushNotifications].[Reason],
                [dbo].[PushNotifications].[OneSignalId],
                [dbo].[PushNotifications].[DataType],
                [dbo].[PushNotifications].[Data],
                IIF(ISNULL([dbo].[PushNotificationConverts].[EmployeeId], -1) = -1, 1, 0) AS [Unread]   ";

            string fromTables = @"
	            [dbo].[PushNotifications]
	            INNER JOIN [dbo].[PushNotificationFilters] ON [dbo].[PushNotificationFilters].[PushNotificationId] = [dbo].[PushNotifications].[Id]
	            LEFT OUTER JOIN [dbo].[PushNotificationConverts] ON 
                    [dbo].[PushNotificationConverts].[PushNotificationId] = [dbo].[PushNotifications].[ID]  
					AND [dbo].[PushNotificationConverts].[EmployeeId] = @employeeId";

            string conditions = string.Format(@"
	            AND [dbo].[PushNotificationFilters].[Field] = 'tag' 
	            AND [dbo].[PushNotificationFilters].[Key] = 'user_email' 
	            AND [dbo].[PushNotificationFilters].[Relation] = '=' 
	            AND [dbo].[PushNotificationFilters].[Value] = @UserEmail 
                {0} 
                {1} 
                {2}
                {3}",
                dataQuery.Reason.HasValue ? string.Format(" AND [dbo].[PushNotifications].[Reason] = {0} ", (int)dataQuery.Reason.Value) : string.Empty,
                dataQuery.Unread.HasValue ? string.Format(" AND IIF(ISNULL([dbo].[PushNotificationConverts].[EmployeeId], -1) = -1, 1, 0) = {0}", 
                                                            dataQuery.Unread.Value ? 1 : 0) : string.Empty,
                dataQuery.DateFrom.HasValue && dataQuery.DateFrom.Value.Year > 2017 ? DateCondition(dataQuery.DateFrom.Value, " > ") : string.Empty,
                dataQuery.DateTo.HasValue && dataQuery.DateTo.Value.Year > 2017 ? DateCondition(dataQuery.DateTo.Value, " < ") : string.Empty);

            string orders = @" 
                IIF(ISNULL([dbo].[PushNotificationConverts].[EmployeeId], -1) = -1, 1, 0) DESC, 
                [dbo].[PushNotifications].[CompletedAt] DESC ";

            return new PagedQueryTemplate {
                PreQuery = preQuery,
                SelectFields = selectFields,
                FromTables = fromTables,
                Conditions = conditions,
                Orders = orders
            };
        }

        private string DateCondition(DateTime dateTime, string comparer)
        {
            return string.Format(" AND [dbo].[PushNotifications].[CompletedAt] {0} {1}", comparer, dateTime.ToUniversalTime().ToEpoch());
        }

        #endregion
    }
}
