using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class CustomerCustomerGroupsRepository : ICustomerCustomerGroupsRepository
    {
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerCustomerGroupsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The implementation of <see cref="DbContext"/></param>
        public CustomerCustomerGroupsRepository(MGCapDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.Entities = (this.DbContext as MGCapDbContext).Set<CustomerCustomerGroup>();
        }

        /// <summary>
        ///     Gets dB representation of the object
        ///     of type TEntity.
        /// </summary>
        public DbSet<CustomerCustomerGroup> Entities { get; }

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
        public CustomerCustomerGroup Add(CustomerCustomerGroup obj)
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
        public async Task<CustomerCustomerGroup> AddAsync(CustomerCustomerGroup obj)
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
        public IQueryable<CustomerCustomerGroup> ReadAll(Func<CustomerCustomerGroup, bool> filter)
        {
            return this.Entities.Where(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table <see cref="CustomerCustomerGroup"/> based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public async Task<IQueryable<CustomerCustomerGroup>> ReadAllAsync(Func<CustomerCustomerGroup, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return Entities
                        .Include(ent => ent.CustomerGroup)
                        .Where(ent => filter.Invoke(ent));
            });
        }

        /// <summary>
        ///     Begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        public void Remove(Func<CustomerCustomerGroup, bool> filter)
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
        public async Task<CustomerCustomerGroup> UpdateAsync(CustomerCustomerGroup obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!this.Exists(obj.CustomerId, obj.CustomerGroupId))
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
        /// <param name="customerId">The PK to be checked</param>
        /// <param name="customerGroupId">The PK to be checked</param>        /// 
        /// <returns><value>True</value> if the PK exists, false otherwise</returns>
        public bool Exists(int customerId, int customerGroupId)
        {
            return this.Entities.Any(ent => ent.CustomerId.Equals(customerId) && ent.CustomerGroupId.Equals(customerGroupId));
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
        public async Task<CustomerCustomerGroup> SingleOrDefaultAsync(Func<CustomerCustomerGroup, bool> filter)
        {
            return await this.Entities
                .Include(ent => ent.CustomerGroup)
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        public void Remove(CustomerCustomerGroup obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            this.Entities.Remove(obj);
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
