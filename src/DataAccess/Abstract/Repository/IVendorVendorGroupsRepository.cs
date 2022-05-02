// ----------------------------------------------------------------------- 
// <copyright file="IVendorVendorGroupsRepository.cs" company="Axzes"> 
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
    public interface IVendorVendorGroupsRepository
    {
        /// <summary> 
        ///     Gets the <see cref="DbSet{VendorVendorGroup}"/> to perform actions on the 
        ///     table that the <typeparamref name="VendorVendorGroup"/> 
        ///     represents 
        /// </summary> 

        DbSet<VendorVendorGroup> Entities { get; }

        VendorVendorGroup Add(VendorVendorGroup obj);

        Task<VendorVendorGroup> AddAsync(VendorVendorGroup obj);

        IQueryable<VendorVendorGroup> ReadAll(Func<VendorVendorGroup, bool> filter);

        Task<IQueryable<VendorVendorGroup>> ReadAllAsync(Func<VendorVendorGroup, bool> filter);

        void Remove(Func<VendorVendorGroup, bool> filter);

        Task<VendorVendorGroup> UpdateAsync(VendorVendorGroup obj);

        bool Exists(int contactId, int addressId);

        Task<VendorVendorGroup> SingleOrDefaultAsync(Func<VendorVendorGroup, bool> filter);

        void Remove(VendorVendorGroup obj);
    }
}
