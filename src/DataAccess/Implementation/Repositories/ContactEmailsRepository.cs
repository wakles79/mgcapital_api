// -----------------------------------------------------------------------
// <copyright file="ContactEmailsRepository.cs" company="Axzes">
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
    class ContactEmailsRepository : BaseRepository<ContactEmail,int>, IContactEmailsRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactEmailsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public ContactEmailsRepository(
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
        public new async Task<ContactEmail> SingleOrDefaultAsync(Func<ContactEmail, bool> filter)
        {
            return await Entities
                .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="ContactEmail"/> represents
        ///     applying a filter
        /// </summary>
        /// <returns>A list with all the ContactEmail with the given filter</returns>
        public async Task<ContactEmail> FirstOrDefaultAsync(Func<ContactEmail, bool> filter)
        {
            return await Entities
                .FirstOrDefaultAsync(ent => filter.Invoke(ent));
        }
    }
}
