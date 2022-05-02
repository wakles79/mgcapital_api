// -----------------------------------------------------------------------
// <copyright file="CustomersRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.DataViewModels.Customer;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.Customer;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Customer"/>
    /// </summary>
    public class CustomersRepository : BaseRepository<Customer, int>, ICustomersRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public CustomersRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table <see cref="Customer"/> based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public override async Task<IQueryable<Customer>> ReadAllAsync(Func<Customer, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Entities
                        .Include(ent => ent.Addresses)
                            .ThenInclude(ent => ent.Address)
                        .Include(ent => ent.Phones)
                        .Include(ent => ent.Certificates)
                        .Include(ent => ent.Contacts)
                            .ThenInclude(ent => ent.Contact)
                                .ThenInclude(ent => ent.Emails)
                        .Include(ent => ent.Contacts)
                            .ThenInclude(ent => ent.Contact)
                                .ThenInclude(ent => ent.Phones)
                        .Include(ent => ent.Contacts)
                            .ThenInclude(ent => ent.Contact)
                                .ThenInclude(ent => ent.Addresses)
                                    .ThenInclude(ent => ent.Address)
                        .Where(ent => filter.Invoke(ent));
            });
        }

        public async Task<DataSource<CustomerGridViewModel>> ReadAllAsyncDapper(DataSourceRequestCustomer request, int companyId)
        {
            // TODO: Refactor this
            var result = new DataSource<CustomerGridViewModel>
            {
                Payload = new List<CustomerGridViewModel>(),
                Count = 0
            };

            string selects = @"
                    [dbo].[Customers].[ID],
                    [dbo].[Customers].[CompanyId],
                    [dbo].[Customers].[Code],
                    [dbo].[Customers].[Name],
                    [dbo].[Customers].[StatusId],
                    [dbo].[Customers].[IsGenericAccount],
                    [dbo].[Customers].[Notes],
                    [dbo].[Customers].[MinimumProfitMargin],
                    [dbo].[Customers].[PONumberRequired],
                    [dbo].[Customers].[CreditLimit],
                    [dbo].[Customers].[CRHoldFlag],
                    [dbo].[Customers].[CreditTerms],
                    [dbo].[Customers].[ShowPricesOnShipper],
                    [dbo].[Customers].[InsuredUpTo],
                    [dbo].[Customers].[InsurerName],
                    [dbo].[Customers].[FinanceCharges],
                    [dbo].[Customers].[Guid],
                    [dbo].[Customers].[CreatedDate],
                    [dbo].[CustomerPhones].[Phone] as Phone,
                    [dbo].[CustomerPhones].[Ext] as Ext,
                    CONCAT([dbo].[Addresses].[AddressLine1]+' ', [dbo].[Addresses].AddressLine2 +' ', [dbo].[Addresses].City +' ', [dbo].[Addresses].[State]+' ', [dbo].[Addresses].[ZipCode]+' ' , [dbo].[Addresses].[CountryCode]+' ' ) as FullAddress,
                    (SELECT COUNT(*) FROM [dbo].[CustomerContacts] WHERE CustomerId = Customers.ID) AS ContactsTotal
            ";
            string filters = @"
                    WHERE CompanyId=@companyId AND
                            ISNULL(Name, '') + 
                            ISNULL(Code, '') + 
                            ISNULL(Phone, '') + 
                            ISNULL(Ext, '') +
                            ISNULL(FullAddress, '')
                                LIKE '%' + ISNULL(@filter, '') + '%'
            ";

            string joins = @"
                    LEFT OUTER JOIN [dbo].[CustomerPhones] ON [dbo].[Customers].ID=[dbo].[CustomerPhones].[CustomerId] AND 
                        ISNULL([dbo].[CustomerPhones].[Default], 0) = 1
                    LEFT OUTER JOIN [dbo].[CustomerAddresses] ON [dbo].[Customers].ID = [dbo].[CustomerAddresses].[CustomerId] AND 
                        ISNULL([dbo].[CustomerAddresses].[Default], 0) = 1
                    LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Addresses].ID = [dbo].[CustomerAddresses].[AddressId]
            ";
            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";
            string innerQuery = $@"
                SELECT  
                    {selects}
                FROM [dbo].[Customers] 
                {joins}
            ";
            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                {innerQuery}
                ) payload 
                {filters}
                ORDER BY {orders} CreatedDate DESC, ID
                OFFSET @pageSize * @pageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<CustomerGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<CustomerListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequestCustomer request, int companyId, int? id)
        {
            var result = new DataSource<CustomerListBoxViewModel>
            {
                Payload = new List<CustomerListBoxViewModel>(),
                Count = 0
            };

            string contactsQuery = "";

            if ((request.WithContacts ?? -1) != -1)
            {
                contactsQuery = $@"
                    AND (SELECT COUNT(*) FROM [dbo].[CustomerContacts] WHERE CustomerId = ID) {(request.WithContacts == 0 ? "= 0 " : "> 0")}
                ";
            }

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
                                [dbo].[Customers].ID as ID, 
                                [dbo].[Customers].[CompanyId] as CompanyId,
                                ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY [dbo].[Customers].[Name], [dbo].[Customers].ID ) as [Index]
                            FROM [dbo].[Customers]
                            ) payload
                        WHERE ID = @id {contactsQuery};
                        END

                        SELECT @total = COUNT(*) FROM [dbo].[Customers] WHERE [dbo].[Customers].[CompanyId]= @companyId;

                        --max(0, @total-@pageSize)
                        SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                        --safety check
                        SELECT @index = ISNULL(@index, 0);

                        --min(@index, @maxIndex)
                        SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                        SELECT 
                            [dbo].[Customers].[ID] as ID,
                            [dbo].[Customers].[Name] as [Name],
							CASE WHEN [dbo].[Customers].[Name]='' THEN cast([dbo].[Customers].[ID] as varchar) ELSE [dbo].[Customers].[Code] END as Code
                        FROM [dbo].[Customers]
                        WHERE [dbo].[Customers].[CompanyId]= @companyId AND
                            ISNULL([dbo].[Customers].[Name], '') 
                                LIKE '%' + ISNULL(@filter, '') + '%'
                            {contactsQuery}
                        ORDER BY Name, ID

                        OFFSET @index ROWS
                        FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@id", id);
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<CustomerListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
        
        public override async Task<Customer> SingleOrDefaultAsync(Func<Customer, bool> filter)
        {
            return await Entities
                .Include(ent => ent.Groups)
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        public async Task<Customer> GetCustomerByContractNumber(string customerCode)
        {
            var result = new Customer();
            var pars = new DynamicParameters();
            string whereQuery = "";

            whereQuery = " C.Code = @customerCode";
            pars.Add("@customerCode", customerCode);

            string query = $@"
                       SELECT
                             C.ID,
                             C.Code,
                             C.CompanyId,
                             C.StatusId
                      FROM Customers AS C
                      WHERE { whereQuery } ";

            var payload = await _baseDapperRepository.QueryAsync<Customer>(query, pars);
            result = payload.FirstOrDefault();
            return result;
        }
    }
}
