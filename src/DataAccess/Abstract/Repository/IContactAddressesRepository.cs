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
    public interface IContactAddressesRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{ContactAddress}"/> to perform actions on the
        ///     table that the <typeparamref name="ContactAddress"/>
        ///     represents
        /// </summary>
        DbSet<ContactAddress> Entities { get; }

        ContactAddress Add(ContactAddress obj);

        Task<ContactAddress> AddAsync(ContactAddress obj);

        IQueryable<ContactAddress> ReadAll(Func<ContactAddress, bool> filter);

        Task<IQueryable<ContactAddress>> ReadAllAsync(Func<ContactAddress, bool> filter);

        void Remove(Func<ContactAddress, bool> filter);

        Task<ContactAddress> UpdateAsync(ContactAddress obj);

        bool Exists(int contactId, int addressId);

        Task<ContactAddress> SingleOrDefaultAsync(Func<ContactAddress, bool> filter);

        void Remove(ContactAddress obj);
    }
}
