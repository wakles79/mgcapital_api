// -----------------------------------------------------------------------
// <copyright file="BuildingsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Building;
using MGCap.Domain.ViewModels.Employee;
using Microsoft.EntityFrameworkCore;
using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.DataAccess.Implementation.Repositories
{
	/// <summary>
	///     Contains the implementation of the functionalities
	///     for handling operations on the <see cref="Building"/>
	/// </summary>
	public class BuildingsRepository : BaseRepository<Building, int>, IBuildingsRepository
	{
		private readonly IBaseDapperRepository _baseDapperRepository;

		/// <summary>
		///     Initializes a new instance of the <see cref="BuildingsRepository"/> class.
		/// </summary>
		/// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
		public BuildingsRepository(
			MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
			: base(dbContext)
		{
			_baseDapperRepository = baseDapperRepository;
		}

		public async Task<IEnumerable<Building>> FindBuildingsInRadioAsync(double latitude, double longitude, double epsilon, int companyId)
		{
			var result = new List<Building>();

			string query = $@"
                        SELECT b.[ID]
                              ,b.[AddressId]
                              ,b.[CompanyId]
                              ,b.[CreatedBy]
                              ,b.[CreatedDate]
                              ,b.[Guid]
                              ,b.[Name]
                              ,b.[UpdatedBy]
                              ,b.[UpdatedDate]
                              ,b.[IsActive]
                              ,b.[SupervisorId]
                          FROM [dbo].[Buildings] as b
                                INNER JOIN [dbo].[Addresses] as a ON b.AddressId = a.ID
                          WHERE SQRT(POWER(a.Latitude - @latitude, 2) + POWER(a.Longitude - @longitude, 2)) <= @epsilon AND
                                b.CompanyId = @companyId
                          ORDER BY SQRT(POWER(a.Latitude - @latitude, 2) + POWER(a.Longitude - @longitude, 2))";

			var pars = new DynamicParameters();
			pars.Add("@companyId", companyId);
			pars.Add("@latitude", latitude);
			pars.Add("@longitude", longitude);
			pars.Add("@epsilon", epsilon);

			var payload = await _baseDapperRepository.QueryAsync<Building>(query, pars);

			if (payload != null && payload.Any())
			{
				result = payload.ToList();
			}

			return result;
		}

		public async Task<Building> FindNearestBuildingAsync(double latitude, double longitude, double epsilon, int companyId)
		{
			var results = await this.FindBuildingsInRadioAsync(latitude, longitude, epsilon, companyId);
			if (results != null && results.Any())
			{
				return results.First();
			}
			return null;
		}

		public async Task<DataSource<BuildingListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null, int? employeeId = null)
		{
			var result = new DataSource<BuildingListBoxViewModel>
			{
				Payload = new List<BuildingListBoxViewModel>(),
				Count = 0
			};

			string employeeIdFilter = string.Empty;
			string rolLevelFilter = string.Empty;
			if (employeeId.HasValue)
			{
				rolLevelFilter = $@" 
                    DECLARE @roleLevel INT;

                    SELECT @roleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].Id = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = {employeeId.Value} ";

				employeeIdFilter = $@" AND ( @roleLevel <= 20 OR (@roleLevel > 20 AND {employeeId.Value} IN (SELECT EmployeeId FROM BuildingEmployees WHERE BuildingID = B3.Id) ) ) ";
			}

			string query = $@"
                declare @index int;
                declare @maxIndex int;
                declare @total int;

                {rolLevelFilter}

                IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
                BEGIN
                    select @index =  @pageNumber;
                END
                ELSE
                BEGIN
                SELECT @index = [Index] - 1 FROM ( 
                    SELECT 
                        B1.ID as ID, 
                        B1.CompanyId as CompanyId,
                        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY B1.Name, B1.ID ) as [Index]
                    FROM [dbo].[Buildings] as B1
						INNER JOIN [dbo].Addresses as A ON B1.AddressId = A.ID 
						WHERE B1.IsActive = 1 AND (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = B1.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0
                    ) payload
                WHERE ID = @id 
                AND 
                (select case when  (SELECT top(1) [dbo].[Buildings].ID
                                    FROM [dbo].[Contracts] INNER JOIN [dbo].[Buildings] ON [dbo].[Contracts].BuildingId = [dbo].[Buildings].ID
                                    WHERE 
                                        [dbo].[Contracts].Status = 1 AND [dbo].[Buildings].IsActive = 1 AND 
                                        (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = [dbo].[Buildings].Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0
                                    ) is null then 0 else 1 end) = 1 ;
                END

                SELECT @total = COUNT(*) FROM [dbo].[Buildings] as B2 WHERE B2.CompanyId= @companyId;

                --max(0, @total-@pageSize)
                SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                --safety check
                SELECT @index = ISNULL(@index, 0);

                --min(@index, @maxIndex)
                SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                SELECT 
                    B3.ID as ID,
	                B3.Name as Name,
					A.FullAddress as FullAddress,
					CASE WHEN B3.Code='' THEN cast(B3.ID as varchar) ELSE B3.Code END as Code				
                        FROM [dbo].[Buildings] as B3 INNER JOIN [dbo].Addresses as A ON B3.AddressId = A.ID
                        WHERE B3.[CompanyId]= @companyId AND B3.[IsActive] = 1 AND
                           (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = B3.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0 AND
                    ISNULL(B3.Name, '') +
                    ISNULL(A.FullAddress, '')
                        LIKE '%' + ISNULL(@filter, '') + '%' {employeeIdFilter} 
                ORDER BY Name, ID

                OFFSET @index ROWS
                FETCH NEXT @pageSize ROWS ONLY ";

			var pars = new DynamicParameters();
			pars.Add("@id", id);
			pars.Add("@companyId", companyId);
			pars.Add("@filter", request.Filter);
			pars.Add("@pageNumber", request.PageNumber);
			pars.Add("@pageSize", request.PageSize);

			var payload = await _baseDapperRepository.QueryAsync<BuildingListBoxViewModel>(query, pars);
			result.Count = payload?.Count() ?? 0;
			result.Payload = payload;

			return result;
		}

		public async Task<DataSource<BuildingGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1, int? isAvailable = -1, int? customerId = -1)
		{
			var result = new DataSource<BuildingGridViewModel>
			{
				Payload = new List<BuildingGridViewModel>(),
				Count = 0
			};

			string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

			string query = $@"
                    -- payload query
					SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
                        B.ID,
                        CASE WHEN B.[Code] = '' THEN CAST(B.[ID] AS VARCHAR(32)) ELSE B.[Code] END AS Code,
						B.[Name],
						B.[IsActive],
						B.[CompanyId],
                        B.[CreatedDate],
                        B.[EmergencyPhone],
                        B.[EmergencyPhoneExt],
						A.[FullAddress],
						-- OMC.FirstName + ' ' + OMC.LastName as OperationsManagerFullName,
                        (
		                    SELECT STRING_AGG (CONCAT(C.[FirstName],' ',C.[LastName]), ', ') 
		                    FROM [dbo].[BuildingEmployees] AS BE 
			                    INNER JOIN [dbo].[Employees] AS E ON E.ID = BE.EmployeeId
			                    INNER JOIN [dbo].[Contacts] AS C ON C.ID = E.ContactId
		                    WHERE BE.BuildingId = B.ID AND BE.Type = {(int)BuildingEmployeeType.OperationsManager}
	                    ) AS OperationsManagerFullName,

						-- DUPLICATE QUERIES
                        (select case when  (SELECT top(1) Bb.id
											FROM [dbo].[Contracts] as C INNER JOIN [dbo].[Buildings] as Bb ON C.BuildingId = Bb.ID
										    WHERE 
                                                C.Status = 1 AND 
                                                (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = Bb.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0
                                                and Bb.ID = B.ID
											) is null then 0 else 1 end) as IsComplete,

						-- TODO: For the sake of optimization, re-use the previous query
                        (select case when  (SELECT top(1) Bb.id
											FROM [dbo].[Contracts] as C INNER JOIN [dbo].[Buildings] as Bb ON C.BuildingId = Bb.ID
										    WHERE 
                                                C.Status = 1 AND Bb.IsActive = 1 AND 
                                                (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = Bb.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0
                                                and Bb.ID = B.ID
											) is null then 0 else 1 end) as IsAvailable,
						ISNULL(C.[Code], '') AS [CustomerCode],
						(
							SELECT 
								TOP 1 CO.CustomerId 
							FROM [dbo].[Contracts] AS CO
								WHERE CO.BuildingId = B.ID AND CO.Status = 1
							ORDER BY CO.ID DESC
						) AS CustomerId

	                    FROM [dbo].[Buildings] as B
							INNER JOIN  [dbo].[Addresses] as A ON B.AddressId = A.ID
							LEFT JOIN [Customers] AS C ON C.[ID] = B.[CustomerId]
							-- LEFT OUTER JOIN [dbo].[Employees] as E on E.ID IN (SELECT EmployeeId FROM BuildingEmployees WHERE BuildingId = B.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} )
							-- LEFT OUTER JOIN [dbo].[Contacts] as OMC on E.ContactId = OMC.ID

                       WHERE B.[CompanyId] = @companyId AND
                         B.[IsActive] = CASE WHEN ISNULL(@isActive, -1) = -1 THEN B.[IsActive] ELSE @isActive END AND
                         -- ISNULL(B.[Name], '') + ISNULL((OMC.FirstName + ' ' + OMC.LastName), '') +
                         ISNULL(A.[FullAddress], '') +
			 ISNULL(B.[Code], '') +
                         ISNULL(B.[Name], '') LIKE '%' + ISNULL(@filter, '') + '%'
                         AND ((@customerId > 0 AND B.[CustomerId] = @customerId) OR @customerId <= 0)
					) payload                         
                       WHERE IsAvailable = CASE WHEN ISNULL(@isAvailable, -1) = -1 THEN IsAvailable ELSE @isAvailable END
                    ORDER BY {orders} IsAvailable DESC, CreatedDate DESC, ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";

			var pars = new DynamicParameters();
			pars.Add("@isActive", isActive);
			pars.Add("@isAvailable", isAvailable);
			pars.Add("@companyId", companyId);
			pars.Add("@filter", request.Filter);
			pars.Add("@pageNumber", request.PageNumber);
			pars.Add("@pageSize", request.PageSize);
			pars.Add("@customerId", customerId);

			var payload = await _baseDapperRepository.QueryAsync<BuildingGridViewModel>(query, pars);

			result.Count = payload.FirstOrDefault()?.Count ?? 0;
			result.Payload = payload;

			return result;
		}

		public override Building SingleOrDefault(Func<Building, bool> filter)
		{
			var result = this.Entities
					   .Include(b => b.Address)
					   .Include(b => b.Employees)
							.ThenInclude(b => b.Employee)
								.ThenInclude(b => b.Role)
					   .Include(b => b.Employees)
							.ThenInclude(b => b.Employee)
								.ThenInclude(b => b.Contact)
					   .SingleOrDefault(filter);
			return result;
		}

		public override Building SingleOrDefault(int id)
		{
			return this.SingleOrDefault(b => b.ID == id);
		}

		public override async Task<Building> SingleOrDefaultAsync(Func<Building, bool> filter)
		{
			return await Task.Run(() => { return this.SingleOrDefault(filter); });
		}

		public override async Task<Building> SingleOrDefaultAsync(int id)
		{
			return await Task.Run(() => { return this.SingleOrDefault(id); });
		}

		public async Task<IEnumerable<EmployeeBuildingViewModel>> GetEmployeesByBuildingId_DapperAsync(int buildingId, BuildingEmployeeType type)
		{
			var filter = @"WHERE [dbo].[BuildingEmployees].[BuildingId] = @buildingId";
			var types = type.GetUniqueFlags();
			if (types.Any())
			{
				filter += " AND [dbo].[BuildingEmployees].[Type] IN @types";
			}

			string query = $@"
                SELECT 
                    [dbo].[Employees].[Id],
                    [dbo].[Employees].[Email],
                    CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [Name],
                    [dbo].[Roles].[Name] AS [RoleName],
                    [dbo].[BuildingEmployees].[Type]
                FROM
                    [dbo].[Employees]
                    LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[Id]
                    LEFT OUTER JOIN [dbo].[Roles] ON [dbo].[Employees].[RoleId] = [dbo].[Roles].[Id]
                    INNER JOIN [dbo].[BuildingEmployees] ON [dbo].[Employees].[Id] = [dbo].[BuildingEmployees].[EmployeeId]
                    {filter} ";

			var pars = new DynamicParameters();
			pars.Add("@buildingId", buildingId);
			pars.Add("@types", types);

			var result = await _baseDapperRepository.QueryAsync<EmployeeBuildingViewModel>(query, pars);
			return result;
		}

		public async Task<IEnumerable<EmployeeBuildingViewModel>> GetEmployeesByBuildingIdsDapperAsync(IEnumerable<int> buildingIds, BuildingEmployeeType type)
		{
			var filter = @"WHERE BE.[BuildingId] IN @buildingIds";
			var types = type.GetUniqueFlags();
			if (types.Any())
			{
				filter += " AND BE.[Type] IN @types";
			}

			string query = $@"
                SELECT 
                    E.[Id],
                    E.[Email],
                    CONCAT_WS(' ', EC.[FirstName], EC.[LastName]) AS [Name],
                    R.[Name] AS [RoleName],
                    BE.[Type],
                    BE.BuildingId,
                    (SELECT TOP 1 CP.Phone FROM ContactPhones AS CP WHERE CP.ContactId = EC.ID) AS [Phone]
                FROM
                    [dbo].[Employees] AS E
                    LEFT JOIN [dbo].[Contacts] AS EC ON E.[ContactId] = EC.[Id]
                    LEFT JOIN [dbo].[Roles] AS R ON E.[RoleId] = R.[Id]
                    INNER JOIN [dbo].[BuildingEmployees] AS BE ON E.[Id] = BE.[EmployeeId]
                    {filter} ";

			var pars = new DynamicParameters();
			pars.Add("@buildingIds", buildingIds);
			pars.Add("@types", types);

			var result = await _baseDapperRepository.QueryAsync<EmployeeBuildingViewModel>(query, pars);
			return result;
		}

		public async Task<IEnumerable<int>> GetOpenWorkOrderIds(int buildingId)
		{
			string query = string.Format(@"
                SELECT 
	                [dbo].[WorkOrders].[Id] 
                FROM 
	                [dbo].[WorkOrders]
                WHERE 
	                [dbo].[WorkOrders].[StatusId] <= {0} AND [dbo].[WorkOrders].[BuildingId] = @buildingId ", (int)WorkOrderStatus.Active);

			var pars = new DynamicParameters();
			pars.Add("@buildingId", buildingId);

			var result = await _baseDapperRepository.QueryAsync<int>(query, pars);
			return result;
		}

		public async Task<DataSource<BuildingListBoxViewModel>> ReadAllByContactCboDapperAsync(DataSourceRequest request, int companyId, int? id = null, int? contactId = null)
		{
			var result = new DataSource<BuildingListBoxViewModel>
			{
				Payload = new List<BuildingListBoxViewModel>(),
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
                        B1.ID as ID, 
                        B1.CompanyId as CompanyId,
                        ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY B1.Name, B1.ID ) as [Index]
                    FROM [dbo].[Buildings] as B1 
						INNER JOIN [dbo].Addresses as A ON B1.AddressId = A.ID
						LEFT OUTER JOIN [dbo].[BuildingContacts] as C ON B1.ID = C.[BuildingId]
						WHERE C.[ContactId] = CASE WHEN ISNULL(@contactId, 0) = 0 THEN C.[ContactId] ELSE @contactId END
                    ) payload
                WHERE ID = @id
                END

                SELECT @total = COUNT(*) FROM [dbo].[Buildings] as B2 WHERE B2.CompanyId= @companyId;

                --max(0, @total-@pageSize)
                SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                --safety check
                SELECT @index = ISNULL(@index, 0);

                --min(@index, @maxIndex)
                SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                SELECT 
                    B3.ID as ID,
	                B3.Name as Name,
					A.FullAddress as FullAddress
                        FROM [dbo].[Buildings] as B3 INNER JOIN [dbo].Addresses as A ON B3.AddressId = A.ID
						LEFT OUTER JOIN [dbo].[BuildingContacts] as C ON B3.ID = C.[BuildingId]
                        WHERE B3.[CompanyId]= @companyId AND C.[ContactId] = CASE WHEN ISNULL(@contactId, 0) = 0 THEN C.[ContactId] ELSE @contactId END AND
                    ISNULL(B3.Name, '') +
                    ISNULL(A.FullAddress, '')
                        LIKE '%' + ISNULL(@filter, '') + '%'  
                ORDER BY Name, ID

                OFFSET @index ROWS
                FETCH NEXT @pageSize ROWS ONLY;";

			var pars = new DynamicParameters();
			pars.Add("@id", id);
			pars.Add("@companyId", companyId);
			pars.Add("@filter", request.Filter);
			pars.Add("@pageNumber", request.PageNumber);
			pars.Add("@pageSize", request.PageSize);
			pars.Add("@contactId", contactId);

			var payload = await _baseDapperRepository.QueryAsync<BuildingListBoxViewModel>(query, pars);
			result.Count = payload.FirstOrDefault()?.Count ?? 0;
			result.Payload = payload;

			return result;
		}

		public async Task<IEnumerable<BuildingByOperationsManagerListBoxViewModel>> ReadAllBuildingsByOperationsManagerDapperAsync(DataSourceRequestBuildingsByOperationsManager request, int companyId)
		{
			string conditionalFilter = string.Empty;
			if (request.OperationsManagerId > 0)
			{
				conditionalFilter = @" = @operationsManagerId";
			}
			else
			{
				conditionalFilter = @" <> @currentEmployeeId";
			}

			string query = $@"
                SELECT
	                B.Id,	
	                B.Name,
	                A.FullAddress,
	                IsSupervisor = 
	                    CASE WHEN BE.Type = 1 THEN 1
	                    ELSE  0 END ,

                    IsTemporaryOperationsManager = 
	                    CASE WHEN BE.Type = {(int)BuildingEmployeeType.TemporaryOperationsManager} THEN 1
	                    ELSE 0 END,

	                OperationsManagerFullName = CONCAT_WS(' ', C.FirstName, C.MiddleName, C.LastName) 

                FROM
	                Buildings AS B
	                INNER JOIN BuildingEmployees AS BE ON BE.BuildingId = B.ID 
				                AND ( (BE.EmployeeId <> @currentEmployeeId AND BE.Type = {(int)BuildingEmployeeType.OperationsManager}) 
                                OR (BE.EmployeeId = @currentEmployeeId 
                                    AND BE.Type IN ({(int)BuildingEmployeeType.Supervisor} , {(int)BuildingEmployeeType.TemporaryOperationsManager})) ) 
	                INNER JOIN BuildingEmployees AS BEE ON BEE.BuildingId = B.ID
	                INNER JOIN Employees  AS E ON E.ID = BEE.EmployeeId AND BEE.Type = {(int)BuildingEmployeeType.OperationsManager}
	                LEFT OUTER JOIN Contacts AS C ON C.ID = E.ContactId
	                INNER JOIN Addresses AS A ON B.AddressId= A.ID

                WHERE B.CompanyId = @companyId	
                        AND B.isActive = 1  
	                    AND ( BE.EmployeeId = @currentEmployeeId OR
                            ( BE.EmployeeId {conditionalFilter} AND B.[ID] NOT IN (SELECT BuildingId FROM BuildingEmployees 
                                                                    WHERE EmployeeId = @currentEmployeeId 
                                                                    AND type IN ({(int)BuildingEmployeeType.Supervisor} , {(int)BuildingEmployeeType.TemporaryOperationsManager}) ) ) )
                                                         
	                    AND ISNULL(B.Name, '') + ISNULL(A.FullAddress, '') 
		                LIKE '%' + ISNULL(@filter, '') + '%'	
                ORDER BY IsSupervisor DESC, IsTemporaryOperationsManager DESC, B.[Name]
                OFFSET @pageSize * @pageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY ";

			var pars = new DynamicParameters();
			pars.Add("@filter", request.Filter);
			pars.Add("@companyId", companyId);
			pars.Add("@currentEmployeeId", request.CurrentEmployeeId);
			pars.Add("@operationsManagerId", request.OperationsManagerId);
			pars.Add("@pageNumber", request.PageNumber);
			pars.Add("@pageSize", request.PageSize);

			var result = await _baseDapperRepository.QueryAsync<BuildingByOperationsManagerListBoxViewModel>(query, pars);
			return result;
		}

		/// <summary>
		/// Reads all buildings given a customerId in a "list-box like" format
		/// </summary>
		/// <returns>Buildings ListBox data source.</returns>
		/// <param name="request">Request.</param>
		/// <param name="companyId">Company identifier.</param>
		/// <param name="customerId">Customer identifier.</param>
		/// <param name="id">Identifier.</param>
		public async Task<DataSource<BuildingListBoxViewModel>> ReadAllByCustomerCboDapperAsync(DataSourceRequest request, int companyId, int customerId, int? id)
		{
			var query = @"
                     SELECT 
						B.[ID] as [ID],
						B.[Name] as [Name],
						A.[FullAddress] as [FullAddress],
						B.[Code]
					FROM [dbo].[Buildings] as B 
						INNER JOIN [dbo].Addresses as A ON B.AddressId = A.ID
					WHERE B.[CompanyId] = @companyId
						AND B.[CustomerId] = @customerId
						AND B.[IsActive] = 1
						AND CONCAT(B.[Name], A.[FullAddress]) LIKE CONCAT('%', @filter, '%')
					GROUP BY B.[ID], B.[Name], A.[FullAddress], B.[Code]                   			 
            ";

			var pars = new DynamicParameters();
			pars.Add("@companyId", companyId);
			pars.Add("@customerId", customerId);
			pars.Add("@filter", request.Filter);

			var payload = await this._baseDapperRepository.QueryAsync<BuildingListBoxViewModel>(query, pars);

			var result = new DataSource<BuildingListBoxViewModel>
			{
				Payload = new List<BuildingListBoxViewModel>(),
				Count = 0
			};

			// Paging aftewards
			// TODO: Move this functionality to a common place to be reusable
			result.Count = payload?.Count() ?? 0;
			result.Payload = payload?.Take(request.PageSize);

			if (id != null)
			{
				var listPayload = payload.ToList();
				var item = listPayload.FirstOrDefault(el => el.ID == id);

				var idx = listPayload.IndexOf(item);

				var postDifference = listPayload.Count - idx;

				result.Payload = listPayload.Skip(idx - 1 - postDifference).Take(request.PageSize);
			}

			return result;

		}

		public async Task<DataSource<BuildingReportGridViewModel>> ReadAllWithCustomerDapperAsync(DataSourceRequest request, int companyId, int? isActive = -1, int? isAvailable = -1)
		{
			var result = new DataSource<BuildingReportGridViewModel>
			{
				Payload = new List<BuildingReportGridViewModel>(),
				Count = 0
			};

			string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

			string query = $@"
                    -- payload query
					SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
                        B.ID,
                        CASE WHEN B.Code = '' THEN CAST(B.ID AS VARCHAR(32)) ELSE B.Code END AS Code,
						B.[Name],
						B.[IsActive],
						B.[CompanyId],
                        B.[CreatedDate],
                        B.[EmergencyPhone],
                        B.[EmergencyPhoneExt],
                        B.[EmergencyNotes],
						A.[FullAddress],
						OMC.FirstName + ' ' + OMC.LastName as OperationsManagerFullName,

						-- DUPLICATE QUERIES
                        (select case when  (SELECT top(1) Bb.id
											FROM [dbo].[Contracts] as C INNER JOIN [dbo].[Buildings] as Bb ON C.BuildingId = Bb.ID
										    WHERE 
                                                C.Status = 1 AND 
                                                (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = Bb.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0
                                                and Bb.ID = B.ID
											) is null then 0 else 1 end) as IsComplete,

						-- TODO: For the sake of optimization, re-use the previous query
                        (select case when  (SELECT top(1) Bb.id
											FROM [dbo].[Contracts] as C INNER JOIN [dbo].[Buildings] as Bb ON C.BuildingId = Bb.ID
										    WHERE 
                                                C.Status = 1 AND Bb.IsActive = 1 AND 
                                                (SELECT COUNT(*) FROM BuildingEmployees WHERE BuildingId = Bb.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} ) > 0
                                                and Bb.ID = B.ID
											) is null then 0 else 1 end) as IsAvailable,
                        ISNULL(C.[Name], '') AS ManagementCompanyFullName

	                    FROM [dbo].[Buildings] as B
							INNER JOIN  [dbo].[Addresses] as A ON B.AddressId = A.ID
							LEFT JOIN [Customers] AS C ON C.[ID] = B.[CustomerId]
							LEFT OUTER JOIN [dbo].[Employees] as E on E.ID IN (SELECT EmployeeId FROM BuildingEmployees WHERE BuildingId = B.Id AND [Type] = {(int)BuildingEmployeeType.OperationsManager} )
							LEFT OUTER JOIN [dbo].[Contacts] as OMC on E.ContactId = OMC.ID

                       WHERE B.[CompanyId] = @companyId AND
                         B.[IsActive] = CASE WHEN ISNULL(@isActive, -1) = -1 THEN B.[IsActive] ELSE @isActive END AND
                         ISNULL(B.[Name], '') + ISNULL((OMC.FirstName + ' ' + OMC.LastName), '') +
                         ISNULL(A.[FullAddress], '') +
                         ISNULL(B.[Name], '') LIKE '%' + ISNULL(@filter, '') + '%'
					) payload                         
                       WHERE IsAvailable = CASE WHEN ISNULL(@isAvailable, -1) = -1 THEN IsAvailable ELSE @isAvailable END
                    ORDER BY {orders} IsAvailable DESC, CreatedDate DESC, ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";

			var pars = new DynamicParameters();
			pars.Add("@isActive", isActive);
			pars.Add("@isAvailable", isAvailable);
			pars.Add("@companyId", companyId);
			pars.Add("@filter", request.Filter);
			pars.Add("@pageNumber", request.PageNumber);
			pars.Add("@pageSize", request.PageSize);

			var payload = await _baseDapperRepository.QueryAsync<BuildingReportGridViewModel>(query, pars);

			result.Count = payload.FirstOrDefault()?.Count ?? 0;
			result.Payload = payload;

			return result;
		}

		public async Task UnassignEmployeesByBuildingIdAsync(int buildingId)
		{
			string query = "DELETE FROM BuildingEmployees WHERE BuildingId = @buildingId";
			var pars = new DynamicParameters();
			pars.Add("@buildingId", buildingId);

			await this._baseDapperRepository.ExecuteAsync(query, pars);
		}

		public async Task AssignEmployeesDapperAsync(IEnumerable<EntityEmployee> buildingEmployees)
		{
			if (buildingEmployees == null || !buildingEmployees.Any())
			{
				return;
			}

			var buildingEmployeesList = buildingEmployees.ToList();
			var innerValues = new List<string>();

			var pars = new DynamicParameters();
			for (int i = 0; i < buildingEmployeesList.Count; i++)
			{
				var e = buildingEmployeesList[i];
				pars.Add($"@buildingId{i}", e.EntityId);
				pars.Add($"@employeeId{i}", e.EmployeeId);
				pars.Add($"@type{i}", e.Type);

				innerValues.Add($"(@buildingId{i}, @employeeId{i}, @type{i})");
			}

			string query = $@"
                INSERT INTO BuildingEmployees
                ( BuildingId, EmployeeId, [Type] )
                VALUES {string.Join(",", innerValues)}";

			await this._baseDapperRepository.ExecuteAsync(query, pars);
		}

		public Task<IEnumerable<BuildingWithLocationViewModel>> GetBuildingsWithLocationCboAsync(int companyId)
		{
			var query = @"
                SELECT
                    B.ID,
                    B.Name,
                    B.IsActive,
                    A.Latitude,
                    A.Longitude
                FROM Buildings AS B INNER JOIN Addresses AS A ON B.AddressId = A.ID
                WHERE B.CompanyId = @companyId
            ";

			var pars = new DynamicParameters();
			pars.Add("@companyId", companyId);

			return this._baseDapperRepository.QueryAsync<BuildingWithLocationViewModel>(query, pars);
		}

		public async Task<Building> GetBuildingByContractNumber(string buldingCode)
		{
			var result = new Building();
			var pars = new DynamicParameters();
			string whereQuery = "";

			whereQuery = "  B.Code = @buldingCode";
			pars.Add("@buldingCode", buldingCode);

			string query = $@"
                            select
                                B.ID,
                                B.AddressId,
                                B.CompanyId,
                                B.IsActive,
                                B.Code
                            from Buildings B
                            WHERE { whereQuery }";

			var payload = await _baseDapperRepository.QueryAsync<Building>(query, pars);
			result = payload.FirstOrDefault();
			return result;
		}

		public async Task<IEnumerable<BuildingOperationManagerGridViewModel>> GetSharedBuildingsFromOperationsManagerDapperAsync(DataSourceRequest request, int companyId, int currentOperationsManager, int? operationsManager)
		{
			string query = $@"
            SELECT 
	            B.[ID],
	            B.[Name],
                OP.[Type] AS CurrentType,
	            (CASE WHEN E.[EmployeeId] IS NULL THEN 0 ELSE 1 END) IsShared,
	            (CASE WHEN E.[Type] IS NULL THEN { (int)BuildingEmployeeType.TemporaryOperationsManager } ELSE E.[Type] END) AS [Type]
            FROM [dbo].[Buildings] as B
	            INNER JOIN BuildingEmployees AS OP ON OP.[BuildingId] = B.[ID] AND OP.[Type] IN (2)
	            LEFT JOIN [dbo].[BuildingEmployees] E ON E.BuildingId = B.ID AND E.[Type] IN (2, 4) AND ((@operationsManager IS NOT NULL AND E.EmployeeId = @operationsManager) OR (@operationsManager IS NULL AND E.EmployeeId = 0))
            WHERE 
	            OP.[EmployeeId] = @currentOperationsManager
	            AND B.IsActive = 1 
	            AND B.CompanyId = @companyId
	            AND ((B.[Name] LIKE '%'+@filter+'%') OR (@filter IS NULL))
            ORDER BY B.[ID] DESC";

			var pars = new DynamicParameters();
			pars.Add("@filter", request.Filter);
			pars.Add("@companyId", companyId);
			pars.Add("@currentOperationsManager", currentOperationsManager);
			pars.Add("@operationsManager", operationsManager);

			var result = await _baseDapperRepository.QueryAsync<BuildingOperationManagerGridViewModel>(query, pars);
			return result;
		}
	}
}
