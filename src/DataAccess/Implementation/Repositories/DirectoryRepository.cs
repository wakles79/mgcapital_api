using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Contact;
using MGCap.Domain.ViewModels.Directory;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class DirectoryRepository : BaseRepository<AuditableCompanyEntity, int>, IDirectoryRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;
        
        public DirectoryRepository(MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<IEnumerable<DirectoryReadViewModel>> ReadAllDapperAsync(int companyId, string userEmail)
        {
            string query = @"
                DECLARE @roleLevel AS INT;
                DECLARE @EmployeeId AS INT;

                SELECT 
	                @roleLevel = [dbo].[Roles].Level,
                    @EmployeeId  = [dbo].[Employees].ID 
                FROM 
	                [dbo].[Roles] 
                    INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                WHERE 
	                [dbo].[Employees].[CompanyId] = @companyId AND [dbo].[Employees].[Email] = @userEmail ;


                SELECT * FROM (
	                SELECT 
		                [dbo].[Employees].[ID], 
		                CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [Name], 
		                '1' AS [Type]
	                FROM [dbo].[Employees]
		                LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Contacts].[ID] = [dbo].[Employees].[ContactId]
	                WHERE [dbo].[Employees].[CompanyId] = @companyId
                    AND (@roleLevel <= 20 OR  Employees.ID in (select Distinct EmployeeId from BuildingEmployees where BuildingId in (select BuildingId from BuildingEmployees  where EmployeeId =@EmployeeId)))

	                UNION ALL

	                SELECT 
		                [dbo].[Buildings].[Id], 
		                [dbo].[Buildings].[Name], 
		                '2' AS [Type] 
	                FROM [dbo].[Buildings] 
	                WHERE [dbo].[Buildings].[IsActive] = 1 AND [dbo].[Buildings].[CompanyId] = @companyId
                    AND (@roleLevel <= 20 OR  Buildings.ID in (select BuildingId from BuildingEmployees where EmployeeId =@EmployeeId))

	                UNION ALL
	
	                SELECT 
		                [dbo].[Customers].[ID], 
		                [dbo].[Customers].[Name], 
		                '4' AS [Type]
	                FROM [dbo].[Customers]
	                WHERE @roleLevel <= 20 AND [dbo].[Customers].[CompanyId] = @companyId 
                ) AS q
                ORDER BY q.[Name], q.[Type] DESC  ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@userEmail", userEmail);

            var result = await _baseDapperRepository.QueryAsync<DirectoryReadViewModel>(query, pars, CommandType.Text);
            return result;
        }

        public async Task<DirectoryDetailsEmployeeViewModel> EmployeeDetails(int employeeId)
        {
            string query = @"
                SELECT 
	                [dbo].[Employees].[ID], 
		            CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [Name], 
	                [dbo].[Employees].[Email] AS [UserEmail],
	                [dbo].[Departments].[Name] AS [Department],
	                [dbo].[Roles].[Level] AS [Role],
	                1 AS [Type],
	                [dbo].[Contacts].[DOB] AS [Birthday],
	                [dbo].[Contacts].[Notes],
	                [dbo].[Contacts].[SendNotifications],
	                -- Phones
	                ISNULL([dbo].[ContactPhones].[ID], -1) AS [Id],
	                [dbo].[ContactPhones].[Phone] AS [Data],
	                [dbo].[ContactPhones].[Type] AS [Label],
	                -- Emails
	                ISNULL([dbo].[ContactEmails].[ID], -1) AS [Id],
	                [dbo].[ContactEmails].[Email] AS [Data],
	                [dbo].[ContactEmails].[Type] AS [Label]
                FROM 
	                [dbo].[Employees]
	                LEFT OUTER JOIN [dbo].[Departments] ON [dbo].[Employees].[DepartmentId] = [dbo].[Departments].[ID]
	                LEFT OUTER JOIN [dbo].[Roles] ON [dbo].[Employees].[RoleId] = [dbo].[Roles].[ID]
	                LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]
	                LEFT OUTER JOIN [dbo].[ContactPhones] ON [dbo].ContactPhones.[ContactId] = [dbo].[Contacts].[ID]
	                LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[ContactEmails].[ContactId] = [dbo].[Contacts].[ID]
                WHERE
	                [dbo].[Employees].[ID] = @EmployeeId ";

            var pars = new DynamicParameters();
            pars.Add("@EmployeeId", employeeId);

            var result = await _baseDapperRepository.QueryChildrenListAsync<DirectoryDetailsEmployeeViewModel,
                                                                            DirectoryChildBaseViewModel,
                                                                            DirectoryChildBaseViewModel>(query, pars, CommandType.Text);

            return result.FirstOrDefault();
        }

        public async Task<DirectoryDetailsBuildingViewModel> BuildingDetails(int buildingId, int companyId, string userEmail)
        {
            string query = @"
                DECLARE @roleLevel AS INT;

                SELECT 
	                @roleLevel = [dbo].[Roles].Level 
                FROM 
	                [dbo].[Roles] 
                    INNER JOIN [dbo].[Employees] ON [dbo].[Roles].[ID] = [dbo].[Employees].RoleId 
                WHERE 
	                [dbo].[Employees].[CompanyId] = @companyId AND [dbo].[Employees].[Email] = @userEmail ;
	
                SELECT 
	                [dbo].[Buildings].[ID],
	                [dbo].[Buildings].[Name],
	                IIF(@roleLevel <= 20, [dbo].[Customers].[ID], -1) AS [CustomerId],
	                IIF(@roleLevel <= 20, [dbo].[Customers].[Name], '') AS [CustomerName],
	                [dbo].[Addresses].[FullAddress],
	                [dbo].[Buildings].[EmergencyPhone],
	                [dbo].[Buildings].[EmergencyPhoneExt],
	                -- Operations Managers
	                ISNULL([OMBE].[EmployeeId], -1) AS [Id],
	                CONCAT([OMC].[FirstName], ' ', [OMC].[MiddleName], ' ', [OMC].[LastName]) AS [Data], 
	                'Operations Manager' AS [Label],
	                -- Supervisors
	                ISNULL([SBE].[EmployeeId], -1) AS [Id],
	                CONCAT([SC].[FirstName], ' ', [SC].[MiddleName], ' ', [SC].[LastName]) AS [Data], 
	                'Supervisor' AS [Label],
	                -- Contacts
	                IIF(@roleLevel <= 20, ISNULL([BC].[Id], -1), -1) AS [Id],
	                IIF(@roleLevel <= 20, CONCAT([BC].[FirstName], ' ', [BC].[MiddleName], ' ', [BC].[LastName]), '') AS [Data], 
	                IIF(@roleLevel <= 20, [dbo].[BuildingContacts].[Type], '') AS [Label]
                FROM
	                [dbo].[Buildings]
	                LEFT OUTER JOIN [dbo].[Contracts] ON [dbo].[Buildings].[Id] = [dbo].[Contracts].[BuildingId]
	                LEFT OUTER JOIN [dbo].[Customers] ON [dbo].[Contracts].[CustomerId] = [dbo].[Customers].[Id]
	                LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Buildings].[AddressId] = [dbo].[Addresses].[ID]

	                LEFT OUTER JOIN [dbo].[BuildingEmployees] AS [SBE] ON [dbo].[Buildings].[Id] = [SBE].[BuildingId] AND [SBE].[Type] = @supervisorType
	                LEFT OUTER JOIN [dbo].[Employees] AS [S] ON [SBE].[EmployeeId] = [S].[ID]
	                LEFT OUTER JOIN [dbo].[Contacts] AS [SC] ON [S].[ContactId] = [SC].[ID]
	
	                LEFT OUTER JOIN [dbo].[BuildingEmployees] AS [OMBE] ON [dbo].[Buildings].[Id] = [OMBE].[BuildingId] AND [OMBE].[Type] = @opersManagerType
	                LEFT OUTER JOIN [dbo].[Employees] AS [OM] ON [OMBE].[EmployeeId] = [OM].[ID]
	                LEFT OUTER JOIN [dbo].[Contacts] AS [OMC] ON [OM].[ContactId] = [OMC].[ID]

	                LEFT OUTER JOIN [dbo].[BuildingContacts] ON [dbo].[Buildings].[ID] = [dbo].[BuildingContacts].[BuildingId]
	                LEFT OUTER JOIN [dbo].[Contacts] AS [BC] ON [dbo].[BuildingContacts].[ContactId] = [BC].[ID]
                WHERE
	                [dbo].[Buildings].[ID] = @buildingId ";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@userEmail", userEmail);
            pars.Add("@buildingId", buildingId);
            pars.Add("@supervisorType", BuildingEmployeeType.Supervisor);
            pars.Add("@opersManagerType", BuildingEmployeeType.OperationsManager);

            var result = await _baseDapperRepository.QueryChildrenListAsync<DirectoryDetailsBuildingViewModel, 
                                                                            DirectoryChildBaseViewModel, 
                                                                            DirectoryChildBaseViewModel, 
                                                                            DirectoryChildBaseViewModel>(query, pars, CommandType.Text);
            return result.FirstOrDefault();
        }

        public async Task<DirectoryDetailsCustomerViewModel> CustomerDetails(int customerId)
        {
            string query = @"
                SELECT 
	                [dbo].[Customers].[ID],
	                [dbo].[Customers].[Name],
	                [dbo].[Customers].[Notes],
	                -- Buildings
	                ISNULL([dbo].[Buildings].[ID], -1) AS [Id],
	                [dbo].[Buildings].[Name],
	                -- Phones
	                ISNULL([dbo].[Contacts].[ID], -1) AS [Id],
	                CONCAT([Contacts].[FirstName], ' ', [Contacts].[MiddleName], ' ', [Contacts].[LastName]) AS [Data], 
	                [dbo].[CustomerContacts].[Type] AS [Label]
                FROM
	                [dbo].[Customers]
	                LEFT OUTER JOIN [dbo].[Contracts] ON [dbo].[Customers].[Id] = [dbo].[Contracts].[CustomerId]
	                LEFT OUTER JOIN [dbo].[Buildings] ON [dbo].[Buildings].[Id] = [dbo].[Contracts].[BuildingId]
	                LEFT OUTER JOIN [dbo].[CustomerContacts] ON [dbo].[Customers].[ID] = [dbo].[CustomerContacts].[CustomerId]
	                LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[CustomerContacts].[ContactId] = [dbo].[Contacts].[ID]
                WHERE
	                [dbo].[Customers].[ID] = @customerId ";

            var pars = new DynamicParameters();
            pars.Add("@customerId", customerId);

            var result = await _baseDapperRepository.QueryChildrenListAsync<DirectoryDetailsCustomerViewModel,
                                                                            DirectoryEntityBaseViewModel,
                                                                            DirectoryChildBaseViewModel>(query, pars, CommandType.Text);
            return result.FirstOrDefault();
        }

        public async Task<DirectoryDetailsContactViewModel> ContactDetails(int contactId)
        {
            string query = @"
                 SELECT 
	                [dbo].[Contacts].[ID],
	                CONCAT([Contacts].[FirstName], ' ', [Contacts].[MiddleName], ' ', [Contacts].[LastName]) AS [Name], 
	                [dbo].[Contacts].[DOB] AS [Birthday], 
	                [dbo].[Contacts].[Notes],
	                -- Phones
	                ISNULL([dbo].[ContactPhones].[ID], -1) AS [Id],
	                [dbo].[ContactPhones].[Type] AS [Label],
	                [dbo].[ContactPhones].[Phone] AS [Data],
	                -- Emails
	                ISNULL([dbo].[ContactEmails].[ID], -1) AS [Id],
	                [dbo].[ContactEmails].[Type] AS [Label],
	                [dbo].[ContactEmails].[Email] AS [Data],
	                -- Buildings
	                ISNULL([dbo].[BuildingContacts].[BuildingId], -1) AS [Id],
	                [dbo].[BuildingContacts].[Type] AS [Label],
	                [dbo].[Buildings].[Name] AS [Data],
	                -- Customers
	                ISNULL([dbo].[CustomerContacts].[CustomerId], -1) AS [Id],
	                [dbo].[CustomerContacts].[Type] AS [Label],
	                [dbo].[Customers].[Name] AS [Data]
                FROM
	                [dbo].[Contacts]
	                LEFT OUTER JOIN [dbo].[ContactPhones] ON [dbo].[Contacts].[Id] = [dbo].[ContactPhones].[ContactId]
	                LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[Contacts].[Id] = [dbo].[ContactEmails].[ContactId]
	                LEFT OUTER JOIN [dbo].[BuildingContacts] ON [dbo].[Contacts].[Id] = [dbo].[BuildingContacts].[ContactId]
	                LEFT OUTER JOIN [dbo].[CustomerContacts] ON [dbo].[Contacts].[Id] = [dbo].[CustomerContacts].[ContactId]
	                LEFT OUTER JOIN [dbo].[Buildings] ON [dbo].[BuildingContacts].[BuildingId] = [dbo].[Buildings].[ID]
	                LEFT OUTER JOIN [dbo].[Customers] ON [dbo].[CustomerContacts].[CustomerId] = [dbo].[Customers].[ID]
                WHERE
	                [dbo].[Contacts].[ID] = @contactId ";

            var pars = new DynamicParameters();
            pars.Add("@contactId", contactId);

            var result = await _baseDapperRepository.QueryChildrenListAsync<DirectoryDetailsContactViewModel,
                                                                            DirectoryChildBaseViewModel,
                                                                            DirectoryChildBaseViewModel,
                                                                            DirectoryChildBaseViewModel,
                                                                            DirectoryChildBaseViewModel>(query, pars, CommandType.Text);
            return result.FirstOrDefault();
        }
    }
}
