using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    /// <summary>
    ///     Contains the operation for seeding the
    ///     Company's table
    /// </summary>
    public static class CompanySeeds
    {
        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifiyng the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.Companies.Any())
            {
                return;
            }
            var companiesList = new List<Company>
            {
                new Company
                {
                    Name = "MG Capital Maintenance",
                    CreatedBy ="Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow
                },
                new Company
                {
                    Name = "Sandbox",
                    CreatedBy ="Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow

                },
                //new Company
                //{
                //    Name = "Sjofn's Company",
                //    CreatedBy ="Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow
                //},
                //new Company
                //{
                //    Name = "Thor's Company",
                //    CreatedBy ="Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow
                //},
            };
            context.Companies.AddRange(companiesList);
        }
    }
}
