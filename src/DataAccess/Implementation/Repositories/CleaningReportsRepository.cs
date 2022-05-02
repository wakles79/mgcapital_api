// -----------------------------------------------------------------------
// <copyright file="CleaningReportsRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.CleaningReportItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="CleaningReport"/>
    /// </summary>
    public class CleaningReportsRepository : BaseRepository<CleaningReport, int>, ICleaningReportsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CleaningReportsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public CleaningReportsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        //private int NextNumber(int companyId)
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

        public override async Task<CleaningReport> AddAsync(CleaningReport obj)
        {
            return await Task.Run(() => { return Add(obj); });
        }

        public override CleaningReport Add(CleaningReport obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            //obj.Number = this.NextNumber(obj.CompanyId);

            return base.Add(obj);
        }

        public async Task<DataSourceCleaningReport> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? contactId = null, int? statusId = null, int? employeeId = null, int? commentDirection = null)
        {
            var result = new DataSourceCleaningReport
            {
                Payload = new List<CleaningReportGridViewModel>(),
                Count = 0,
                PendingToReplyCount = 0,
                RepliedCount = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string whereStr = "";
            //var rolLevelFilter = string.Empty;
            //// extra params
            //bool isExpiredStr = request.IsExpired ?? false;
            //int employeeId = -1;
            //var statusIds = new List<int>();


            if (request.DateFrom.HasValue)
            {
                whereStr += $" AND CAST([DateOfService] AS DATE) >= @dateFrom ";
            }

            if (request.DateTo.HasValue)
            {
                whereStr += $" AND CAST([DateOfService] AS DATE) <= @dateTo ";
            }

            if (employeeId.HasValue)
            {
                whereStr += $" AND [EmployeeId] = @employeeId ";
            }

            if (contactId.HasValue)
            {
                whereStr += $" AND [ContactId] = @contactId ";
            }

            if (statusId.HasValue)
            {
                whereStr += statusId == (int)CleaningReportStatus.Draft ? $" AND [Submitted] = 0 " : $" AND [Submitted] > 0 ";
            }

            if (commentDirection.HasValue)
            {
                whereStr += " AND LastCommentDirection = @commentDirection";
            }

            var selectItemsObservancesStr = String.Empty;
            var whereItemsObservancesStr = String.Empty;

            if (!string.IsNullOrEmpty(request.Filter))
            {
                selectItemsObservancesStr = $@", ( SELECT STRING_AGG( ISNULL(Observances, ''), '') As Observances 
                                      FROM [dbo].[CleaningReportItems]
                                      WHERE [dbo].[CleaningReportItems].[CleaningReportId] = [dbo].[CleaningReports].[ID]                
                                    ) as ItemObservances ";

                whereItemsObservancesStr = $"+ ISNULL(ItemObservances, '')";
            }


            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
	                    [dbo].[CleaningReports].[ID],
                        [dbo].[CleaningReports].[Number],
	                    [dbo].[CleaningReports].[CompanyId],
                        [dbo].[CleaningReports].[ContactId],
                        [dbo].[CleaningReports].[DateOfService],
                        [dbo].[CleaningReports].[EmployeeId],
                        [dbo].[CleaningReports].[Location],
                        [dbo].[CleaningReports].[guid],
						[dbo].[CleaningReports].Submitted,
						CONCAT(EmployeeContact.[FirstName], ' ', EmployeeContact.[MiddleName], ' ', EmployeeContact.[LastName]) AS [PreparedFor],
						CONCAT([Contacts].[FirstName], ' ', [Contacts].[MiddleName], ' ', [Contacts].[LastName]) as [To],
						(select count(*) from [dbo].[CleaningReportItems]  where [dbo].[CleaningReportItems].[CleaningReportId] = [dbo].[CleaningReports].[ID] AND [dbo].[CleaningReportItems].[Type] = 0) as CleaningItems,
						(select count(*) from [dbo].[CleaningReportItems]  where [dbo].[CleaningReportItems].[CleaningReportId] = [dbo].[CleaningReports].[ID] AND [dbo].[CleaningReportItems].[Type] = 1) as FindingItems,
                        [dbo].[Companies].[Name] as CompanyName,
                        (SELECT ISNULL(
	                        (
		                        SELECT TOP(1) [dbo].[CleaningReportNotes].[Direction]
                                FROM [dbo].[CleaningReportNotes] WHERE [dbo].[CleaningReportNotes].CleaningReportId = [dbo].[CleaningReports].[ID]
                                ORDER BY CreatedDate DESC)
	                        , 0)) AS [LastCommentDirection]
                        {selectItemsObservancesStr}

	                    FROM [dbo].[CleaningReports]
								INNER JOIN [dbo].[Employees] ON [dbo].[Employees].ID = [dbo].[CleaningReports].[EmployeeId]
								INNER JOIN [dbo].[Contacts] as EmployeeContact ON EmployeeContact.ID = [dbo].[Employees].[ContactId] 
								INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].ID = [dbo].[CleaningReports].[ContactId]
                                INNER JOIN [dbo].[Companies] ON [dbo].[Companies].ID = [dbo].[CleaningReports].[CompanyId]                             
					                    ) payload 
                        WHERE CompanyId = @companyId AND
                                ISNULL(Location, '') + 
                                ISNULL([PreparedFor], '') +
                                ISNULL([To], '') +
                                ISNULL(CompanyName, '') +
                                ISNULL(CAST(Number AS NVARCHAR(15)), '')
                                {whereItemsObservancesStr}    
                                    LIKE '%' + ISNULL(@filter, '') + '%'
                                {whereStr}
                    ORDER BY  {orders} ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@dateFrom", request.DateFrom);
            pars.Add("@dateTo", request.DateTo);
            pars.Add("@employeeId", employeeId);
            pars.Add("@contactId", contactId);
            pars.Add("@commentDirection", commentDirection);

            var payload = await _baseDapperRepository.QueryAsync<CleaningReportGridViewModel>(query, pars);

            // Gets the last direction for each note (0 if the cleaning report doesn't have any note)
            string directionsCountQuery = @"
                SELECT
                    ISNULL((SELECT TOP 1 N.Direction FROM CleaningReportNotes AS N WHERE N.CleaningReportId = C.ID ORDER BY N.CreatedDate DESC), 0) AS Direction
                FROM CleaningReports AS C
                WHERE C.CompanyId = @companyId AND C.CreatedDate >= DATEADD(yy, -10, GETDATE())
            ";

            var counts = await _baseDapperRepository.QueryAsync<int>(directionsCountQuery, pars);

            if (counts?.Any() == true)
            {
                foreach (var c in counts)
                {
                    if (c == (int)CleaningReportNoteDirection.Incoming)
                    {
                        result.PendingToReplyCount++;
                    }
                    else if (c == (int)CleaningReportNoteDirection.Outgoing)
                    {
                        result.RepliedCount++;
                    }
                }
            }

            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<CleaningReportListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null)
        {
            var result = new DataSource<CleaningReportListBoxViewModel>
            {
                Payload = new List<CleaningReportListBoxViewModel>(),
                Count = 0
            };

            string query = $@"
                        declare @index int;
                        declare @maxIndex int;
                        declare @total int;

                        IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
                        BEGIN
                            select @index =  @pageNumber;
                        END
                        ELSE
                        BEGIN
                        SELECT @index = [Index] - 1 FROM ( 
                            SELECT 
		                        [dbo].[CleaningReports].ID as ID, 
		                        [dbo].[CleaningReports].[CompanyId] as CompanyId,
                                ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY [dbo].[CleaningReports].[Number], [dbo].[CleaningReports].ID ) as [Index]
                            FROM [dbo].[CleaningReports]
                            ) payload
                        WHERE ID = @id;
                        END

                        SELECT @total = COUNT(*) FROM [dbo].[CleaningReports] WHERE [dbo].[CleaningReports].[CompanyId]= @companyId;

                        --max(0, @total-@pageSize)
                        SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                        --safety check
                        SELECT @index = ISNULL(@index, 0);

                        --min(@index, @maxIndex)
                        SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                        SELECT 
	                        CR.[ID],
                            CR.[CreatedDate],
                            CR.[Location],
	                        CR.[Number],
                            CONCAT(C.FirstName, ' ', C.LastName) AS [To],
                            CONCAT(EC.FirstName, ' ', EC.LastName) AS [From],
	                        CR.[ContactId] as CustomerContactId

                        FROM [dbo].[CleaningReports] AS CR
                            INNER JOIN [dbo].[Contacts] AS C ON CR.ContactId = C.ID
                            INNER JOIN [dbo].[Employees] AS E ON CR.EmployeeId = E.ID
                            INNER JOIN [dbo].[Contacts] AS EC ON E.ContactId = EC.ID
                        WHERE CR.[CompanyId]= @companyId AND
	                        CONCAT(CR.[Number], C.FirstName, C.LastName, EC.FirstName, EC.LastName) 
		                        LIKE '%' + ISNULL(@filter, '') + '%'
                        ORDER BY CR.Number DESC, CR.ID

                        OFFSET @index ROWS
                        FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@id", id);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<CleaningReportListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<CleaningReportDetailsViewModel> GetCleaningReportDetailsDapperAsync(int cleaningReportId = -1, Guid? guid = null)
        {
            var result = new CleaningReportDetailsViewModel();
            var whereStr = String.Empty;
            var innerCondition = string.Empty;
            var wherehead = String.Empty;
            var pars = new DynamicParameters();

            if (cleaningReportId > 0)
            {
                whereStr = "AND CleaningReportId = @cleaningReportId ";
                wherehead = " WHERE [dbo].[CleaningReports].ID = @cleaningReportId ";
                pars.Add("@cleaningReportId", cleaningReportId);
            }
            else if (guid.HasValue)
            {
                innerCondition = " AND  [dbo].[CleaningReports].[Guid] = @guid";
                wherehead = " WHERE [dbo].[CleaningReports].Guid = @guid ";
                pars.Add("@guid", guid.Value);
            }
            var query = $@"
	                    SELECT 
	                    [dbo].[CleaningReportItems].[ID],
						[dbo].[CleaningReportItems].CleaningReportId,
						[dbo].[CleaningReportItems].[BuildingId],
						[dbo].[CleaningReportItems].[Location],
						[dbo].[CleaningReportItems].[Observances],
						[dbo].[CleaningReportItems].[Time],
						[dbo].[CleaningReportItems].[Type],
						([dbo].[Buildings].[Name]) as BuildingName,
						ISNULL([dbo].[CleaningReportItemAttachments].ID, -1) AS [ID],
						[dbo].[CleaningReportItemAttachments].BlobName,
						[dbo].[CleaningReportItemAttachments].[FullUrl],
						[dbo].[CleaningReportItemAttachments].[Title],
						[dbo].[CleaningReportItemAttachments].[CleaningReportItemId],
						[dbo].[CleaningReportItemAttachments].[ImageTakenDate]

	                    FROM [dbo].[CleaningReportItems]
								INNER JOIN [dbo].[Buildings] ON [dbo].[Buildings].ID = [dbo].[CleaningReportItems].[BuildingId] 
								LEFT OUTER JOIN [dbo].[CleaningReportItemAttachments] ON [dbo].[CleaningReportItemAttachments].[CleaningReportItemId] = [dbo].[CleaningReportItems].ID
                                INNER JOIN [dbo].[CleaningReports] ON [dbo].[CleaningReports].[ID] = [dbo].[CleaningReportItems].CleaningReportId {innerCondition}
					                   
                        WHERE [Type] = @type                        
                                {whereStr}
                        ORDER BY [BuildingId]
                    ;
                        ";
            var queryHead = $@"
                        SELECT 
	                    [dbo].[CleaningReports].[ID],
	                    [dbo].[CleaningReports].[CompanyId],
                        [dbo].[CleaningReports].[ContactId],
                        [dbo].[CleaningReports].[DateOfService],
                        [dbo].[CleaningReports].[EmployeeId],
                        [dbo].[CleaningReports].[Location],
                        [dbo].[CleaningReports].[guid],
						[dbo].[CleaningReports].Submitted,
						CONCAT(EmployeeContact.[FirstName], ' ', EmployeeContact.[MiddleName], ' ', EmployeeContact.[LastName]) AS [From],
						CONCAT(Contact.[FirstName], ' ', Contact.[MiddleName], ' ', Contact.[LastName]) as [To],
                        [dbo].[Companies].[Name] as CompanyName,
                        [dbo].[ContactEmails].Email as ToEmail

	                    FROM [dbo].[CleaningReports]
								INNER JOIN [dbo].[Employees] ON [dbo].[Employees].ID = [dbo].[CleaningReports].[EmployeeId]
								INNER JOIN [dbo].[Contacts] as EmployeeContact ON EmployeeContact.ID = [dbo].[Employees].[ContactId] 
								INNER JOIN [dbo].[Contacts] as Contact ON Contact.ID = [dbo].[CleaningReports].[ContactId]
								LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[ContactEmails].[ContactId] = Contact.ID AND 
								ISNULL([dbo].[ContactEmails].[Default], 0) = 1
                                INNER JOIN [dbo].[Companies] ON [dbo].[Companies].ID = [dbo].[CleaningReports].[CompanyId]
                            {wherehead}
                            ";


            pars.Add("@type", (int)CleaningReportType.Cleaning);

            var res = await _baseDapperRepository.QueryAsync<CleaningReportDetailsViewModel>(queryHead, pars);
            result = res.FirstOrDefault();

            var items = await _baseDapperRepository.QueryChildListAsync<CleaningReportItemGridViewModel, CleaningReportItemAttachmentUpdateViewModel>(query, pars, System.Data.CommandType.Text);
            if (items.Any())
                result.CleaningItems = items;

            pars.Add("@type", (int)CleaningReportType.Findings);
            var itemsF = await _baseDapperRepository.QueryChildListAsync<CleaningReportItemGridViewModel, CleaningReportItemAttachmentUpdateViewModel>(query, pars, System.Data.CommandType.Text);
            if (itemsF.Any())
                result.FindingItems = itemsF;

            // <saimel>
            string queryNotes = $@"
                SELECT 
                    [dbo].[CleaningReportNotes].[Id],
                    [dbo].[CleaningReportNotes].[CleaningReportId],
                    [dbo].[CleaningReportNotes].[CreatedDate],
                    [dbo].[CleaningReportNotes].[Direction],
                    CASE WHEN [dbo].[CleaningReportNotes].[SenderId] IS NULL 
		                THEN 'Customer' 
		                ELSE (
			                SELECT TOP(1) CONCAT_WS(' ', c.FirstName, c.LastName)
			                FROM [dbo].[Contacts] AS c
				                LEFT OUTER JOIN [dbo].[Employees] AS e ON e.ContactId = c.ID
				                LEFT OUTER JOIN [dbo].[CleaningReportNotes] AS crn ON crn.SenderId = e.ID
				                LEFT OUTER JOIN [dbo].[CleaningReports] ON crn.CleaningReportId = [dbo].[CleaningReports].[ID]
                            {wherehead})
	                END AS [SenderName],
                    [dbo].[CleaningReportNotes].[Message]
                FROM
                    [dbo].[CleaningReportNotes]
	                LEFT OUTER JOIN [dbo].[CleaningReports] ON [dbo].[CleaningReportNotes].[CleaningReportId] = [dbo].[CleaningReports].[Id] 
                    {wherehead} ";

            var notes = await _baseDapperRepository.QueryAsync<CleaningReportNoteViewModel>(queryNotes, pars);

            result.Notes = notes;
            // </saimel>

            return result;
        }
    }
}
