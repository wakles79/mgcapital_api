// <copyright file="MGCapIdentityDbContext.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Context
{
    /// <summary>
    ///     Implementation of the <see cref="IdentityDbContext"/>
    /// </summary>
   public class MGCapIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public MGCapIdentityDbContext(DbContextOptions<MGCapIdentityDbContext> options)
    : base(options)
        {
        }

        /// <summary>
        /// Customize the ASP.NET Identity model and override the defaults if needed.
        /// For example, you can rename the ASP.NET Identity table names and more.
        /// Add your customizations after calling base.OnModelCreating(builder);
        /// </summary>
        /// <param name="builder">The builder for constructing this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
