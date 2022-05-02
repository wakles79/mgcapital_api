using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.CustomerUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class CustomerUserRepository : BaseRepository<CustomerUser, int>, ICustomerUserRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public CustomerUserRepository(MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public override async Task<CustomerUser> AddAsync(CustomerUser obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("The given object must not be null");
            }

            obj.BeforeCreate(obj.Email, obj.CompanyId);

            var dbContext = this.DbContext as MGCapDbContext;
            var role = dbContext.Roles.FirstOrDefault(r => r.Level.Equals((int)CustomerUserRole.Customer));
            if (role != null)
            {
                obj.RoleId = role.ID;
            }

            await this.Entities.AddAsync(obj);
            return obj;
        }

        public async Task<CustomerContactExistsViewModel> GetCustomerContactByEmail(string email)
        {
            string query = @"
                SELECT
                    TOP 1
	                ISNULL([dbo].[ContactEmails].[Id], -1) AS [ContactEmailId],
	                ISNULL([dbo].[CustomerContacts].[ContactId], -1) AS [ContactId],
	                ISNULL([dbo].[Customers].[Id], -1) AS [CustomerId]
                FROM
	                [dbo].[ContactEmails]
	                INNER JOIN [dbo].[CustomerContacts] ON [dbo].[ContactEmails].[ContactId] = [dbo].[CustomerContacts].[ContactId]
	                INNER JOIN [dbo].[Contacts] ON [dbo].[CustomerContacts].[ContactId] = [dbo].[Contacts].[Id]
	                INNER JOIN [dbo].[Customers] ON [dbo].[CustomerContacts].[CustomerId] = [dbo].[Customers].[Id]
                WHERE
	                [dbo].[ContactEmails].[Email] = @userEmail 
	                AND [dbo].[Contacts].[CompanyId] = 1 ";

            var pars = new DynamicParameters();
            pars.Add("@userEmail", email);

            var result = await _baseDapperRepository.QuerySingleOrDefaultAsync<CustomerContactExistsViewModel>(query, pars);
            return result ?? new CustomerContactExistsViewModel { ContactEmailId = -1, ContactId = -1, CustomerId = -1 };
        }

        public async Task<CustomerLoginResponseViewModel> GetWithCompanyDapperAsync(string userEmail)
        {
            string query = @"
                SELECT 
	                [dbo].[CustomerUsers].[Id] AS [CustomerUserId],
	                [dbo].[CustomerUsers].[Email] AS [Email],
	                [dbo].[CustomerUsers].[FirstName] AS [FirstName],
	                [dbo].[CustomerUsers].[MiddleName] AS [MiddleName],
	                [dbo].[CustomerUsers].[LastName] AS [LastName],
	                [dbo].[CustomerUsers].[ContactId] AS [ContactId],
	                [dbo].[CustomerUsers].[CompanyId] AS [CompanyId],
	                [dbo].[Companies].[Name] AS [CompanyName],
	                [dbo].[Roles].[Level] AS [RoleLevel]
                FROM
	                [dbo].[CustomerUsers]
	                INNER JOIN [dbo].[Contacts] ON [dbo].[CustomerUsers].[ContactId] = [dbo].[Contacts].[Id]
	                INNER JOIN [dbo].[Companies] ON [dbo].[CustomerUsers].[CompanyId] = [dbo].[Companies].[Id]
	                LEFT OUTER JOIN [dbo].[Roles] ON [dbo].[CustomerUsers].[RoleId] = [dbo].[Roles].[Id]
                WHERE 
	                [dbo].[CustomerUsers].[Email] = @userEmail ";

            var pars = new DynamicParameters();
            pars.Add("@userEmail", userEmail);

            var result = await _baseDapperRepository.QuerySingleOrDefaultAsync<CustomerLoginResponseViewModel>(query, pars);
            return result;
        }

        public async Task<CustomerUserDetailsViewModel> SingleOrDefaultByIdDapperAsync(int userId, int companyId)
        {
            string query = @"
            SELECT
	            [dbo].[CustomerUsers].[Id] AS [Id],
	            [dbo].[CustomerUsers].[Email] AS [Email],
	            [dbo].[Contacts].[FirstName] AS [FirstName],
	            [dbo].[Contacts].[MiddleName] AS [MiddleName],
	            [dbo].[Contacts].[LastName] AS [LastName]
            FROM
	            [dbo].[CustomerUsers]
	            LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[CustomerUsers].[ContactId] = [dbo].[Contacts].[Id]
            WHERE
	            [dbo].[CustomerUsers].[Id] = @userId AND [dbo].[CustomerUsers].[CompanyId] = @companyId ";

            var pars = new DynamicParameters();
            pars.Add("@userId", userId);
            pars.Add("@companyId", companyId);

            var result = await _baseDapperRepository.QuerySingleOrDefaultAsync<CustomerUserDetailsViewModel>(query, pars);
            return result;
        }
    }
}
