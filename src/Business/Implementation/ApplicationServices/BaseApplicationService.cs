// -----------------------------------------------------------------------
// <copyright file="BaseApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    /// <summary>
    ///     Contains the common functionalities of the Business layer
    ///     Through these object would be possible to communicate with the
    ///     DataAcces layer and apply businesses related operations
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for manipulate</typeparam>
    /// <typeparam name="TKey">The type of the key for that entity</typeparam>
    public abstract class BaseApplicationService<TEntity, TKey> : IBaseApplicationService<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Gets or Sets the Repository for handling the entity object
        /// </summary>
        public IBaseRepository<TEntity, TKey> Repository { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplicationService{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IBaseRepository{TEntity,TKey}"/> for accessing to the
        /// functionalities of the DataAceess layer.</param>
        protected BaseApplicationService(IBaseRepository<TEntity, TKey> repository)
        {
            this.Repository = repository;
        }
        /// <summary>
        ///     Adds an object to the table
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public TEntity Add(TEntity obj)
        {
            return this.Repository.Add(obj);
        }

        /// <summary>
        ///     Asynchronously adds an object to the table
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public async Task<TEntity> AddAsync(TEntity obj)
        {
            return await this.Repository.AddAsync(obj);
        }

        /// <summary>
        ///     Add the list of elements given in <paramref name="objs"/> to the Table
        /// <remarks>
        ///     Use this method instead of <see cref="IBaseApplicationService{TEntity,TKey}.Add"/> when possible
        /// </remarks>
        /// </summary>
        /// <param name="objs">The objects to be added</param>
        /// <returns>The given <paramref name="objs"/> after being inserted</returns>
        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> objs)
        {
            return this.Repository.AddRange(objs);
        }

        /// <summary>
        ///     Asynchronously adds the list of elements given in <paramref name="objs"/> to the Table
        /// <remarks>
        ///     Use this method instead of <see cref="IBaseApplicationService{TEntity,TKey}.Add"/> when possible
        /// </remarks>
        /// </summary>
        /// <param name="objs">The objects to be added</param>
        /// <returns>The given <paramref name="objs"/> after being inserted</returns>
        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> objs)
        {
            return await this.Repository.AddRangeAsync(objs);
        }

        /// <summary>
        ///     Check if the given <paramref name="obj"/> exists in the table
        /// </summary>
        /// <param name="obj">The element to locate in the Table</param>
        /// <returns><value>True</value> if the given object exists, false otherwise</returns>
        public bool Exists(TEntity obj)
        {
            return this.Repository.Exists(obj);
        }

        /// <summary>
        ///     Check if there is an element with the given <value>Id</value> in the Table
        /// </summary>
        /// <param name="id">The PK to be checked</param>
        /// <returns><value>True</value> if the PK exists, false otherwise</returns>
        public bool Exists(TKey id)
        {
            return this.Repository.Exists(id);
        }

        /// <summary>
        ///     Checks if there is at least one element that satisfies the condition
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the Table</param>
        /// <returns><value>True</value> if any element satisfies the condition; otherwise, false</returns>
        public bool Exists(Func<TEntity, bool> filter)
        {
            return this.Repository.Exists(filter);
        }

        /// <summary>
        ///     Asynchronously check if the given <paramref name="obj"/> exists in the table
        /// </summary>
        /// <param name="obj">The element to locate in the Table</param>
        /// <returns><value>True</value> if the given object exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(TEntity obj)
        {
            return await this.Repository.ExistsAsync(obj);
        }

        /// <summary>
        ///     Asynchronously check if there is an element with the given <value>Id</value> in the Table
        /// </summary>
        /// <param name="id">The PK to be checked</param>
        /// <returns><value>True</value> if the PK exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(TKey id)
        {
            return await this.Repository.ExistsAsync(id);
        }

        /// <summary>
        ///     Asynchronously checks if there is at least one element that satisfies the condition
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the Table</param>
        /// <returns><value>True</value> if any element satisfies the condition; otherwise, false</returns>
        public async Task<bool> ExistsAsync(Func<TEntity, bool> filter)
        {
            return await this.Repository.ExistsAsync(filter);
        }

        /// <summary>
        ///     Filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public IQueryable<TEntity> ReadAll(Func<TEntity, bool> filter)
        {
            return this.Repository.ReadAll(filter);
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public async Task<IQueryable<TEntity>> ReadAllAsync(Func<TEntity, bool> filter)
        {
            return await this.Repository.ReadAllAsync(filter);
        }
        /// <summary>
        ///     Begins tracking entity with the given
        ///     <value>Id</value> in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="id">The <value>Id</value> of the Entity to remove</param>
        public void Remove(TKey id)
        {
            this.Repository.Remove(id);
        }

        /// <summary>
        ///     Begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        public void Remove(Func<TEntity, bool> filter)
        {
            this.Repository.Remove(filter);
        }

        /// <summary>
        ///     Begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        public void Remove(TEntity obj)
        {
            this.Repository.Remove(obj);
        }

        /// <summary>
        ///     Asynchronously begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAsync(Func<TEntity, bool> filter)
        {
            await this.Repository.RemoveAsync(filter);
        }

        /// <summary>
        ///     Asynchronously begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAsync(TEntity obj)
        {
            await this.Repository.RemoveAsync(obj);
        }

        /// <summary>
        ///     Asynchronously begins tracking entity with the given
        ///     <value>Id</value> in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="id">The <value>Id</value> of the Entity to remove</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAsync(TKey id)
        {
            await this.Repository.RemoveAsync(id);
        }

        /// <summary>
        ///     Begins tracking the given Entities
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="objs">The objects to be marked</param>
        public void RemoveRange(IEnumerable<TEntity> objs)
        {
            this.Repository.RemoveRange(objs);
        }

        /// <summary>
        ///     Asynchronously begins tracking the given Entities
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseApplicationService{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="objs">The objects to be marked</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveRangeAsync(IEnumerable<TEntity> objs)
        {
            await this.Repository.RemoveRangeAsync(objs);
        }

        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public int SaveChanges()
        {
            return this.Repository.SaveChanges();
        }

        /// <summary>
        ///     Asynchronously saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await this.Repository.SaveChangesAsync();
            }
            catch (DbUpdateException dbuEx)
            {
                throw dbuEx;
            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Returns the only element of the table that satisfies the predicate
        ///     given in <paramref name="filter"/> or a default value if no element
        ///     exist.
        /// <exception>
        ///     Throws an exception if more than one element satisfies
        ///     the condition
        /// </exception>
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the table</param>
        /// <returns>The element that satisfies the given predicate</returns>
        public TEntity SingleOrDefault(Func<TEntity, bool> filter)
        {
            return this.Repository.SingleOrDefault(filter);
        }

        /// <summary>
        ///     Returns the only element of the table with the given <value>Id</value>
        /// </summary>
        /// <param name="id">The Id of the desired element</param>
        /// <returns>The element with the given Id</returns>
        public TEntity SingleOrDefault(TKey id)
        {
            return this.Repository.SingleOrDefault(id);
        }

        /// <summary>
        ///     Asynchronously returns the only element of the table that satisfies the predicate
        ///     given in <paramref name="filter"/> or a default value if no element
        ///     exist.
        /// <exception>
        ///     Throws an exception if more than one element satisfies
        ///     the condition
        /// </exception>
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the table</param>
        /// <returns>The element that satisfies the given predicate</returns>
        public async Task<TEntity> SingleOrDefaultAsync(Func<TEntity, bool> filter)
        {
            return await this.Repository.SingleOrDefaultAsync(filter);
        }

        /// <summary>
        ///     Asynchronously returns the only element of the table with the given <value>Id</value>
        /// </summary>
        /// <param name="id">The Id of the desired element</param>
        /// <returns>The element with the given Id</returns>
        public async Task<TEntity> SingleOrDefaultAsync(TKey id)
        {
            return await this.Repository.SingleOrDefaultAsync(id);
        }

        /// <summary>
        ///     Begins tracking the given param
        /// <remarks>
        ///     All the properties will be marked
        ///     as modified. To mark only some properties use the
        ///     <see cref="M:Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0)"/>
        /// </remarks>
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public TEntity Update(TEntity obj)
        {
            return this.Repository.Update(obj);
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
        public async Task<TEntity> UpdateAsync(TEntity obj)
        {
            return await this.Repository.UpdateAsync(obj);
        }

        /// <summary>
        ///     Begins tracking objects given in <paramref name="obj"/>
        /// <remarks>
        ///     All the properties will be marked
        ///     as modified. To mark only some properties use the
        ///     <see cref="M:Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0)"/>
        /// </remarks>
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> obj)
        {
            return this.Repository.UpdateRange(obj);
        }

        /// <summary>
        ///     Asynchronously begins tracking objects given in <paramref name="obj"/>
        /// <remarks>
        ///     All the properties will be marked
        ///     as modified. To mark only some properties use the
        ///     <see cref="M:Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0)"/>
        /// </remarks>
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> obj)
        {
            return await this.Repository.UpdateRangeAsync(obj);
        }

        #region IDisposable Support

        /// <summary>
        ///     Release the allocated resources
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Release the allocated resources
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
                    this.Repository.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                Repository = null;
                this.disposedValue = true;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="BaseApplicationService{TEntity, TKey}"/> class.
        /// </summary>
        ~BaseApplicationService()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }
        #endregion
    }
}
