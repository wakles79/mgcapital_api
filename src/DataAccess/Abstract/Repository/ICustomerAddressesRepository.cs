// -----------------------------------------------------------------------
// <copyright file="ICustomerAddressesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using MGCap.Domain.Models;
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
    public interface ICustomerAddressesRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{CustomerAddress}"/> to perform actions on the
        ///     table that the <typeparamref name="CustomerAddress"/>
        ///     represents
        /// </summary>
        DbSet<CustomerAddress> Entities { get; }

        CustomerAddress Add(CustomerAddress obj);

        Task<CustomerAddress> AddAsync(CustomerAddress obj);

        IQueryable<CustomerAddress> ReadAll(Func<CustomerAddress, bool> filter);

        Task<IQueryable<CustomerAddress>> ReadAllAsync(Func<CustomerAddress, bool> filter);

        void Remove(Func<CustomerAddress, bool> filter);

        Task<CustomerAddress> UpdateAsync(CustomerAddress obj);

        bool Exists(int customerId, int addressId);

        Task<CustomerAddress> SingleOrDefaultAsync(Func<CustomerAddress, bool> filter);

        void Remove(CustomerAddress obj);
    }
}
