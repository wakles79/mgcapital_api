using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class CustomerFeedRepository : ICustomerFeedRepository
    {
        private readonly IBaseDapperRepository DapperRepository;

        public CustomerFeedRepository(IBaseDapperRepository dapperRepository)
        {
            this.DapperRepository = dapperRepository;
        }

        public async Task<DataSource<WOCustomerGridViewModel>> WorkOrdersReadAllDapperAsync(CustomerWODataSourceRequest dataRequest, int companyId, string userEmail)
        {
            string preQuery = @"
                DECLARE @contactId AS INT;

                SELECT @contactId = (SELECT [ContactId] FROM [dbo].[CustomerUsers] WHERE [Email] = @userEmail AND [CompanyId] = @companyId) ";

            string selectFields = @" 
	            [dbo].[WorkOrders].[Id] AS [Id],
	            [dbo].[WorkOrders].[Guid] AS [Guid],
	            [dbo].[WorkOrders].[Number] AS [Number],
	            [dbo].[Buildings].[Name] AS [BuildingName],
	            [dbo].[WorkOrders].[Location] AS [Location],
	            [dbo].[WorkOrders].[Description] AS [Description],
	            [dbo].[WorkOrders].[StatusId] AS [StatusId] ";

            string fromTables = @"
	            [dbo].[WorkOrders]
	            INNER JOIN [dbo].[Buildings] ON [dbo].[Buildings].[Id] = [dbo].[WorkOrders].[BuildingId]
                INNER JOIN [dbo].[BuildingContacts] ON ([dbo].[BuildingContacts].[BuildingId] = [dbo].[Buildings].[Id] AND 
                                                        [dbo].[BuildingContacts].[ContactId] = @contactId  AND
														    (   [dbo].[BuildingContacts].[ShowHistoryFrom] is null OR
                                                                [dbo].[BuildingContacts].[ShowHistoryFrom] <=  [dbo].[WorkOrders].[CreatedDate]
                                                            )
                                                        ) ";

            string conditions = string.Format(@"
	            AND [dbo].[WorkOrders].[CompanyId] = @companyId
                AND [dbo].[WorkOrders].[DueDate] IS NOT NULL 
                AND (([dbo].[WorkOrders].[SendRequesterNotifications] = 1 AND [dbo].[WorkOrders].RequesterEmail = @userEmail) or [dbo].[WorkOrders].[SendPropertyManagersNotifications] = 1) 
                {0}
                {1}
                {2} 
                {3} ",
                dataRequest.BuildingId.HasValue ? " AND [dbo].[Buildings].[Id] = @buildingId " : string.Empty,
                dataRequest.DateFrom.HasValue ? " AND CAST([dbo].[WorkOrders].[CreatedDate] AS DATE) >= @dateFrom " : string.Empty,
                dataRequest.DateTo.HasValue ? " AND CAST([dbo].[WorkOrders].[CreatedDate] AS DATE) <= @dateTo " : string.Empty,
                string.IsNullOrEmpty(dataRequest.Statuses) ? string.Empty : $" AND [dbo].[WorkOrders].[StatusId] IN ({dataRequest.Statuses.Replace("_", ",")})" );

            string orders = @" [dbo].[WorkOrders].[Number] ";

            var query = new PagedQueryTemplate
            {
                PreQuery = preQuery,
                SelectFields = selectFields,
                FromTables = fromTables,
                Conditions = conditions,
                Orders = orders,
                PageNumber = dataRequest.PageNumber,
                RowsPerPage = dataRequest.PageSize
            };

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@userEmail", userEmail);
            pars.Add("buildingId", dataRequest.BuildingId);
            pars.Add("@dateFrom", dataRequest.DateFrom);
            pars.Add("@dateTo", dataRequest.DateTo);

            var result = await DapperRepository.QueryPagedAsync<WOCustomerGridViewModel>(query, pars);

            return result;
        }

        public async Task<DataSource<ListBoxViewModel>> BuildingsByContactIdDapperAsync(int customerId, int companyId, string userEmail)
        {
            string query = @"
                DECLARE @contactId AS INT;

                SELECT @contactId = ( 
						                SELECT [dbo].[CustomerUsers].[ContactId] 
						                FROM [dbo].[CustomerUsers] 
						                WHERE [dbo].[CustomerUsers].[Email] = @userEmail AND [dbo].[CustomerUsers].[CompanyId] = @companyId 
					                 )

                SELECT
	                [dbo].[Buildings].[Id] AS [Id],
	                [dbo].[Buildings].[Name] AS [Name]
                FROM
	                [dbo].[Buildings]
                    INNER JOIN [dbo].[BuildingContacts] ON ([dbo].[BuildingContacts].[BuildingId] = [dbo].[Buildings].[Id] AND 
                                                            [dbo].[BuildingContacts].[ContactId] = @contactId ) 
                WHERE
	                [dbo].[Buildings].[CompanyId] = @companyId 
                ORDER BY
	                [dbo].[Buildings].[Name] ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@userEmail", userEmail);

            var result = new DataSource<ListBoxViewModel>();
            result.Payload = await DapperRepository.QueryAsync<ListBoxViewModel>(query, pars);

            return result;
        }

        public async Task<DataSource<CleaningReportCustomerBaseViewModel>> CleaningReportsReadAllDapperAsync(CustomerCleaningReportDataSourceRequest dataRequest, int companyId, string userEmail)
        {
            string selectFields = @"
	            [dbo].[CleaningReports].[Id],
	            [dbo].[CleaningReports].[Guid],
	            [dbo].[CleaningReports].[DateOfService] AS [Date],
	            [dbo].[CleaningReports].[Location],
	            [CleaningItemsCount] = (SELECT COUNT(Id) FROM [dbo].[CleaningReportItems] 
							            WHERE [dbo].[CleaningReportItems].CleaningReportId = [dbo].[CleaningReports].[Id] AND [dbo].[CleaningReportItems].[Type] = 0),
	            [FindingItemsCount] = (SELECT COUNT(Id) FROM [dbo].[CleaningReportItems] 
							            WHERE [dbo].[CleaningReportItems].CleaningReportId = [dbo].[CleaningReports].[Id] AND [dbo].[CleaningReportItems].[Type] = 1)";

            string fromTables = @"
	            [dbo].[CleaningReports] ";

            string conditions = string.Format(@"
		        AND [dbo].[CleaningReports].[CompanyId] = @companyId
		        AND [dbo].[CleaningReports].[Submitted] <> 0
		        AND @contactId IN (
			        SELECT 
				        [dbo].[BuildingContacts].[ContactId] 
			        FROM 
				        [dbo].[BuildingContacts] 
				        INNER JOIN [dbo].[CleaningReportItems] ON [dbo].[CleaningReportItems].[BuildingId] = [dbo].[BuildingContacts].[BuildingId]
			        WHERE
				        [dbo].[CleaningReportItems].[CleaningReportId] = [dbo].[CleaningReports].[Id]
                        AND 
			            (
                            [dbo].[BuildingContacts].[ShowHistoryFrom] is null OR
				            [dbo].[BuildingContacts].[ShowHistoryFrom] <=  [dbo].[CleaningReports].[CreatedDate]
                         )
		        )     
                {0}
                {1}",
                dataRequest.DateFrom.HasValue ? " AND CAST([dbo].[CleaningReports].[DateOfService] AS DATE) >= @dateFrom " : string.Empty,
                dataRequest.DateTo.HasValue ? " AND CAST([dbo].[CleaningReports].[DateOfService] AS DATE) <= @dateTo " : string.Empty);

            string orders = @"[dbo].[CleaningReports].[DateOfService]";

            var query = new PagedQueryTemplate
            {
                SelectFields = selectFields,
                FromTables = fromTables,
                Conditions = conditions,
                Orders = orders,
                PageNumber = dataRequest.PageNumber,
                RowsPerPage = dataRequest.PageSize
            };

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@contactId", dataRequest.ContactId);
            pars.Add("@userEmail", userEmail);
            pars.Add("@dateFrom", dataRequest.DateFrom);
            pars.Add("@dateTo", dataRequest.DateTo);

            var result = await DapperRepository.QueryPagedAsync<CleaningReportCustomerBaseViewModel>(query, pars);

            return result;
        }
    }
}
