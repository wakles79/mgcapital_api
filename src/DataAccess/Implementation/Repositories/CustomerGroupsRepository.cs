// -----------------------------------------------------------------------
// <copyright file="CustomerGroupsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="CustomerGroup"/>
    /// </summary>
    public class CustomerGroupsRepository : BaseRepository<CustomerGroup, int>, ICustomerGroupsRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomerGroupsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public CustomerGroupsRepository(
            MGCapDbContext dbContext)
            : base(dbContext)
        {
        }

    }
}
