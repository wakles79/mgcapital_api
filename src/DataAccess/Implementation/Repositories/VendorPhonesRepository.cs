// -----------------------------------------------------------------------
// <copyright file="VendorPhonesRepository.cs" company="Axzes">
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
    class VendorPhonesRepository : BaseRepository<VendorPhone, int>, IVendorPhonesRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VendorPhonesRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public VendorPhonesRepository(
            MGCapDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
