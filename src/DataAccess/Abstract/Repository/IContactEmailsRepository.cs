// -----------------------------------------------------------------------
// <copyright file="IContactEmailsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
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
    public interface IContactEmailsRepository : IBaseRepository<ContactEmail,int>
    {
        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="ContactEmail"/> represents
        ///     applying a filter
        /// </summary>
        /// <returns>A list with all the Employees with the given filter</returns>
        new Task<ContactEmail> SingleOrDefaultAsync(Func<ContactEmail, bool> filter);

        new Task<ContactEmail> FirstOrDefaultAsync(Func<ContactEmail, bool> filter);
    }
}
