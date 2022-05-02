// -----------------------------------------------------------------------
// <copyright file="MGCapIdentityDbContextExtensions.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace MGCap.DataAccess.Implementation.Extensions
{
    /// <summary>
    ///     Contains functionalities that extends the
    ///     <see cref="MGCapIdentityDbContext"/>
    /// </summary>
    public static class MGCapIdentityDbContextExtensions
    {
        /// <summary>
        ///     Applies the seeds to the context
        /// </summary>
        /// <param name="context">
        ///     The <see cref="DbContext"/> where to
        ///     apply the seeds
        /// </param>
        public static void EnsureSeedData(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<MGCapIdentityDbContext>();
            if (context.AllMigrationsApplied())
            {
                AddUser(context);
                context.SaveChanges();

                // Save changes and release resources
                context.Dispose();
            }
        }

        /// <summary>
        ///     Seed for having a User in the DB
        /// </summary>
        /// <param name="context">The context where to apply the seed</param>
        public static void AddUser(MGCapIdentityDbContext context)
        {
            var emails = new List<string> { "axzesllc@gmail.com", "robin@axzes.com" };
            if (!context.Users.Any(u => emails.Contains(u.Email)))
            {
                context.Users.AddRange(
                    new List<ApplicationUser> {
                        new ApplicationUser
                        {
                            Email = emails[0],
                            UserName = emails[0],
                            EmailConfirmed = true,
                            NormalizedEmail = emails[0].ToUpper(),
                            NormalizedUserName = emails[0].ToUpper(),
                            PasswordHash = "AQAAAAEAACcQAAAAEMsMF+AmwcVzPo5IUnUATj6ca2sVoW/KgCVCAqZBUCPJAOYfWvHxRhmEOemeZ3h3DA==", // Fernand0.
                            SecurityStamp = "e3e1cfe3-f6e3-4ace-a0d7-89c5a4b8e318",
                        },
                        new ApplicationUser
                        {
                            Email = emails[1],
                            UserName = emails[1],
                            EmailConfirmed = true,
                            NormalizedEmail = emails[1].ToUpper(),
                            NormalizedUserName = emails[1].ToUpper(),
                            PasswordHash = "AQAAAAEAACcQAAAAEMsMF+AmwcVzPo5IUnUATj6ca2sVoW/KgCVCAqZBUCPJAOYfWvHxRhmEOemeZ3h3DA==", // Fernand0.
                            SecurityStamp = "e3e1cfe3-f6e3-4ace-a0d7-89c5a4b8e318",
                        },
                });
            }
        }
    }
}
