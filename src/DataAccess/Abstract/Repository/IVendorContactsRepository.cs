// -----------------------------------------------------------------------
// <copyright file="IVendorContactsRepository.cs" company="Axzes">
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
    public interface IVendorContactsRepository
    {
        /// <summary>
        ///     Gets the <see cref="DbSet{VendorContact}"/> to perform actions on the
        ///     table that the <typeparamref name="VendorContact"/>
        ///     represents
        /// </summary>

        DbSet<VendorContact> Entities { get; }

        VendorContact Add(VendorContact obj);

        Task<VendorContact> AddAsync(VendorContact obj);

        IQueryable<VendorContact> ReadAll(Func<VendorContact, bool> filter);

        Task<IQueryable<VendorContact>> ReadAllAsync(Func<VendorContact, bool> filter);

        void Remove(Func<VendorContact, bool> filter);

        Task<VendorContact> UpdateAsync(VendorContact obj);

        bool Exists(int contactId, int addressId);

        Task<VendorContact> SingleOrDefaultAsync(Func<VendorContact, bool> filter);

        void Remove(VendorContact obj);

        Task<DataSource<ContactGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int vendorId);
    }
}
