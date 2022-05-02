// -----------------------------------------------------------------------
// <copyright file="BaseSessionApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using MGCap.Business.Implementation.Extensions;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.DataAccess.Implementation.Repositories;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Serilog;
using System.Threading.Tasks;
using System.Linq;
using MGCap.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MGCap.Business.Implementation.Services;
using Serilog.Events;
using Serilog.Context;
using System.Data.Common;
using Dapper;

namespace MGCap.Business.Implementation.ApplicationServices
{
    /// <summary>
    ///     Contains the main functionalities
    ///     for handling operations related with
    ///     the User's session
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the class with handle</typeparam>
    /// <typeparam name="TKey">The type of the PK of the handled Entity</typeparam>
    public abstract class BaseSessionFranchiseApplicationService<TEntity, TKey> : BaseApplicationService<TEntity, TKey>
        where TEntity : AuditableFranchiseEntity<TKey>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseSessionFranchiseApplicationService{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="repository">Inject an <see cref="IBaseRepository{TEntity,TKey}"/> instance</param>
        /// <param name="httpContextAccessor">Inject an <see cref="IHttpContextAccessor"/> for accessing values in the <see cref="HttpContext"/></param>
        protected BaseSessionFranchiseApplicationService(
            IBaseRepository<TEntity, TKey> repository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository)
        {
            HttpContextAccessor = httpContextAccessor;
            UpdateDatabase = false;
        }

        private int? _franchiseId = null;

        /// <summary>
        /// Gets the <value>FranchiseId</value> of the current User
        /// </summary>
        public int? FranchiseId
        {
            get
            {
                return _franchiseId ?? (_franchiseId = this.Repository.GetCurrentFranchise(this.CompanyId));
            }
        }

        /// <summary>
        ///     Gets the functionalities of the DataAccess layer
        /// </summary>
        public new BaseRepository<TEntity, TKey> Repository => base.Repository as BaseRepository<TEntity, TKey>;

        /// <summary>
        /// Gets the Actual DBContext
        /// </summary>
        public DbContext DbContext => this.Repository.DbContext;

        /// <summary>
        /// Gets <value>IHttpContextAccessor field</value>
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor { get; }

        /// <summary>
        /// Gets the <value>strCompanyId</value> of the current User
        /// </summary>
        public string StrCompanyId => this.HttpContextAccessor?.HttpContext?.Request?.Headers["CompanyId"];

        /// <summary>
        ///     Gets the <value>UserEmail</value> of the current User
        /// </summary>
        public string UserEmail => this.HttpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";

        /// <summary>
        /// Gets the <value>CompanyId</value> of the current User
        /// </summary>
        public int CompanyId => string.IsNullOrEmpty(this.StrCompanyId) ? 0 : int.Parse(this.StrCompanyId);

        /// <summary>
        /// Gets or sets a value indicating whether for future 'Caching/Optimization' this flag should inform whenever is necessary
        /// to make a DB query or a 'Memory/Session' query
        /// </summary>
        protected static bool UpdateDatabase { get; set; }


        #region Overridden Methods

        /// <summary>
        ///     Overrides the <see cref="BaseApplicationService{TEntity,TKey}.Add"/>
        ///     to assign the Company to the given <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">object to be added</param>
        /// <returns>The modified entity</returns>
        public new TEntity Add(TEntity obj)
        {
            this.AssignFranchise(obj);
            //Log.Debug($"The User with email: {this.EmployeeEmail} and CompanyID: {this.CompanyId}, added a new entity of type: {nameof(obj)}");
            return this.Repository.Add(obj);
        }

        /// <summary>
        ///     Asyncronously overrides the <see cref="BaseApplicationService{TEntity,TKey}.AddAsync"/>
        ///     to assign the Company to the given <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">object to be added</param>
        /// <returns>The modified entity</returns>
        public new async Task<TEntity> AddAsync(TEntity obj)
        {
            this.AssignFranchise(obj);
            //Log.Debug($"The User with email: {this.EmployeeEmail} and CompanyID: {this.CompanyId}, added a new entity of type: {nameof(obj)}");
            return await this.Repository.AddAsync(obj);
        }

        /// <summary>
        ///     Overrides the <see cref="BaseApplicationService{TEntity,TKey}.AddRange"/>
        ///     to assign the Company to the given <paramref name="objs"/>
        /// </summary>
        /// <param name="objs">objects to be added</param>
        /// <returns>The modified entities</returns>
        public new IEnumerable<TEntity> AddRange(IEnumerable<TEntity> objs)
        {
            var entities = objs as TEntity[] ?? objs.ToArray();
            foreach (var obj in entities)
            {
                this.AssignFranchise(obj);
            }

            //Log.Debug($"The User with email: {this.EmployeeEmail} and CompanyID: {this.CompanyId}, added {entities.Count()} new entities of type: {nameof(objs)}");
            return this.Repository.AddRange(entities);
        }

        /// <summary>
        ///     Asyncronously overrides the <see cref="BaseApplicationService{TEntity,TKey}.AddRangeAsync"/>
        ///     to assign the Company to the given<paramref name="objs"/>
        /// </summary>
        /// <param name="objs">objects to be added</param>
        /// <returns>The modified entities</returns>
        public new async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> objs)
        {
            var entities = objs as TEntity[] ?? objs.ToArray();
            foreach (var obj in entities)
            {
                this.AssignFranchise(obj);
            }

            //Log.Debug($"The User with email: {this.EmployeeEmail} and CompanyID: {this.CompanyId}, added {entities.Count()} new entities of type: {nameof(objs)}");

            return await this.Repository.AddRangeAsync(entities);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.Exists(TEntity)"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>True if there is an element that satisfies the filter and belongs to the User's company</returns>
        public new bool Exists(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            return this.Repository.Exists(ent => filter(ent) && ent.FranchiseId == id);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.ExistsAsync(TEntity)"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>True if there is an element that satisfies the filter and belongs to the User's company</returns>
        public new async Task<bool> ExistsAsync(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            return await this.Repository.ExistsAsync(ent => filter(ent) && ent.FranchiseId == id);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.ReadAll"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>All the elements that satisfies the filter and belongs to the User's company</returns>
        public new IQueryable<TEntity> ReadAll(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            // We Read all elements filtered by CompanyId and Ordered by "CreatedDate" Desc
            return this.Repository
                       .ReadAll(ent => filter(ent) && ent.FranchiseId == id)
                       .OrderByDescending(ent => ent.CreatedDate);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.ReadAllAsync"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>All the elements that satisfies the filter and belongs to the User's company</returns>
        public new async Task<IQueryable<TEntity>> ReadAllAsync(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            return await this.Repository
                             .ReadAllAsync(ent => filter(ent) && ent.FranchiseId == id);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.Remove(System.Func{TEntity,bool})"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        public new void Remove(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            this.Repository.Remove(ent => filter(ent) && ent.FranchiseId == id);
            //Log.Debug($"The User with email: {this.EmployeeEmail} and CompanyID: {this.CompanyId}, removed entities of type: {nameof(TEntity)}");
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.RemoveAsync(System.Func{TEntity,bool})"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public new async Task RemoveAsync(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            var task = this.Repository.RemoveAsync(ent => filter(ent) && ent.FranchiseId == id);
            //Log.Debug($"The User with email: {this.EmployeeEmail} and CompanyID: {this.CompanyId}, removed entities of type: {nameof(TEntity)}");

            await task;
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.SingleOrDefault(System.Func{TEntity,bool})"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>The element that pass the filter.</returns>
        public new TEntity SingleOrDefault(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            return this.Repository.SingleOrDefault(ent => filter(ent) && ent.FranchiseId == id);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.SingleOrDefault(TKey)"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="id">The ID of the element to be returned</param>
        /// <returns>The element with the given ID; the default value if there is no element with the given ID.</returns>
        public new TEntity SingleOrDefault(TKey id)
        {
            var fId = this.FranchiseId;
            return this.Repository.SingleOrDefault(ent => ent.ID.Equals(id) && ent.FranchiseId == fId);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.SingleOrDefaultAsync(System.Func{TEntity,bool})"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="filter">The filter to be applied</param>
        /// <returns>The element that pass the filter.</returns>
        public new async Task<TEntity> SingleOrDefaultAsync(Func<TEntity, bool> filter)
        {
            var id = this.FranchiseId;
            return await this.Repository.SingleOrDefaultAsync(ent => filter(ent) && ent.FranchiseId == id);
        }

        /// <summary>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.SingleOrDefaultAsync(TKey)"/>
        ///     but applying the Company filter
        /// </summary>
        /// <param name="id">The ID of the element to be returned</param>
        /// <returns>The element with the given ID; the default value if there is no element with the given ID.</returns>
        public new async Task<TEntity> SingleOrDefaultAsync(TKey id)
        {
            var fId = this.FranchiseId;
            return await this.Repository.SingleOrDefaultAsync(ent => ent.ID.Equals(id) && ent.FranchiseId == fId);
        }

        /// <summary>
        /// <para>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.Update(TEntity)"/>
        ///     but applying the Company filter
        /// </para>
        /// <para>
        ///     This operations updates the <see cref="AuditableEntity"/> properties automatically.
        /// </para>
        /// </summary>
        /// <param name="obj">The element to be updated</param>
        /// <returns>The modified element.</returns>
        public new TEntity Update(TEntity obj)
        {
            this.UntrackReadOnlyFields(obj);
            //var logEventUserEmailProperty = new LogEventProperty("EmailProperty", new ScalarValue(this.EmployeeEmail));
            //Log.BindProperty("UserEmail", this.EmployeeEmail, false, out logEventUserEmailProperty);
            //var logEventCompanyIdProperty = new LogEventProperty("CompanyId", new ScalarValue(this.EmployeeEmail));
            //Log.BindProperty("UserEmail", this.CompanyId, false, out logEventCompanyIdProperty);
            //using (LogContext.PushProperty("UserEmail", this.EmployeeEmail))
            //{
            //    using (LogContext.PushProperty("CompanyId", this.CompanyId))
            //    {
            //        Log.Debug($"The User with email: {EmployeeEmail} and CompanyID: {CompanyId}, updated an entity of type: {nameof(TEntity)} and ID: {(obj as Entity<TKey>).ID.ToString()}");

            //    }
            //}

            return this.Repository.Update(obj);
        }

        /// <summary>
        /// <para>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.UpdateAsync(TEntity)"/>
        ///     but applying the Company filter
        /// </para>
        /// <para>
        ///     This operations updates the <see cref="AuditableEntity"/> properties automatically.
        /// </para>
        /// </summary>
        /// <param name="obj">The element to be updated</param>
        /// <returns>The modified element.</returns>
        public new async Task<TEntity> UpdateAsync(TEntity obj)
        {
            this.UntrackReadOnlyFields(obj);
            //var logEventUserEmailProperty = new LogEventProperty("EmailProperty", new ScalarValue(this.EmployeeEmail));
            //Log.BindProperty("UserEmail", this.EmployeeEmail, false, out logEventUserEmailProperty);
            //var logEventCompanyIdProperty = new LogEventProperty("CompanyId", new ScalarValue(this.EmployeeEmail));
            //Log.BindProperty("UserEmail", this.CompanyId, false, out logEventCompanyIdProperty);
            //Log.Debug("The User with email: {EmployeeEmail} and CompanyID: {CompanyId}, updated an entity of type: {nameof(TEntity)} and ID: {(obj as Entity<TKey>).ID.ToString()}", logEventUserEmailProperty, logEventCompanyIdProperty);
            return await this.Repository.UpdateAsync(obj);
        }

        /// <summary>
        /// <para>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.UpdateRange(IEnumerable{TEntity})"/>
        /// </para>
        /// <para>
        ///     This operations updates the <see cref="AuditableEntity"/> properties automatically
        ///     for each of the given elements.
        /// </para>
        /// </summary>
        /// <param name="objs">The elements to be updated</param>
        /// <returns>The modified elements.</returns>
        public new IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> objs)
        {
            var entities = objs as TEntity[] ?? objs.ToArray();
            foreach (var obj in entities)
            {
                this.UntrackReadOnlyFields(obj);
            }

            return this.Repository.UpdateRange(entities);
        }

        /// <summary>
        /// <para>
        ///     Exact operations that the <see cref="BaseApplicationService{TEntity,TKey}.UpdateRangeAsync(IEnumerable{TEntity})"/>
        /// </para>
        /// <para>
        ///     This operations updates the <see cref="AuditableEntity"/> properties automatically
        ///     for each of the given elements.
        /// </para>
        /// </summary>
        /// <param name="objs">The elements to be updated</param>
        /// <returns>The modified elements.</returns>
        public new async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> objs)
        {
            foreach (var obj in objs)
            {
                this.UntrackReadOnlyFields(obj);
            }

            return await this.Repository.UpdateRangeAsync(objs);
        }

        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public new int SaveChanges()
        {
            this.UpdateAuditableEntities();
            return base.SaveChanges();
        }

        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <param name="updateAuditableFields">Flag to choose whenever to update or not all Auditable Fields</param>
        /// <returns>The number of state entries written to the DB</returns>
        public int SaveChanges(bool updateAuditableFields = true)
        {
            if (updateAuditableFields)
            {
                this.UpdateAuditableEntities();
            }

            return base.SaveChanges();
        }

        /// <summary>
        ///     Asynchronously saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public new async Task<int> SaveChangesAsync()
        {
            this.UpdateAuditableEntities();
            return await base.SaveChangesAsync();
        }

        /// <summary>
        ///      Asynchronously saves all changes made in the Context to the Database.
        /// </summary>
        /// <param name="updateAuditableFields">Flag to choose whenever to update or not all Auditable Fields</param>
        /// <returns>The number of state entries written to the DB</returns>
        public async Task<int> SaveChangesAsync(bool updateAuditableFields = true)
        {
            if (updateAuditableFields)
            {
                this.UpdateAuditableEntities();
            }

            return await base.SaveChangesAsync();
        }

        #endregion


        /// <summary>
        ///     Assign the value of the current User's company
        ///     to the element to be created/updated.
        /// </summary>
        /// <param name="obj">The element to be modified.</param>
        private void AssignFranchise(TEntity obj)
        {
            var Id = this.FranchiseId;
            obj.FranchiseId = Id;
        }

        /// <summary>
        ///     Leave all the read only fields unchanged.
        ///     This should be used in "update" methods only
        /// </summary>
        /// <param name="obj">The entity</param>
        private void UntrackReadOnlyFields(TEntity obj)
        {
            // State = Unchanged
            // this.Repository.Entities.Attach(obj);
            // var props = this.Repository.DbContext
            //    .Entry(obj)
            //    .Properties
            //    .Where(p => p.Metadata.Name != "ID" &&
            //                p.Metadata.Name != "CompanyId" &&
            //                p.Metadata.Name != "Guid");
            // foreach (var p in props)
            // {
            //    p.IsModified = true;
            // }
        }

        /// <summary>
        ///     Updates all fields from Auditable Entities
        /// </summary>
        private void UpdateAuditableEntities()
        {
            // Gets all modified entities as IAuditableEntity
            var modifiedEntries = this.DbContext
                                    .ChangeTracker
                                    .Entries()
                                    .Where(x => x.Entity is IAuditableEntity &&
                                                (x.State == EntityState.Added ||
                                                 x.State == EntityState.Modified ||
                                                 x.State == EntityState.Deleted));

            foreach (var entry in modifiedEntries)
            {
                if (entry?.Entity is IAuditableEntity entity)
                {
                    var now = DateTime.UtcNow;
                    var userName = this.UserEmail;
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedBy = userName;
                        entity.CreatedDate = now;
                    }
                    else
                    {
                        this.DbContext.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        this.DbContext.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    }

                    entity.UpdatedBy = userName;
                    entity.UpdatedDate = now;
                }
            }
        }


    }
}
