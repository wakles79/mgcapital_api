// -----------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Entities;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Set of newly added <see cref="IDocumentEntity{T}"/> entities to be
        /// set their Number property after disposing <see cref="DbContext"/>
        /// </summary>
        private HashSet<IEntity> _addedDocuments = new HashSet<IEntity>();
        
        
        /// <summary>
        /// Set of modified <see cref="IDocumentEntity{T}"/> entities to
        /// avoid updating their Number property
        /// </summary>
        private HashSet<IEntity> _updatedDocuments = new HashSet<IEntity>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="dbContext">The implementation of <see cref="DbContext"/></param>
        /// <param name="mgcapDbOptions">The implementation of <see cref="MGCapDbOptions"/></param>
        protected BaseRepository(DbContext dbContext)
        {
            this.DbContext = dbContext;
            if (this.DbContext is MGCapDbContext)
            {
                this.Entities = (this.DbContext as MGCapDbContext).Set<TEntity>();
            }
        }

        /// <summary>
        ///     Gets dB representation of the object
        ///     of type TEntity.
        /// </summary>
        public DbSet<TEntity> Entities { get; }


        /// <summary>
        /// Gets the Actual DBContext
        /// </summary>
        public DbContext DbContext { get; }

        /// <summary>
        ///     Gets an instance of the <see cref="IDbConnection"/> using the ConenctionString from the context.
        /// </summary>
        //protected IDbConnection DbConnection => DbContext.Database.GetDbConnection();

        /// <summary>
        ///     Adds an object to the table
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        public virtual TEntity Add(TEntity obj)
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
        public virtual async Task<TEntity> AddAsync(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            await this.Entities.AddAsync(obj);
            return obj;
        }

        /// <summary>
        ///     Add the list of elements given in <paramref name="objs"/> to the Table
        /// <remarks>
        ///     Use this method instead of <see cref="IBaseRepository{TEntity,TKey}.Add"/> when possible
        /// </remarks>
        /// </summary>
        /// <param name="objs">The objects to be added</param>
        /// <returns>The given <paramref name="objs"/> after being inserted</returns>
        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> objs)
        {
            if (objs == null)
            {
                throw new ArgumentNullException(nameof(objs), "The given object must not be null");
            }

            if (!objs.Any())
            {
                throw new ArgumentException(nameof(objs), "The given param must contains at least on element");
            }

            this.Entities.AddRange(objs);
            return objs;
        }

        /// <summary>
        ///     Asynchronously adds the list of elements given in <paramref name="objs"/> to the Table
        /// <remarks>
        ///     Use this method instead of <see cref="IBaseRepository{TEntity,TKey}.Add"/> when possible
        /// </remarks>
        /// </summary>
        /// <param name="objs">The objects to be added</param>
        /// <returns>The given <paramref name="objs"/> after being inserted</returns>
        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> objs)
        {
            if (objs == null)
            {
                throw new ArgumentNullException(nameof(objs), "The given object must not be null");
            }

            if (!objs.Any())
            {
                throw new ArgumentException("The given param must contains at least on element", nameof(objs));
            }

            await this.Entities.AddRangeAsync(objs);
            return objs;
        }

        /// <summary>
        ///     Check if the given <paramref name="obj"/> exists in the table
        /// </summary>
        /// <param name="obj">The element to locate in the Table</param>
        /// <returns><value>True</value> if the given object exists, false otherwise</returns>
        public virtual bool Exists(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            return this.Entities.Contains(obj);
        }

        /// <summary>
        ///     Check if there is an element with the given <value>Id</value> in the Table
        /// </summary>
        /// <param name="id">The PK to be checked</param>
        /// <returns><value>True</value> if the PK exists, false otherwise</returns>
        public virtual bool Exists(TKey id)
        {
            return this.Entities.Any(ent => ent.ID.Equals(id));
        }

        /// <summary>
        ///     Checks if there is at least one element that satisfies the condition
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the Table</param>
        /// <returns><value>True</value> if any element satisfies the condition; otherwise, false</returns>
        public virtual bool Exists(Func<TEntity, bool> filter)
        {
            return this.Entities.Any(filter);
        }

        /// <summary>
        ///     Asynchronously check if the given <paramref name="obj"/> exists in the table
        /// </summary>
        /// <param name="obj">The element to locate in the Table</param>
        /// <returns><value>True</value> if the given object exists, false otherwise</returns>
        public virtual async Task<bool> ExistsAsync(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            return await this.Entities.ContainsAsync(obj);
        }

        /// <summary>
        ///     Asynchronously check if there is an element with the given <value>Id</value> in the Table
        /// </summary>
        /// <param name="id">The PK to be checked</param>
        /// <returns><value>True</value> if the PK exists, false otherwise</returns>
        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            return await this.Entities.AnyAsync(ent => ent.ID.Equals(id));
        }

        /// <summary>
        ///     Asynchronously checks if there is at least one element that satisfies the condition
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the Table</param>
        /// <returns><value>True</value> if any element satisfies the condition; otherwise, false</returns>
        public virtual async Task<bool> ExistsAsync(Func<TEntity, bool> filter)
        {
            return await this.Entities.AnyAsync(ent => filter.Invoke(ent), cancellationToken: default(CancellationToken));
        }

        /// <summary>
        ///     Filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public virtual IQueryable<TEntity> ReadAll(Func<TEntity, bool> filter)
        {
            return this.Entities.Where(ent => filter.Invoke(ent));
        }


        /// <summary>
        ///     Asynchronously filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public virtual async Task<IQueryable<TEntity>> ReadAllAsync(Func<TEntity, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return this.Entities.Where(ent => filter.Invoke(ent));
            });
        }

        /// <summary>
        ///     Begins tracking entity with the given
        ///     <value>Id</value> in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="id">The <value>Id</value> of the Entity to remove</param>
        public virtual void Remove(TKey id)
        {
            var objToDelete = this.Entities.FirstOrDefault(ent => ent.ID.Equals(id));

            if (objToDelete == null)
            {
                throw new ArgumentException("Does not exist an element with the given key", nameof(id));
            }

            this.Entities.Remove(objToDelete);
        }

        /// <summary>
        ///     Begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        public virtual void Remove(Func<TEntity, bool> filter)
        {
            var elementsToRemove = Entities.Where(ent => filter.Invoke(ent));
            Entities.RemoveRange(elementsToRemove);
        }

        /// <summary>
        ///     Begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        public virtual void Remove(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            this.Entities.Remove(obj);
        }

        /// <summary>
        ///     Asynchronously begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(Func<TEntity, bool> filter)
        {
            await Task.Factory.
               StartNew(() =>
               {
                   return Entities.Where(ent => filter.Invoke(ent));
               }).
               ContinueWith(antecedent =>
               {
                   var elementsToRemove = antecedent.Result;
                   Entities.RemoveRange(elementsToRemove);
               });
        }

        /// <summary>
        ///     Asynchronously begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            await Task.Factory.StartNew(() =>
            {
                Entities.Remove(obj);
            });
        }

        /// <summary>
        ///     Asynchronously begins tracking entity with the given
        ///     <value>Id</value> in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="id">The <value>Id</value> of the Entity to remove</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(TKey id)
        {
            var objToDelete = await this.Entities.FirstOrDefaultAsync(ent => ent.ID.Equals(id));

            if (objToDelete == null)
            {
                throw new ArgumentException("Does not exist an element with the given key", nameof(id));
            }

            this.Entities.Remove(objToDelete);
        }

        /// <summary>
        ///     Begins tracking the given Entities
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="objs">The objects to be marked</param>
        public virtual void RemoveRange(IEnumerable<TEntity> objs)
        {
            if (objs == null)
            {
                throw new ArgumentNullException(nameof(objs), "The given object must not be null");
            }

            if (!objs.Any())
            {
                return;
            }

            this.Entities.RemoveRange(objs);
        }

        /// <summary>
        ///     Asynchronously begins tracking the given Entities
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IBaseRepository{TEntity,TKey}.Update"/> is called
        /// </summary>
        /// <param name="objs">The objects to be marked</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> objs)
        {
            if (objs == null)
            {
                throw new ArgumentNullException(nameof(objs), "The given object must not be null");
            }

            if (!objs.Any())
            {
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                Entities.RemoveRange(objs);
            });
        }

        private void UpdateSoftDeletableEntities()
        {
            var deletedEntries = this.DbContext
                                    .ChangeTracker
                                    .Entries()
                                    .Where(x => x.Entity is ISoftDeletable &&
                                                 x.State == EntityState.Deleted);
            foreach (var entry in deletedEntries)
            {
                if (entry?.Entity is ISoftDeletable entity)
                {
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                }
            }
        }

        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public int SaveChanges()
        {
            try
            {
                this.UpdateSoftDeletableEntities();
                this.SkipNumbersUpdate();
                this.UpdateDocumentEntities();
                var result = this.DbContext.SaveChanges();
                this.AssignNumbers();
                return result;
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

        private void SkipNumbersUpdate()
        {
            try
            {
                var entries = this.DbContext
                    .ChangeTracker
                    .Entries()
                    .Where(ent => ent.State == EntityState.Modified)
                    ?.ToList();

                foreach (var entry in entries)
                {
                    switch (entry.Entity)
                    {
                        case null:
                            continue;
                        case IDocumentEntity<int> intEntity:
                            this.DbContext.Entry(intEntity).Property(e => e.Number).IsModified = false;
                            break;
                        case IDocumentEntity<long> longEntity:
                            this.DbContext.Entry(longEntity).Property(e => e.Number).IsModified = false;
                            break;
                        case IDocumentEntity<string> strEntity:
                            this.DbContext.Entry(strEntity).Property(e => e.Number).IsModified = false;
                            break;
                    }  
                }
            }
            catch
            {
                // ignored
            } 
        }

        private void UpdateDocumentEntities()
        {
            try
            {
                var entries = this.DbContext
                    .ChangeTracker
                    .Entries()
                    .Where(ent => ent.State == EntityState.Added)
                    ?.ToList();

                foreach (var entry in entries)
                {
                    this.TrackEntityDocument(entry);
                }
            }
            catch
            {
                // ignored
            }
        }
        
        private void TrackEntityDocument(EntityEntry entry)
        {
            switch (entry.Entity)
            {
                case null:
                    return;
                case IDocumentEntity<int> intEntity:
                    this.TrackDocument(entry.State, intEntity);
                    break;
                case IDocumentEntity<long> longEntity:
                    this.TrackDocument(entry.State, longEntity);
                    break;
                case IDocumentEntity<string> strEntity:
                    this.TrackDocument(entry.State, strEntity);
                    break;
            } 
        }

        private void TrackDocument<T>(EntityState state, IDocumentEntity<T> entity)
        {
            switch (state)
            {
                case EntityState.Added:
                    this._addedDocuments.Add(entity as IEntity);
                    break;
                case EntityState.Modified:
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            } 
        }

        /// <summary>
        /// Updates POCO references with their actual Number from DB
        /// </summary>
        private void AssignNumbers()
        {
            try
            {
                var createdEntries = this._addedDocuments.ToArray();

                foreach (var entity in createdEntries)
                {
                    switch (entity)
                    {
                        case null:
                            continue;
                        case IDocumentEntity<int> intEntity:
                            switch (intEntity)
                            {
                                case IEntity<int> strNumber:
                                    intEntity.Number = this.ReadNumber<int,int>(intEntity as IEntity<int>);
                                    break;
                                case IEntity<long> strNumber:
                                    intEntity.Number = this.ReadNumber<long,int>(intEntity as IEntity<long>);
                                    break;
                                case IEntity<string> strNumber:
                                    intEntity.Number = this.ReadNumber<string,int>(intEntity as IEntity<string>);
                                    break;
                            }
                            this._addedDocuments.Remove(entity);
                            break;
                        case IDocumentEntity<long> longEntity:
                            switch (longEntity)
                            {
                                case IEntity<int> strNumber:
                                    longEntity.Number = this.ReadNumber<int, long>(longEntity as IEntity<int>);
                                    break;
                                case IEntity<long> strNumber:
                                    longEntity.Number = this.ReadNumber<long, long>(longEntity as IEntity<long>);
                                    break;
                                case IEntity<string> strNumber:
                                    longEntity.Number = this.ReadNumber<string, long>(longEntity as IEntity<string>);
                                    break;
                            }
                            this._addedDocuments.Remove(entity);
                            break;
                        case IDocumentEntity<string> strEntity:
                            switch (strEntity)
                            {
                                case IEntity<int> strNumber:
                                    strEntity.Number = this.ReadNumber<int, string>(strEntity as IEntity<int>);
                                    break;
                                case IEntity<long> strNumber:
                                    strEntity.Number = this.ReadNumber<long, string>(strEntity as IEntity<long>);
                                    break;
                                case IEntity<string> strNumber:
                                    strEntity.Number = this.ReadNumber<string, string>(strEntity as IEntity<string>);
                                    break;
                            }
                            this._addedDocuments.Remove(entity);
                            break;
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///  Reads the recently added entity's Number using raw SQL queries
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TDocumentKey"></typeparam>
        /// <typeparam name="TDocumentNumber"></typeparam>
        /// <returns></returns>
        private TDocumentNumber ReadNumber<TDocumentKey,TDocumentNumber>(IEntity<TDocumentKey> entity)
        {
            TDocumentNumber result = default;
            var type = entity.GetType();

            var model = this.DbContext.Model;
            var entityTypes = model.GetEntityTypes();
            var entityType = entityTypes.First(t => t.ClrType == type);
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            var tableName = tableNameAnnotation.Value.ToString();

            var query = $"SELECT TOP 1 [Number] FROM {tableName} WHERE ID = @id";

            using (var command = this.DbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                var parameter = new SqlParameter("@id", entity.ID);
                command.Parameters.Add(parameter);

                this.DbContext.Database.OpenConnection();

                using (var readerResult = command.ExecuteReader())
                {
                    while (readerResult.Read())
                    {
                        result = (TDocumentNumber)readerResult[0];
                    }
                }
            }

            return result;
        }


        /// <summary>
        ///     Asynchronously saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                this.UpdateSoftDeletableEntities();
                this.SkipNumbersUpdate();
                this.UpdateDocumentEntities();
                var result = await this.DbContext.SaveChangesAsync();
                this.AssignNumbers();
                return result;
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
        /// <exception cref="InvalidOperationException">
        ///     Throws an exception if more than one element satisfies
        ///     the condition
        /// </exception>
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the table</param>
        /// <returns>The element that satisfies the given predicate</returns>
        public virtual TEntity SingleOrDefault(Func<TEntity, bool> filter)
        {
            return this.Entities.SingleOrDefault(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Returns the only element of the table with the given <value>Id</value>
        /// </summary>
        /// <param name="id">The Id of the desired element</param>
        /// <returns>The element with the given Id</returns>
        public virtual TEntity SingleOrDefault(TKey id)
        {
            return this.Entities.SingleOrDefault(ent => ent.ID.Equals(id));
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
        public virtual async Task<TEntity> SingleOrDefaultAsync(Func<TEntity, bool> filter)
        {
            return await this.Entities.SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Asynchronously returns the only element of the table with the given <value>Id</value>
        /// </summary>
        /// <param name="id">The Id of the desired element</param>
        /// <returns>The element with the given Id</returns>
        public virtual async Task<TEntity> SingleOrDefaultAsync(TKey id)
        {
            return await this.Entities.SingleOrDefaultAsync(ent => ent.ID.Equals(id));
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
        public virtual TEntity Update(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!this.Exists(obj.ID))
            {
                throw new ArgumentException("The given object does not exist in DB", nameof(obj));
            }

            this.Entities.Update(obj);
            return obj;
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
        public virtual async Task<TEntity> UpdateAsync(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!this.Exists(obj.ID))
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
        ///     Begins tracking objects given in <paramref name="obj"/>
        /// <remarks>
        ///     All the properties will be marked
        ///     as modified. To mark only some properties use the
        ///     <see cref="M:Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0)"/>
        /// </remarks>
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        public virtual IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!obj.Any())
            {
                throw new ArgumentException("The given param must contains at least on element", nameof(obj));
            }

            this.Entities.UpdateRange(obj);
            return obj;
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
        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            if (!obj.Any())
            {
                throw new ArgumentException("The given param must contains at least on element", nameof(obj));
            }

            await Task.Factory.StartNew(() =>
            {
                this.Entities.UpdateRange(obj);
            });
            return obj;
        }

        /// <summary>
        ///     Create and instance of the DBConnection and open it
        /// </summary>
        /// <returns>An open instance of a connection to the DB.</returns>
        //protected IDbConnection OpenConnection()
        //{
        //    var connection = DbConnection;
        //    connection.Open();
        //    return connection;
        //}

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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }
        #endregion

        #region Franchise
        public int GetCurrentFranchise(int companyId)
        {
            var dbContext = (this.DbContext as MGCapDbContext);
            return dbContext.Companies?.FirstOrDefault(ent => ent.ID == companyId)
                                       ?.FranchiseId ?? 0;
        }
        #endregion

    }
}
