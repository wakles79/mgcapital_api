using Dapper;
using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class VendorContactsRepository : IVendorContactsRepository
    {
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorContactsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The implementation of <see cref="DbContext"/></param>
        public VendorContactsRepository(MGCapDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.Entities = (this.DbContext as MGCapDbContext).Set<VendorContact>();
        }

        /// <summary>
        ///     Gets dB representation of the object
        ///     of type TEntity.
        /// </summary>
        public DbSet<VendorContact> Entities { get; }

        /// <summary>
        /// Gets the Actual DBContext
        /// </summary>
        public DbContext DbContext { get; }

        /// <summary>
        ///     Gets an instance of the <see cref="IDbConnection"/> using the ConenctionString from the context.
        /// </summary>
        protected IDbConnection DbConnection => DbContext.Database.GetDbConnection();

        /// <summary>
        ///     Adds an object to the table
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public VendorContact Add(VendorContact obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("The given object must not be null");
            }

            this.Entities.Add(obj);
            return obj;
        }

        /// <summary>
        ///     Asynchronously adds an object to the table
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<VendorContact> AddAsync(VendorContact obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            await this.Entities.AddAsync(obj);
            return obj;
        }

        /// <summary>
        ///     Filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public IQueryable<VendorContact> ReadAll(Func<VendorContact, bool> filter)
        {
            return this.Entities.Where(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table <see cref="VendorContact"/> based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public async Task<IQueryable<VendorContact>> ReadAllAsync(Func<VendorContact, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Entities
                        .Include(ent => ent.Contact)
                        .Where(ent => filter.Invoke(ent));
            });
        }

        /// <summary>
        ///     Begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        public void Remove(Func<VendorContact, bool> filter)
        {
            var elementsToRemove = Entities.Where(ent => filter.Invoke(ent));
            Entities.RemoveRange(elementsToRemove);
        }

        /// <summary>
        ///     Asynchronously begins tracking the given param
        /// <remarks>
        ///     All the properties will be marked
        ///     as modified. To mark only some properties use the
        ///     <see cref="M:Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0)"/>
        /// </remarks>
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public async Task<VendorContact> UpdateAsync(VendorContact obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!this.Exists(obj.VendorId, obj.ContactId))
            {
                throw new ArgumentException("The given object does not exist in DB", nameof(obj));
            }

            await Task.Factory.StartNew(() =>
            {
                this.Entities.Update(obj);
            });
            return obj;
        }

        /// <summary>
        ///     Check if there is an element with the given <value>Id</value> in the Table
        /// </summary>
        /// <param name="contactId">The PK to be checked</param>
        /// <param name="vendorId">The PK to be checked</param>        /// 
        /// <returns><value>True</value> if the PK exists, false otherwise</returns>
        public bool Exists(int vendorId, int contactId)
        {
            return this.Entities.Any(ent => ent.ContactId.Equals(contactId) && ent.VendorId.Equals(vendorId));
        }


        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public int SaveChanges()
        {
            return this.DbContext.SaveChanges();
        }

        /// <summary>
        ///     Asynchronously saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public Task<int> SaveChangesAsync()
        {
            return this.DbContext.SaveChangesAsync();
        }
        /// <summary>
        ///     Asynchronously returns the only element of the table that satisfies the predicate
        ///     given in <paramref name="filter"/> or a default value if no element
        ///     exist.
        /// <exception cref="InvalidOperationException">
        ///     Throws an exception if more than one element satisfies
        ///     the condition
        /// </exception>
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the table</param>
        /// <returns>The element that satisfies the given predicate</returns>
        public async Task<VendorContact> SingleOrDefaultAsync(Func<VendorContact, bool> filter)
        {
            return await this.Entities
                .Include(ent => ent.Contact)
                    .ThenInclude(ent => ent.Emails)
                .Include(ent => ent.Contact)
                    .ThenInclude(ent => ent.Phones)
                .Include(ent => ent.Contact)
                    .ThenInclude(ent => ent.Addresses)
                        .ThenInclude(ent => ent.Address)
                .Include(ent => ent.Vendor)
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        public void Remove(VendorContact obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            this.Entities.Remove(obj);
        }


        public async Task<DataSource<ContactGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int vendorId)
        {
            // TODO: Refactor this
            var result = new DataSource<ContactGridViewModel>
            {
                Payload = new List<ContactGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "" : $"{request.SortField} {request.SortOrder.ToUpper()},";

            string query = $@"
                    -- payload query
                    SELECT *, [Count] = COUNT(*) OVER() FROM (
	                    SELECT 
	                    [dbo].[Contacts].[ID],
                        [dbo].[Contacts].[CompanyId],
                        [dbo].[Contacts].[DOB],
                        [dbo].[Contacts].[Gender],
                        [dbo].[Contacts].[Salutation],
                        [dbo].[Contacts].[Title],
                        [dbo].[Contacts].[Notes],
                        [dbo].[Contacts].[Guid],
                        [dbo].[ContactPhones].[Phone] as Phone,
                        [dbo].[ContactPhones].[Ext] as Ext,
                        [dbo].[ContactEmails].[Email] as Email,
	                    [dbo].[VendorContacts].[VendorId],
                        [dbo].[VendorContacts].[Type],
                        CONCAT([dbo].[Contacts].[FirstName]+' ',[dbo].[Contacts].[MiddleName]+' ',[dbo].[Contacts].[LastName]+' ') as FullName,
                        CONCAT([dbo].[Addresses].[AddressLine1]+' ', [dbo].[Addresses].AddressLine2 +' ', [dbo].[Addresses].City +' ', [dbo].[Addresses].[State]+' ', [dbo].[Addresses].[ZipCode]+' ' , [dbo].[Addresses].[CountryCode]+' ' ) as FullAddress
	                    FROM [dbo].[VendorContacts]
                        INNER JOIN  [dbo].[Contacts] ON  [dbo].[Contacts].[ID] = [dbo].[VendorContacts].[ContactId]
	                    LEFT OUTER JOIN [dbo].[ContactPhones] ON [dbo].[Contacts].ID=[dbo].[ContactPhones].[ContactId] AND 
                            ISNULL([dbo].[ContactPhones].[Default], 0) = 1
                        LEFT OUTER JOIN [dbo].[ContactEmails] ON [dbo].[Contacts].ID = [dbo].[ContactEmails].[ContactId] AND 
                            ISNULL([dbo].[ContactEmails].[Default], 0) = 1
                        LEFT OUTER JOIN [dbo].[ContactAddresses] ON [dbo].[Contacts].ID = [dbo].[ContactAddresses].[ContactId] AND 
                            ISNULL([dbo].[ContactAddresses].[Default], 0) = 1
                        LEFT OUTER JOIN [dbo].[Addresses] ON [dbo].[Addresses].ID = [dbo].[ContactAddresses].[AddressId]
					                    ) payload 
                        WHERE VendorId= @vendorId AND
                                ISNULL(FullName, '') + 
                                ISNULL(Phone, '') + 
                                ISNULL(Ext, '') +
                                ISNULL(Email, '') +
                                ISNULL(FullAddress, '')
                                    LIKE '%' + ISNULL(@filter, '') + '%'
                    ORDER BY ID
                    OFFSET @pageSize * @pageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";

            using (var conn = DbConnection)
            {
                var payload = await conn.QueryAsync<ContactGridViewModel>(query, new
                {
                    vendorId,
                    filter = request.Filter,
                    pageNumber = request.PageNumber,
                    pageSize = request.PageSize
                });
                result.Count = payload.FirstOrDefault()?.Count ?? 0;
                result.Payload = payload;
            }

            return result;

        }
        /// <summary>
        ///     Create and instance of the DBConnection and open it
        /// </summary>
        /// <returns>An open instance of a connection to the DB.</returns>
        protected IDbConnection OpenConnection()
        {
            var connection = DbConnection;
            connection.Open();
            return connection;
        }

        #region IDisposable Support
        /// <summary>
        ///     Release the allocated resources of the <see cref="DbContext"/>
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Release the allocated resources of the <see cref="DbContext"/>
        /// <remarks>
        ///     If the derived classes use objects that could
        ///     manage resources outside the context, override it
        ///     and dispose those objects
        /// </remarks>
        /// </summary>
        /// <param name="disposing">True for disposing the object; otherwise, false</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.DbContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }

        #endregion
    }
}
