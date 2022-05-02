// -----------------------------------------------------------------------
// <copyright file="IContactAddressesRepository.cs" company="Axzes">
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
    public interface IVendorAddressesRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{ContactAddress}"/> to perform actions on the
        ///     table that the <typeparamref name="ContactAddress"/>
        ///     represents
        /// </summary>
        DbSet<VendorAddress> Entities { get; }

        VendorAddress Add(VendorAddress obj);

        Task<VendorAddress> AddAsync(VendorAddress obj);

        IQueryable<VendorAddress> ReadAll(Func<VendorAddress, bool> filter);

        Task<IQueryable<VendorAddress>> ReadAllAsync(Func<VendorAddress, bool> filter);

        void Remove(Func<VendorAddress, bool> filter);

        Task<VendorAddress> UpdateAsync(VendorAddress obj);

        bool Exists(int contactId, int addressId);

        Task<VendorAddress> SingleOrDefaultAsync(Func<VendorAddress, bool> filter);

        void Remove(VendorAddress obj);
    }
}
