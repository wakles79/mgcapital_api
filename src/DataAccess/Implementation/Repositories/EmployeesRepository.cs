// -----------------------------------------------------------------------
// <copyright file="EmployeesRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.Role;
using MGCap.Domain.ViewModels.WorkOrder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Employee"/>
    /// </summary>
    public class EmployeesRepository : BaseRepository<Employee, int>, IEmployeesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;
        private readonly IPermissionRolesRepository PermissionRolesRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmployeesRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public EmployeesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository, IPermissionRolesRepository permissionsRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
            PermissionRolesRepository = permissionsRepository;
        }

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="Employee"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="employeeEmail">The current logged in user Email</param>
        /// <returns>A list with all the Employees with the given Email</returns>
        public Task<List<Employee>> ReadWithCompanyAsync(string employeeEmail)
        {
            return Entities
                .Include(ent => ent.Contact)
                    .ThenInclude(c => c.Addresses)
                .Include(ent => ent.Contact)
                    .ThenInclude(c => c.Emails)
                .Include(ent => ent.Contact)
                    .ThenInclude(c => c.Phones)
                .Include(entity => entity.Company)
                .Include(ent => ent.Role)
                .Where(ent => ent.Email == employeeEmail)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeLoginReponseViewModel>> ReadWithCompanyDapperAsync(string employeeEmail)
        {
            string query = string.Format(@"
                SELECT 
                    [dbo].[Employees].[Id] AS [EmployeeId],
                    [dbo].[Employees].[Guid] AS [EmployeeGuid],
                    [dbo].[Employees].[Email] AS [EmployeeEmail],
                    [dbo].[Roles].[Level] AS [RoleLevel],
                    [dbo].[Roles].[Name] AS [RoleName],
                    [dbo].[Roles].[ID] AS [RoleId],
                    CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName],
                    [dbo].[Companies].[Id] AS [CompanyId],
                    [dbo].[Companies].[Name] AS [CompanyName],
                    [dbo].[CompanySettings].[LogoFullUrl] AS [CompanyLogoUrl]
                FROM
                    [dbo].[Employees]
	                INNER JOIN [dbo].[Roles] ON [dbo].[Employees].[RoleId] = [dbo].[Roles].[ID]
                    INNER JOIN [dbo].[Companies] ON [dbo].[Employees].[CompanyId] = [dbo].[Companies].[Id]
                    INNER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[Id]
                    LEFT JOIN [dbo].[CompanySettings] ON [dbo].[CompanySettings].[CompanyId] = [dbo].[Companies].[Id]
                WHERE
                    [dbo].[Employees].[Email] = @useremail  ");

            var pars = new DynamicParameters();
            pars.Add("@useremail", employeeEmail);

            var result = await _baseDapperRepository.QueryAsync<EmployeeLoginReponseViewModel>(query, pars);
            foreach (var Emp in result)
            {
                Emp.Permissions = await PermissionRolesRepository.ReadAllByRoleDapperAsync(Emp.RoleId);
            }
            return result;
        }

        public Employee AssignRolePermissions(Employee obj, Func<Role, bool> func = null)
        {
            func = func ?? new Func<Role, bool>(r => r.Level == 20);
            var dbContext = this.DbContext as MGCapDbContext;
            var role = dbContext.Roles.FirstOrDefault(func);
            if (role == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            // Sets roleId to employee
            obj.RoleId = role.ID;

            var query = @"
                -- DELETE ALL EXISTING PERMS
                DELETE FROM [dbo].[EmployeePermissions] WHERE EmployeeId = @employeeId;

                -- INSERT PERMS TEMPLATE
                INSERT INTO [dbo].[EmployeePermissions] (EmployeeId, PermissionId)
    	            SELECT @employeeId, PermissionId FROM PermissionRoles WHERE RoleId = @roleId;
                        ";
            var pars = new DynamicParameters();
            pars.Add("@employeeId", obj.ID);
            pars.Add("@roleId", role.ID);

            this._baseDapperRepository.Execute(query, pars);

            return obj;
        }

        public override async Task<Employee> AddAsync(Employee obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            var result = await base.AddAsync(obj);
            //this.AssignRolePermissions(obj);
            return result;
        }

        public override Employee Add(Employee obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }
            var result = base.Add(obj);
            //this.AssignRolePermissions(obj);
            return result;
        }

        private string GetEmployeeIdQuery()
        {
            return @"
                SELECT 
                    ISNULL([dbo].[Employees].[Id], -1) AS [Id] 
                FROM 
                    [dbo].[Employees] 
                WHERE 
                    [dbo].[Employees].[Email] = @email AND [dbo].[Employees].[CompanyId] = @companyId ";
        }

        public async Task<int> GetEmployeeIdByEmailAndCompanyIdDapperAsync(string email, int companyId)
        {
            string query = this.GetEmployeeIdQuery();

            var pars = new DynamicParameters();
            pars.Add("@email", email);
            pars.Add("@companyId", companyId);

            int? employeeId = await _baseDapperRepository.QuerySingleOrDefaultAsync<int>(query, pars);
            return employeeId ?? -1;
        }

        public int GetEmployeeIdByEmailAndCompanyIdDapper(string email, int companyId)
        {
            string query = this.GetEmployeeIdQuery();

            var pars = new DynamicParameters();
            pars.Add("@email", email);
            pars.Add("@companyId", companyId);

            int? employeeId = _baseDapperRepository.QuerySingleOrDefault<int>(query, pars);
            return employeeId ?? -1;
        }

        public override async Task<Employee> SingleOrDefaultAsync(Func<Employee, bool> filter)
        {
            return await Entities
                .Include(ent => ent.Contact)
                    .ThenInclude(c => c.Addresses)
                        .ThenInclude(c => c.Address)
                .Include(ent => ent.Contact)
                    .ThenInclude(c => c.Emails)
                .Include(ent => ent.Contact)
                    .ThenInclude(c => c.Phones)
                .Include(entity => entity.Company)
                .Include(entity => entity.Department)
                .Include(entity => entity.Role)
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        public async Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int companyId, int? id = null, int? employeeId = null, int? roleLevel = null)
        {
            // TODO: Refactor this
            var result = new DataSource<EmployeeListBoxViewModel>
            {
                Payload = new List<EmployeeListBoxViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string subSelect = string.Empty;
            string subJoinRole = string.Empty;
            string subWhereRole = string.Empty;
            string subWhereRoleLevel = string.Empty;

            string selectEmployees = string.Empty;

            subJoinRole = " INNER JOIN [dbo].[Roles] ON [dbo].[Roles].Id = [dbo].[Employees].RoleId ";

            if (employeeId.HasValue)
            {
                subSelect = $@" 
                    DECLARE @roleLevel INT;

                    SELECT @roleLevel = [dbo].[Roles].Level 
                    FROM [dbo].[Roles] 
                        INNER JOIN [dbo].[Employees] ON [dbo].[Roles].Id = [dbo].[Employees].RoleId 
                    WHERE [dbo].[Employees].Id = {employeeId.Value} ";

                subWhereRole = " AND [dbo].[Roles].Level >= @roleLevel ";
            }

            if (roleLevel.HasValue)
            {
                subWhereRoleLevel = $@"AND [Level] = @roleLevelE";

                selectEmployees = $@"
                    DECLARE @mismatch AS int;
                    SELECT @mismatch = (SELECT 
						                    [dbo].[Employees].[ID] as ID
					                    FROM [dbo].[Employees]
						                    INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
					                    WHERE [dbo].[Employees].[ID] = @id AND [dbo].[Roles].[Level] = @roleLevelE)


                    -- Always selecting exising employee (even though can be queried in last query, we use set union)
                    SELECT 
                      [dbo].[Employees].[ID] as ID,
                      [dbo].[Employees].[Email] as Email,
                      CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[LastName], 
		                    CASE WHEN @mismatch is null THEN ' [ROLE MISMATCH]' ELSE '' END
		                    ) AS [Name],
                      [dbo].[Roles].[Level],
                      [dbo].[Roles].[Name] as RoleName          
                    FROM [dbo].[Employees]
                    INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
                    INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
                      WHERE [dbo].[Employees].[ID] = @id

                    UNION

                    -- Selects all employees that match companyId and roleLevel (in case @roleLevelE is passed as parameter)
                    SELECT 
                      [dbo].[Employees].[ID] as ID,
                      [dbo].[Employees].[Email] as Email,
                      CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[LastName]) as Name,
                      [dbo].[Roles].Level,
                      [dbo].[Roles].[Name] as RoleName  
                    FROM [dbo].[Employees]
                    INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
                    INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
                    WHERE [dbo].[Employees].[CompanyId]= @companyId AND Roles.[Level] = @roleLevelE AND
                      ISNULL([dbo].[Contacts].[FirstName], '') +
                      ISNULL([dbo].[Contacts].[MiddleName], '') +
                      ISNULL([dbo].[Contacts].[LastName], '') 
                        LIKE '%' + ISNULL(@filter, '') + '%' 
	                    AND [dbo].Employees.ID <> ISNULL(@mismatch, 0) ";
            }
            else
            {
                selectEmployees = $@"
                        SELECT 
                            [dbo].[Employees].[ID] as ID,
                            [dbo].[Employees].[Email] as Email,
                            CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[MiddleName]+' ',[dbo].[Contacts].[LastName]+' ') as Name,
                            [dbo].[Roles].[Level],
                            [dbo].[Roles].[Name] as RoleName
                        FROM [dbo].[Employees]
                        INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId] 
                        {subJoinRole}    
                        WHERE [dbo].[Employees].[CompanyId]= @companyId {subWhereRole} AND 
                            ISNULL([dbo].[Contacts].[FirstName], '') +
                            ISNULL([dbo].[Contacts].[MiddleName], '') +
                            ISNULL([dbo].[Contacts].[LastName], '') 
                                LIKE '%' + ISNULL(@filter, '') + '%' ";
            }

            string query = $@"
                        declare @index int 
                        declare @maxIndex int;
                        declare @total int;

                        {subSelect} 

                        IF ISNULL(@id, 0) <= 0 OR ISNULL(@filter, '') <> ''
                        BEGIN
                            select @index =  @pageNumber;
                        END
                        ELSE
                        BEGIN
                        SELECT @index = [Index] - 1 FROM ( 
                            SELECT 
                                [dbo].[Employees].ID,
                                [dbo].[Employees].CompanyId, 
                                R.[Level], 
                                ROW_NUMBER() OVER (PARTITION BY [dbo].[Employees].CompanyId Order BY ISNULL([dbo].[Contacts].[FirstName], '') + ISNULL([dbo].[Contacts].[MiddleName], '') + ISNULL([dbo].[Contacts].[LastName], ''), [dbo].[Employees].ID) as [Index]
                            FROM [dbo].[Employees]
                            INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
                            INNER JOIN [dbo].[Roles] AS R ON R.ID = Employees.RoleId
                            ) payload
                        WHERE ID = @id {subWhereRoleLevel}
                        END

                        SELECT @total = COUNT(*) FROM [dbo].[Employees] WHERE [dbo].[Employees].[CompanyId]= @companyId;

                        --max(0, @total-@pageSize)
                        SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                        --safety check
                        SELECT @index = ISNULL(@index, 0);

                        --min(@index, @maxIndex)
                        SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                        {selectEmployees}

                        ORDER BY Name, [dbo].[Employees].ID
                        OFFSET @index ROWS
                        FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@id", id);
            pars.Add("@roleLevelE", roleLevel);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<EmployeeListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<EmployeebBuildingListBoxViewModel>> ReadAllByBuildingCboDapperAsync(int buildingId)
        {
            var result = new DataSource<EmployeebBuildingListBoxViewModel>
            {
                Payload = new List<EmployeebBuildingListBoxViewModel>(),
                Count = 0
            };

            string query = $@"
            SELECT 
	            E.[ID],
	            CONCAT(C.FirstName, ' ', C.LastName) AS [Name],
	            R.[Name] AS [RoleName],
	            E.[Email],
                BE.[Type]
            FROM [BuildingEmployees] AS BE
	            INNER JOIN [Employees] AS E ON E.[ID] = BE.[EmployeeId]
	            INNER JOIN [Contacts] AS C ON C.[ID] = E.[ContactId]
	            INNER JOIN [Roles] AS R ON R.[ID] = E.[RoleId]
            WHERE BE.[BuildingId] = @BuildingId
            ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@BuildingId", buildingId);

            var rows = await this._baseDapperRepository.QueryAsync<EmployeebBuildingListBoxViewModel>(query, parameters);
            result.Count = rows?.Count() ?? 0;
            result.Payload = rows;

            return result;
        }

        public async Task<DataSource<EmployeeListBoxViewModel>> ReadAllSupervisorsCboDapperAsync(DataSourceRequest request, int companyId, int? buildingId)
        {
            var result = new DataSource<EmployeeListBoxViewModel>
            {
                Payload = new List<EmployeeListBoxViewModel>(),
                Count = 0
            };

            // TODO: Do the [RoleMismatch] thing
            string query = $@"
                SELECT * FROM (

                    SELECT E.ID, 
                          CONCAT(C.FirstName, ' ', C.MiddleName, ' ', C.LastName) AS [Name],
                          R.Name AS RoleName,
                          R.Level AS RoleLevel,
                          0 AS Selected
                    FROM Employees AS E 
                        INNER JOIN Contacts AS C ON E.ContactId = C.ID
                        INNER JOIN BuildingEmployees AS BE ON BE.EmployeeId = E.ID AND BE.Type = {(int)BuildingEmployeeType.Supervisor}
                        INNER JOIN Buildings AS B ON BE.BuildingId = B.ID
                        INNER JOIN Roles AS R ON R.ID = E.RoleId
                    WHERE B.ID = @buildingId

                    UNION 

                    SELECT E.ID, 
                          CONCAT(C.FirstName, ' ', C.MiddleName, ' ', C.LastName) AS [Name],
                          R.Name AS RoleName,
                          R.Level AS RoleLevel,
                          1 AS Selected
                    FROM Employees AS E 
                        INNER JOIN Contacts AS C ON E.ContactId = C.ID
                        INNER JOIN Roles AS R ON E.RoleId = R.ID
                    WHERE E.CompanyId = @companyId AND R.[Level] >= {(int)EmployeeRole.Supervisor}
                    AND E.ID NOT IN 
                            (SELECT EmployeeId
                             FROM BuildingEmployees 
                             WHERE BuildingId = @buildingId )                                       
                    ) Q
					WHERE Q.Name LIKE '%' + ISNULL(@filter, '') + '%'
                    ORDER BY Q.Selected, Q.Name, Q.ID

            ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@buildingId", buildingId);
            pars.Add("@filter", request.Filter);

            var payload = await _baseDapperRepository.QueryAsync<EmployeeWithRoleLevelListBoxViewModel>(query, pars);
            if (payload != null && payload.Any())
            {
                foreach (var item in payload)
                {
                    if (item.RoleLevel != (int)EmployeeRole.Supervisor)
                    {
                        item.Name += " [ROLE MISMATCH]";
                    }
                }
            }
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<string>> GetPermissionsAsync(int companyId, string email)
        {
            var result = new List<string>();
            string query = @"
                    SELECT p.[Name] FROM [dbo].[Employees] AS e
                        INNER JOIN [dbo].[EmployeePermissions] AS ep ON e.ID = ep.EmployeeId
                        INNER JOIN [dbo].[PermissionRoles] AS pr ON e.RoleId = pr.RoleId 
                        INNER JOIN [dbo].[Permissions] AS p ON p.ID = ep.PermissionId OR p.ID = pr.PermissionId
                    WHERE e.CompanyId = @companyId AND e.Email = @email GROUP BY p.[Name]";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@email", email);

            result = (await _baseDapperRepository.QueryAsync<string>(query, pars))?.ToList();

            return result;
        }

        public DataSource<RoleListBoxViewModel> ReadAllCboRoles()
        {
            var dbContext = this.DbContext as MGCapDbContext;

            var result = new DataSource<RoleListBoxViewModel>
            {
                Payload = new List<RoleListBoxViewModel>(),
                Count = 0
            };
            result.Payload = dbContext.Roles.OrderBy(r => r.Level).ThenBy(r => r.ID).Select(r => new RoleListBoxViewModel { ID = r.ID, Name = r.Name, Level = r.Level });
            result.Count = result.Payload.Count();
            return result;
        }

        public async Task<DataSource<EmployeeGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int companyId, int? roleLevel = null, int? roleId = null)
        {

            var result = new DataSource<EmployeeGridViewModel>
            {
                Payload = new List<EmployeeGridViewModel>(),
                Count = 0
            };
            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string query = $@" 
                --payload query 
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
	                        SELECT 
						        E.ID,
                                E.Guid,
							    CONCAT(C.FirstName, ' ', C.LastName) as FullName,
							    E.Email,
							    R.Name as RoleName,	
                                R.Level as RoleLevel,
							    (SELECT TOP 1 CP.Phone
							     from [dbo].ContactPhones AS CP
                                    WHERE CP.[Default] = 1 AND	CP.ContactId = E.ContactId
							    ) as Phone,
							    E.CreatedDate,
                                E.EmployeeStatusId,
                                D.Name as DepartmentName

	                        FROM [dbo].[Employees] as E 
						    INNER JOIN [dbo].Contacts as C ON E.ContactId = C.ID	
						    INNER JOIN [dbo].Roles as R ON E.RoleId = R.ID
						    LEFT JOIN [dbo].Departments as D ON D.ID = E.DepartmentId		

					       WHERE E.[CompanyId] = @companyId 
                           AND R.Level = CASE WHEN @roleLevel IS NULL THEN R.Level ELSE @roleLevel END
                           AND R.ID = CASE WHEN @roleId IS NULL THEN R.ID ELSE @roleId END
                           AND CONCAT(C.FirstName, C.MiddleName, C.LastName, E.Email, R.Name) LIKE '%' + ISNULL(@filter, '') + '%'
					     ) payload 
                        
                        ORDER BY {orders} FullName ASC, ID
                        OFFSET @pageSize * @pageNumber ROWS
                        FETCH NEXT @pageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@roleLevel", roleLevel);
            pars.Add("@roleId", roleId);

            var payload = await _baseDapperRepository.QueryAsync<EmployeeGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<int> GetCurrentEmployeeIdAsync(string userEmail, int companyId)
        {
            string query = @"
                SELECT TOP 1 ID FROM Employees WHERE Email=@email AND CompanyId=@companyId;
            ";

            var pars = new DynamicParameters();
            pars.Add("@email", userEmail);
            pars.Add("@companyId", companyId);

            var res = await this._baseDapperRepository.QuerySingleOrDefaultAsync<int>(query, pars);
            return res;
        }

        public async Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int companyId, ComparisonPredicate comparisonPredicate, int roleLevel, int? id = null)
        {
            // TODO: Refactor this
            var result = new DataSource<EmployeeListBoxViewModel>
            {
                Payload = new List<EmployeeListBoxViewModel>(),
                Count = 0
            };

            var operatorValue = ComparisonUtils.GetComparisonOperator(comparisonPredicate);

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string subSelect = string.Empty;
            string subJoinRole = string.Empty;
            string subWhereRole = string.Empty;
            string subWhereRoleLevel = string.Empty;

            string query = $@"
                       declare @index int 
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
                                [dbo].[Employees].ID,
                                [dbo].[Employees].CompanyId, 
                                R.[Level], 
                                ROW_NUMBER() OVER (PARTITION BY [dbo].[Employees].CompanyId Order BY ISNULL([dbo].[Contacts].[FirstName], '') + ISNULL([dbo].[Contacts].[MiddleName], '') + ISNULL([dbo].[Contacts].[LastName], ''), [dbo].[Employees].ID) as [Index]
                            FROM [dbo].[Employees]
                            INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
                            INNER JOIN [dbo].[Roles] AS R ON R.ID = Employees.RoleId
                            ) payload
                        WHERE ID = @id AND [Level] {operatorValue} @roleLevel
                        END

                        SELECT @total = COUNT(*) FROM [dbo].[Employees] WHERE [dbo].[Employees].[CompanyId]= @companyId;

                        --max(0, @total-@pageSize)
                        SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                        --safety check
                        SELECT @index = ISNULL(@index, 0);

                        --min(@index, @maxIndex)
                        SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                    -- Selects all employees that match companyId and roleLevel (in case @roleLevelE is passed as parameter)
                    SELECT 
                      [dbo].[Employees].[ID] as ID,
                      CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[LastName]) as Name,
                      [dbo].[Roles].Level,
                      [dbo].[Roles].[Name] as RoleName  
                    FROM [dbo].[Employees]
                    INNER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
                    INNER JOIN [dbo].[Roles] ON [dbo].[Roles].ID = Employees.RoleId
                    WHERE [dbo].[Employees].[CompanyId]= @companyId AND Roles.[Level] {operatorValue} @roleLevel AND
                      CONCAT([dbo].[Contacts].[FirstName], [dbo].[Contacts].[MiddleName], [dbo].[Contacts].[LastName])
                        LIKE  + CONCAT('%', @filter, '%')  

                        ORDER BY Name, [dbo].[Employees].ID
                        OFFSET @index ROWS
                        FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@id", id);
            pars.Add("@roleLevelE", roleLevel);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);
            pars.Add("@roleLevel", roleLevel);

            var payload = await _baseDapperRepository.QueryAsync<EmployeeListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<EmployeeUpdateViewModel> SingleOrDefaultDapperAsync(int employeeId, int companyId)
        {

            string query = $@"
                            SELECT 
	                            E.ID,
	                            E.Guid,
	                            E.Email,
	                            E.EmployeeStatusId,
	                            E.DepartmentId,
	                            E.ContactId,
                                E.RoleId,
                                E.Code,
	                            C.FirstName,
	                            C.MiddleName,
	                            C.LastName,
	                            C.Salutation,
	                            C.Gender,
	                            C.DOB,
	                            C.Notes	
                            FROM Employees AS E 
	                            INNER JOIN Contacts AS C ON E.ContactId = C.ID
                            WHERE E.CompanyId = @companyId AND
                                  E.ID = @employeeId";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@employeeId", employeeId);

            var result = await _baseDapperRepository.QuerySingleOrDefaultAsync<EmployeeUpdateViewModel>(query, pars);
            return result;
        }

        public async Task<IEnumerable<WorkOrderContactViewModel>> ReadAllOfficeStaffOrMastersDapperAsync(int companyId)
        {
            string query = @"
                SELECT 
                    CONCAT(eContact.FirstName, ' ', eContact.LastName) AS FullName,
                    E.Email,
                    'Office Staff' AS Type,
                    eContact.SendNotifications AS SendNotifications
                FROM 
                    Employees AS E 
                        INNER JOIN Roles R on E.RoleId = R.ID 
                        INNER JOIN Contacts AS eContact ON eContact.ID = E.ContactId
                WHERE R.Level <= @minRoleLevel AND E.CompanyId = @companyId ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@minRoleLevel", (int)EmployeeRole.Office_Staff);

            var result = await _baseDapperRepository.QueryAsync<WorkOrderContactViewModel>(query, pars);

            return result;
        }

        public async Task<IEnumerable<EmployeeListBoxViewModel>> ReadAllWorkOrderEmployeesAsync(int workOrderId)
        {
            string query = @"
                SELECT 
	                E.[ID],
	                CONCAT(C.FirstName, ' ', C.LastName) AS [Name],
	                R.[Name] AS [RoleName],
                    R.[Level],
	                E.[Email]
                FROM [WorkOrderEmployees] AS WE
	                INNER JOIN [Employees] AS E ON [E].[ID] = [WE].[EmployeeId]
	                INNER JOIN [Contacts] AS C ON [C].ID = [E].[ContactId]
	                INNER JOIN [Roles] AS R ON [R].[ID] = [E].[RoleId]
                WHERE [WE].[WorkOrderId] = @WorkOrderId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@WorkOrderId", workOrderId);
            var rows = await this._baseDapperRepository.QueryAsync<EmployeeListBoxViewModel>(query, parameters);

            return rows.AsEnumerable();
        }

        public Task<Employee> GetByEmailAndCompanyAsync(string email, int companyId)
        {
            string query = $"SELECT TOP 1 E.* FROM [Employees] AS E WHERE E.Email = @Email AND E.CompanyId = @CompanyId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@CompanyId", companyId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<Employee>(query, parameters);
        }

        public Task<Role> GetEmployeeRoleAsync(int companyId, string userEmail)
        {
            var query = @"
                SELECT TOP 1 R.* 
                FROM [Employees] AS E 
                    INNER JOIN [Roles] AS R ON E.RoleId = R.ID
                WHERE E.Email = @Email AND E.CompanyId = @CompanyId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Email", userEmail);
            parameters.Add("@CompanyId", companyId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<Role>(query, parameters);
        }

        public Task<string> GetEmailSignatureAsync(string userEmail, int companyId)
        {
            var query = @"
                    SELECT TOP 1 E.[EmailSignature]
                    FROM [Employees] AS E 
                    WHERE E.Email = @Email AND E.CompanyId = @CompanyId
                    ";
            var pars = new DynamicParameters();
            pars.Add("@Email", userEmail);
            pars.Add("@CompanyId", companyId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<string>(query, pars);
        }

        public Task<string> GetCompanyEmailSignatureAsync(int companyId)
        {
            var query = @"
                SELECT TOP 1 CS.[EmailSignature]
                FROM CompanySettings AS CS 
                WHERE CS.CompanyId = @CompanyId
            ";

            var pars = new DynamicParameters();
            pars.Add("@CompanyId", companyId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<string>(query, pars);
        }
    }
}
