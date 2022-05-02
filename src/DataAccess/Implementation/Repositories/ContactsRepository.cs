// -----------------------------------------------------------------------
// <copyright file="ContactRepository.cs" company="Axzes">
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
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Contact"/>
    /// </summary>
    public class ContactsRepository : BaseRepository<Contact, int>, IContactsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public ContactsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="Contact"/> represents
        ///     applying a filter
        /// </summary>
        /// <returns>A list with all the Contacts with the given filter</returns>
        public new async Task<Contact> SingleOrDefaultAsync(Func<Contact, bool> filter)
        {
            return await Entities
                .Include(ent => ent.Emails)
                .Include(ent => ent.Phones)
                .Include(ent => ent.Addresses)
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table <see cref="Contact"/> based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public override async Task<IQueryable<Contact>> ReadAllAsync(Func<Contact, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Entities
                        .Include(ent => ent.Addresses)
                            .ThenInclude(ent => ent.Address)
                        .Include(ent => ent.Emails)
                        .Include(ent => ent.Phones)
                        .Where(ent => filter.Invoke(ent));
            });
        }

        public async Task<DataSource<ContactLogsViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId)
        {
            // TODO: Refactor this
            var result = new DataSource<ContactLogsViewModel>
            {
                Payload = new List<ContactLogsViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
	                    C.[ID],
	                    C.[CompanyId],
                        C.[DOB],
                        C.[Gender],
                        C.[Salutation],
                        C.[Title],
                        C.[Notes],
                        C.[Guid],
                        C.[CreatedDate],
                        CP.[Phone] as Phone,
                        CP.[Ext] as Ext,
                        CE.[Email] as Email,
                        CONCAT(C.[FirstName], ' ', C.[MiddleName], ' ', C.[LastName]) as FullName,
                        CONCAT(A.[AddressLine1], ' ', A.AddressLine2, ' ', A.City, ' ', A.[State], ' ', A.[ZipCode], ' ' , A.[CountryCode]) as FullAddress
                        ,ML.LastLogin as ManagerApp
                        ,WP.LastLogin as WebApp
                        ,CL.LastLogin as ClientApp
                        ,(CASE WHEN CustomerContacts.ContactId IS NULL THEN 0 ELSE 1 END) AS IsCustomer
                        ,(CASE WHEN EMP.Email IS NULL THEN 0 ELSE 1 END) AS IsEmployee
                        FROM [dbo].[Contacts] AS C
	                    LEFT OUTER JOIN [dbo].[ContactPhones] AS CP ON C.ID=CP.[ContactId] AND 
                            ISNULL(CP.[Default], 0) = 1
                        LEFT OUTER JOIN [dbo].[ContactEmails] AS CE ON C.ID = CE.[ContactId] AND 
                            ISNULL(CE.[Default], 0) = 1
                        LEFT OUTER JOIN [dbo].[ContactAddresses] AS CA ON C.ID = CA.[ContactId] AND 
                            ISNULL(CA.[Default], 0) = 1
                        LEFT OUTER JOIN [dbo].[Addresses] AS A ON A.ID = CA.[AddressId]

                        LEFT OUTER JOIN (SELECT
                            ProviderDisplayName as Email
                            ,LoginProvider
                            , max(CONVERT(DATETIME, AspNetUserLogins.[ProviderKey])) as LastLogin
                            from AspNetUserLogins
                            WHERE LoginProvider = 'MG Manager'
                            group by AspNetUserLogins.ProviderDisplayName,AspNetUserLogins.[LoginProvider]
                            ) as ML on (ML.Email = CE.Email)

                        LEFT OUTER JOIN (SELECT
                            ProviderDisplayName as Email
                            ,LoginProvider
                            , max(CONVERT(DATETIME, AspNetUserLogins.[ProviderKey])) as LastLogin
                            from AspNetUserLogins
                            WHERE LoginProvider = 'Web Portal'
                            group by AspNetUserLogins.ProviderDisplayName,AspNetUserLogins.[LoginProvider]
                            ) as WP on (WP.Email = CE.Email)

                        LEFT OUTER JOIN (SELECT
                            ProviderDisplayName as Email
                            ,LoginProvider
                            , max(CONVERT(DATETIME, AspNetUserLogins.[ProviderKey])) as LastLogin
                            from AspNetUserLogins
                            WHERE LoginProvider = 'MG Client'
                            group by AspNetUserLogins.ProviderDisplayName,AspNetUserLogins.[LoginProvider]
                            ) as CL on (CL.Email = CE.Email)
                    
                        Left OUTER JOIN  CustomerContacts ON (CustomerContacts.ContactId = C.ID )

                        LEFT OUTER JOIN (select DISTINCT Email FROM Employees where CompanyId = @companyId) as EMP ON (EMP.Email = CE.Email)                        

                        WHERE C.CompanyId = @companyId AND
                                CONCAT(C.FirstName, C.MiddleName, C.LastName, A.AddressLine1, A.AddressLine2, A.City, A.State, A.ZipCode, CP.Phone, CP.Ext, CE.Email)
                                    LIKE '%' + ISNULL(@filter, '') + '%'

					                    ) payload 

                    ORDER BY {orders} CreatedDate DESC, ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<ContactLogsViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<ContactListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id)
        {
            var result = new DataSource<ContactListBoxViewModel>
            {
                Payload = new List<ContactListBoxViewModel>(),
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
                                [dbo].[Contacts].ID as ID, 
                                [dbo].[Contacts].[CompanyId] as CompanyId,
                                ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY ISNULL(CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[LastName]), ''), [dbo].[Contacts].ID ) as [Index]
                            FROM [dbo].[Contacts]
                            ) payload
                        WHERE ID = @id;
                        END

                        SELECT @total = COUNT(*) FROM [dbo].[Contacts] WHERE [dbo].[Contacts].[CompanyId]= @companyId;

                        --max(0, @total-@pageSize)
                        SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                        --safety check
                        SELECT @index = ISNULL(@index, 0);

                        --min(@index, @maxIndex)
                        SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                        SELECT * FROM (SELECT 
                            C.[ID] as ID,
                            C.CompanyId,
                            CONCAT(C.[FirstName], ' ', C.[LastName]) as [Name],
                            -- First Matching Phone
                            ISNULL((SELECT TOP 1 CP.Phone FROM [dbo].[ContactPhones] AS CP WHERE CP.ContactId = C.ID), '') AS Phone,
                            -- First Matching Email
                            ISNULL((SELECT TOP 1 CE.Email FROM [dbo].[ContactEmails] AS CE WHERE CE.ContactId = C.ID), '') AS Email,
                            -- First Matching Address
                            ISNULL((SELECT TOP 1 CONCAT(A.AddressLine1, ' ', A.City, ' ', A.State, ' ', A.ZipCode) FROM [dbo].[ContactAddresses] AS CA INNER JOIN [dbo].[Addresses] AS A ON A.ID = CA.AddressId WHERE CA.ContactId = C.ID), '') AS FullAddress
                        FROM [dbo].[Contacts] AS C) payload 

                        WHERE CompanyId= @companyId AND
                            ISNULL(CONCAT(Name, Phone, Email, FullAddress), '') 
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

            var payload = await _baseDapperRepository.QueryAsync<ContactListBoxViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
