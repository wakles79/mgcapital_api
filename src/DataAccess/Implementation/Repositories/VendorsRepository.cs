// -----------------------------------------------------------------------
// <copyright file="VendorsRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Vendor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="Vendor"/>
    /// </summary>
    public class VendorsRepository : BaseRepository<Vendor, int>, IVendorsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VendorsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public VendorsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table <see cref="Vendor"/> based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public override async Task<IQueryable<Vendor>> ReadAllAsync(Func<Vendor, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Entities
                        .Include(ent => ent.Addresses)
                            .ThenInclude(ent => ent.Address)
                        .Include(ent => ent.Emails)
                        .Include(ent => ent.Phones)
                        .Include(ent => ent.Contacts)
                        .Where(ent => filter.Invoke(ent));
            });
        }

        public async Task<DataSource<VendorGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int companyId)
        {
            // TODO: Refactor this
            var result = new DataSource<VendorGridViewModel>
            {
                Payload = new List<VendorGridViewModel>(),
                Count = 0
            };

            string selects = @"
                    [dbo].[Vendors].[ID],
                    [dbo].[Vendors].[CompanyId],
                    [dbo].[Vendors].[Code],
                    [dbo].[Vendors].[Name],
                    [dbo].[Vendors].[VendorTypeId],
                    [dbo].[Vendors].[IsPerson],
                    [dbo].[Vendors].[SSN],
                    [dbo].[Vendors].[FEIN],
                    [dbo].[Vendors].[Is1099],
                    [dbo].[Vendors].[IsSensitiveAccount],
                    [dbo].[Vendors].[TermsDaysOrProx],
                    [dbo].[Vendors].[TermsDiscPercent],
                    [dbo].[Vendors].[TermsNet],
                    [dbo].[Vendors].[AccountNumber],
                    [dbo].[Vendors].[DefaultGLAccountNumber1],
                    [dbo].[Vendors].[DefaultGLAccountNumber2],
                    [dbo].[Vendors].[Guid],
                    [dbo].[VendorPhones].[Phone] as Phone,
                    [dbo].[VendorPhones].[Ext] as Ext,
                    CONCAT([dbo].[Addresses].[AddressLine1]+' ', [dbo].[Addresses].AddressLine2 +' ', [dbo].[Addresses].City +' ', [dbo].[Addresses].[State]+' ', [dbo].[Addresses].[ZipCode]+' ' , [dbo].[Addresses].[CountryCode]+' ' ) as FullAddress
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
                    LEFT OUTER JOIN [dbo].[VendorPhones] ON [dbo].[Vendors].ID=[dbo].[VendorPhones].[VendorId] AND 
                        ISNULL([dbo].[VendorPhones].[Default], 0) = 1
                    LEFT OUTER JOIN [dbo].[VendorAddresses] ON [dbo].[Vendors].ID = [dbo].[VendorAddresses].[VendorId] AND 
                        ISNULL([dbo].[VendorAddresses].[Default], 0) = 1
                    LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Addresses].ID = [dbo].[VendorAddresses].[AddressId]
            ";
            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";
            string innerQuery = $@"
                SELECT  
                    {selects}
                FROM [dbo].[Vendors] 
                {joins}
            ";
            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                {innerQuery}
                ) payload 
                {filters}
                ORDER BY {orders} ID
                OFFSET @pageSize * @pageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";

            var pars = new DynamicParameters();
            pars.Add("@companyId", companyId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<VendorGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public override async Task<Vendor> SingleOrDefaultAsync(Func<Vendor, bool> filter)
        {
            return await Entities
                .Include(ent => ent.Groups)
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        public async Task<DataSource<ListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int companyId, int? id = null)
        {
            // TODO: Refactor this
            var result = new DataSource<ListBoxViewModel>
            {
                Payload = new List<ListBoxViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

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
                                [dbo].[Vendors].ID as ID, 
                                [dbo].[Vendors].[CompanyId] as CompanyId,
                                ROW_NUMBER() OVER (PARTITION BY CompanyId Order BY [dbo].[Vendors].[Name], [dbo].[Vendors].ID ) as [Index]
                            FROM [dbo].[Vendors]
                            ) payload
                        WHERE ID = @id;
                        END

                        SELECT @total = COUNT(*) FROM [dbo].[Vendors] WHERE [dbo].[Vendors].[CompanyId]= @companyId;

                        --max(0, @total-@pageSize)
                        SELECT @maxIndex = IIF(@pageSize > @total, 0, @total - @pageSize);

                        --safety check
                        SELECT @index = ISNULL(@index, 0);

                        --min(@index, @maxIndex)
                        SELECT @index = IIF(@index > @maxIndex, @maxIndex, @index)

                        SELECT 
                            [dbo].[Vendors].[ID] as ID,
                            [dbo].[Vendors].[Name] as [Name]
                        FROM [dbo].[Vendors]
                        WHERE [dbo].[Vendors].[CompanyId]= @companyId AND
                            ISNULL([dbo].[Vendors].[Name], '') 
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

            var payload = await _baseDapperRepository.QueryAsync<ListBoxViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;
            
            return result;
        }

    }
}
