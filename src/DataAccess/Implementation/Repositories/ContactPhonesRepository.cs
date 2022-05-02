// -----------------------------------------------------------------------
// <copyright file="ContactPhonesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ContactPhonesRepository : BaseRepository<ContactPhone, int>, IContactPhonesRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactPhonesRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public ContactPhonesRepository(
            MGCapDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="ContactPhone"/> represents
        ///     applying a filter
        /// </summary>
        /// <returns>A list with all the Employees with the given filter</returns>
        public new async Task<ContactPhone> SingleOrDefaultAsync(Func<ContactPhone, bool> filter)
        {
            return await Entities
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }
    }
}
