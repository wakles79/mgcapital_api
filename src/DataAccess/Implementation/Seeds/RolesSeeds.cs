using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class RolesSeeds
    {
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.Roles.Any())
            {
                return;
            }

            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Master",
                    Level = 10
                },
                new Role
                {
                    Name = "Office Staff",
                    Level = 20
                },
                new Role
                {
                    Name = "Operation Manager",
                    Level = 30
                },
                new Role
                {
                    Name = "Subcontractor-Operation Manager",
                    Level = 35
                },
                new Role
                {
                    Name = "Supervisor",
                    Level = 40
                },
                new Role
                {
                    Name = "Subcontractor-Supervisor",
                    Level = 45
                },
                new Role
                {
                    Name = "Customer",
                    Level = 110
                },
            };

            context.Roles.AddRange(roles);
        }
    }
}
