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
    ///     Department's table
    /// </summary>
    public static class DepartmentsSeeds
    {
        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifiyng the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.Departments.Any())
            {
                return;
            }

            var companiesIds = context.Companies
                          .Select(c => c.ID)
                          .ToList();

            var departmentList = new List<Department>
            {
                new Department
                {
                    CompanyId = companiesIds[0],
                    Name = "Department 1",
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow,
                    Guid = Guid.NewGuid()
                },
                new Department
                {
                    CompanyId = companiesIds[1],
                    Name = "Department 2",
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow,
                    Guid = Guid.NewGuid()
                },
                //new Department
                //{
                //    CompanyId = companiesIds[2],
                //    Name = "Department 3",
                //    CreatedBy = "Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow,
                //    Guid = Guid.NewGuid()
                //},
                //new Department
                //{
                //    CompanyId = companiesIds[3],
                //    Name = "Department 4",
                //    CreatedBy = "Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow,
                //    Guid = Guid.NewGuid()
                //}
            };

            context.Departments.AddRange(departmentList);
        }
    }
}
