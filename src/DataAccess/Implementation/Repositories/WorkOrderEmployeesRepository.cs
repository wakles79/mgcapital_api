using Dapper;
using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MGCap.Domain.Options;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderEmployeesRepository : IWorkOrderEmployeesRepository
    {
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderEmployeesRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The implementation of <see cref="DbContext"/></param>
        /// <param name="mgcapDbOptions">The implementation of <see cref="MGCapDbOptions"/></param>
        public WorkOrderEmployeesRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository)
        {
            this.DbContext = dbContext;
            this._baseDapperRepository = baseDapperRepository;
            if (this.DbContext is MGCapDbContext)
            {
                this.Entities = (this.DbContext as MGCapDbContext).Set<WorkOrderEmployee>();
            }
        }

        private IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Gets dB representation of the object
        ///     of type WorkOrderEmployee.
        /// </summary>
        public DbSet<WorkOrderEmployee> Entities { get; }


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
        public virtual WorkOrderEmployee Add(WorkOrderEmployee obj)
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
        public virtual async Task<WorkOrderEmployee> AddAsync(WorkOrderEmployee obj)
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

        /// </remarks>
        /// </summary>
        /// <param name="objs">The objects to be added</param>
        /// <returns>The given <paramref name="objs"/> after being inserted</returns>
        public virtual IEnumerable<WorkOrderEmployee> AddRange(IEnumerable<WorkOrderEmployee> objs)
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
        /// </remarks>
        /// </summary>
        /// <param name="objs">The objects to be added</param>
        /// <returns>The given <paramref name="objs"/> after being inserted</returns>
        public virtual async Task<IEnumerable<WorkOrderEmployee>> AddRangeAsync(IEnumerable<WorkOrderEmployee> objs)
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
        public virtual bool Exists(WorkOrderEmployee obj)
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

        /// <summary>
        ///     Checks if there is at least one element that satisfies the condition
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the Table</param>
        /// <returns><value>True</value> if any element satisfies the condition; otherwise, false</returns>
        public virtual bool Exists(Func<WorkOrderEmployee, bool> filter)
        {
            return this.Entities.Any(filter);
        }

        /// <summary>
        ///     Asynchronously check if the given <paramref name="obj"/> exists in the table
        /// </summary>
        /// <param name="obj">The element to locate in the Table</param>
        /// <returns><value>True</value> if the given object exists, false otherwise</returns>
        public virtual async Task<bool> ExistsAsync(WorkOrderEmployee obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            return await this.Entities.ContainsAsync(obj);
        }

        /// <summary>
        ///     Asynchronously checks if there is at least one element that satisfies the condition
        /// </summary>
        /// <param name="filter">The predicate to be applied for each element in the Table</param>
        /// <returns><value>True</value> if any element satisfies the condition; otherwise, false</returns>
        public virtual async Task<bool> ExistsAsync(Func<WorkOrderEmployee, bool> filter)
        {
            return await this.Entities.AnyAsync(ent => filter.Invoke(ent), cancellationToken: default(CancellationToken));
        }

        /// <summary>
        ///     Filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public virtual IQueryable<WorkOrderEmployee> ReadAll(Func<WorkOrderEmployee, bool> filter)
        {
            return this.Entities.Include(ent => ent.WorkOrder)
                                .Include(ent => ent.Employee)
                                .Where(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Asynchronously filter the elements in the table based on
        ///     the given predicate
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>The elements that satisfy the predicate <paramref name="filter"/></returns>
        public virtual async Task<IQueryable<WorkOrderEmployee>> ReadAllAsync(Func<WorkOrderEmployee, bool> filter)
        {
            return await Task.Factory.StartNew(() =>
            {
                return this.Entities.Where(ent => filter.Invoke(ent));
            });
        }

        /// <summary>
        ///     Begins tracking all the Entities that satisfy
        ///     the predicate given in <paramref name="filter"/> in the
        ///     state such that it will be removed when <see cref="IWorkOrderEmployeesRepository{WorkOrderEmployee,int}.Update"/> is called
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        public virtual void Remove(Func<WorkOrderEmployee, bool> filter)
        {
            var elementsToRemove = Entities.Where(ent => filter.Invoke(ent));
            Entities.RemoveRange(elementsToRemove);
        }

        /// <summary>
        ///     Begins tracking the given Entity
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IWorkOrderEmployeesRepository{WorkOrderEmployee,int}.Update"/> is called
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        public virtual void Remove(WorkOrderEmployee obj)
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
        ///     state such that it will be removed when <see cref="IWorkOrderEmployeesRepository{WorkOrderEmployee,int}.Update"/> is called
        /// </summary>
        /// <param name="filter">A function to be applied in each element of the table</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(Func<WorkOrderEmployee, bool> filter)
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
        ///     state such that it will be removed when <see cref="IWorkOrderEmployeesRepository{WorkOrderEmployee,int}.Update"/> is called
        /// </summary>
        /// <param name="obj">The objects to be marked</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(WorkOrderEmployee obj)
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
        ///     Begins tracking the given Entities
        ///     in the <see cref="F:Microsof.EntityFrameworkCore.EntityState.Deleted"/>
        ///     state such that it will be removed when <see cref="IWorkOrderEmployeesRepository{WorkOrderEmployee,int}.Update"/> is called
        /// </summary>
        /// <param name="objs">The objects to be marked</param>
        public virtual void RemoveRange(IEnumerable<WorkOrderEmployee> objs)
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
        ///     state such that it will be removed when <see cref="IWorkOrderEmployeesRepository{WorkOrderEmployee,int}.Update"/> is called
        /// </summary>
        /// <param name="objs">The objects to be marked</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RemoveRangeAsync(IEnumerable<WorkOrderEmployee> objs)
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

        /// <summary>
        ///     Saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public int SaveChanges()
        {
            try
            {
                return this.DbContext.SaveChanges();
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
        ///     Asynchronously saves all changes made in the Context to the Database
        /// </summary>
        /// <returns>The number of state entries written to the DB</returns>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await this.DbContext.SaveChangesAsync();
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
        public virtual WorkOrderEmployee SingleOrDefault(Func<WorkOrderEmployee, bool> filter)
        {
            return this.Entities.SingleOrDefault(ent => filter.Invoke(ent));
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
        public virtual async Task<WorkOrderEmployee> SingleOrDefaultAsync(Func<WorkOrderEmployee, bool> filter)
        {
            return await this.Entities.SingleOrDefaultAsync(ent => filter.Invoke(ent));
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
        public virtual WorkOrderEmployee Update(WorkOrderEmployee obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
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
        public virtual async Task<WorkOrderEmployee> UpdateAsync(WorkOrderEmployee obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
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
        public virtual IEnumerable<WorkOrderEmployee> UpdateRange(IEnumerable<WorkOrderEmployee> obj)
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
        public virtual async Task<IEnumerable<WorkOrderEmployee>> UpdateRangeAsync(IEnumerable<WorkOrderEmployee> obj)
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

        #region IDisposable Support
        /// <summary>
        ///     Release the allocated resources of the <see cref="DbContext"/>
        /// </summary>m
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

        public async Task RemoveByWorkOrderIdsAsync(IEnumerable<int> woIds)
        {
            string query = "DELETE FROM WorkOrderEmployees WHERE WorkOrderId IN @workOrderIds";
            var pars = new DynamicParameters();
            pars.Add("@workOrderIds", woIds);

            await this._baseDapperRepository.ExecuteAsync(query, pars);
        }

        public async Task AssignEmployeesDapperAsync(List<EntityEmployee> workOrderEmployees)
        {
            if (workOrderEmployees == null || !workOrderEmployees.Any())
            {
                return;
            }

            var innerValues = new List<string>();

            var pars = new DynamicParameters();
            for (int i = 0; i < workOrderEmployees.Count; i++)
            {
                var e = workOrderEmployees[i];
                pars.Add($"@workOrderId{i}", e.EntityId);
                pars.Add($"@employeeId{i}", e.EmployeeId);
                pars.Add($"@type{i}", e.Type);

                innerValues.Add($"(@workOrderId{i}, @employeeId{i}, @type{i})");
            }
            string query = $@"
                INSERT INTO WorkOrderEmployees
                ( WorkOrderId, EmployeeId, [Type] )
                VALUES {string.Join(",", innerValues)}";

            await this._baseDapperRepository.ExecuteAsync(query, pars);
        }

        #endregion

    }
}
