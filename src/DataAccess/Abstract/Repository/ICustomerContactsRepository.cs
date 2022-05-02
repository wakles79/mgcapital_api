// -----------------------------------------------------------------------
// <copyright file="ICustomerContactsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Contact;
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
    public interface ICustomerContactsRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{CustomerContact}"/> to perform actions on the
        ///     table that the <typeparamref name="CustomerContact"/>
        ///     represents
        /// </summary>
        DbSet<CustomerContact> Entities { get; }

        CustomerContact Add(CustomerContact obj);

        Task<CustomerContact> AddAsync(CustomerContact obj);

        IQueryable<CustomerContact> ReadAll(Func<CustomerContact, bool> filter);

        Task<IQueryable<CustomerContact>> ReadAllAsync(Func<CustomerContact, bool> filter);

        void Remove(Func<CustomerContact, bool> filter);

        Task<CustomerContact> UpdateAsync(CustomerContact obj);

        bool Exists(int customerId, int addressId);

        Task<CustomerContact> SingleOrDefaultAsync(Func<CustomerContact, bool> filter);

        void Remove(CustomerContact obj);

        Task<DataSource<ContactGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int customerId);
    }
}
