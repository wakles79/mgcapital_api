﻿// ----------------------------------------------------------------------- 
// <copyright file="ICustomerCustomerGroupsRepository.cs" company="Axzes"> 
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
    public interface ICustomerCustomerGroupsRepository
    {
        /// <summary> 
        ///     Gets the <see cref="DbSet{CustomerCustomerGroup}"/> to perform actions on the 
        ///     table that the <typeparamref name="CustomerCustomerGroup"/> 
        ///     represents 
        /// </summary> 

        DbSet<CustomerCustomerGroup> Entities { get; }

        CustomerCustomerGroup Add(CustomerCustomerGroup obj);

        Task<CustomerCustomerGroup> AddAsync(CustomerCustomerGroup obj);

        IQueryable<CustomerCustomerGroup> ReadAll(Func<CustomerCustomerGroup, bool> filter);

        Task<IQueryable<CustomerCustomerGroup>> ReadAllAsync(Func<CustomerCustomerGroup, bool> filter);

        void Remove(Func<CustomerCustomerGroup, bool> filter);

        Task<CustomerCustomerGroup> UpdateAsync(CustomerCustomerGroup obj);

        bool Exists(int contactId, int addressId);

        Task<CustomerCustomerGroup> SingleOrDefaultAsync(Func<CustomerCustomerGroup, bool> filter);

        void Remove(CustomerCustomerGroup obj);
    }
} 
