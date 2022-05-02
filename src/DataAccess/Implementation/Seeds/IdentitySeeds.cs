using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    /// <summary>
    ///     Apply the seeds for the Identity's related 
    ///     objects.
    /// </summary>
    public class IdentitySeeds
    {
        /// <summary>
        ///     If True, apply the migrations not matter that the Table has object.
        ///     If False, only apply the migrations if the Table does not have  al least one object.
        /// </summary>
        private const bool APPLY_MIGRATION = false;

        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifying the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapIdentityDbContext context, int count = 1)
        {
            AddUser(context);
            //AddRole(context);
            //AddBasicPermissions(context);
            //EnsureRoleAssignation(context);
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
            context.SaveChanges();
        }


    }
}
