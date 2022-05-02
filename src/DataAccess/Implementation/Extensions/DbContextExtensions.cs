// -----------------------------------------------------------------------
// <copyright file="DbContextExtensions.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Extensions
{
    /// <summary>
    ///     Contains functionalities that extends the
    ///     <see cref="DbContext"/>
    /// </summary>
   public static class DbContextExtensions
    {
        /// <summary>
        ///     Checks if all the migrations are
        ///     applied to the context.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> to check</param>
        /// <returns>True if all the migrations are applied;otherwise, false/</returns>
        /// 
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}
