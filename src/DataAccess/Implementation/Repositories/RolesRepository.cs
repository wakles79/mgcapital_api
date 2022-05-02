// -----------------------------------------------------------------------
// <copyright file="RolesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="RolesRepository"/>
    /// </summary>
    public class RolesRepository : BaseRepository<Role, int>, IRolesRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RolesRepository"/> class.
        /// </summary>
        public RolesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }
    }
}
