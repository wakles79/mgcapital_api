// -----------------------------------------------------------------------
// <copyright file="ICustomerEmployeesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CustomerEmployee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public interface ICustomerEmployeesRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{CustomerEmployee}"/> to perform actions on the
        ///     table that the <typeparamref name="CustomerEmployee"/>
        ///     represents
        /// </summary>
        DbSet<CustomerEmployee> Entities { get; }

        CustomerEmployee Add(CustomerEmployee obj);

        Task<CustomerEmployee> AddAsync(CustomerEmployee obj);

        IQueryable<CustomerEmployee> ReadAll(Func<CustomerEmployee, bool> filter);

        Task<IQueryable<CustomerEmployee>> ReadAllAsync(Func<CustomerEmployee, bool> filter);

        void Remove(Func<CustomerEmployee, bool> filter);

        Task<CustomerEmployee> UpdateAsync(CustomerEmployee obj);

        bool Exists(int customerId, int employeeId);

        Task<CustomerEmployee> SingleOrDefaultAsync(Func<CustomerEmployee, bool> filter);

        void Remove(CustomerEmployee obj);

        Task<DataSource<CustomerEmployeeGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int customerId);
    }
}
