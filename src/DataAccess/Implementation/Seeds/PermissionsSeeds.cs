using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class PermissionsSeeds
    {
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.Permissions.Any())
            {
                return;
            }

            var roles = new List<Permission>
            {
                new Permission
                {
                    Name = "Users"
                },
                new Permission
                {
                    Name = "Permissions"
                },
                new Permission
                {
                    Name = "Proposals"
                },
                new Permission
                {
                    Name = "Analytics"
                },
                new Permission
                {
                    Name = "Services"
                },
                new Permission
                {
                    Name = "Buildings"
                },
                new Permission
                {
                    Name = "SupplyManagement"
                },
                new Permission
                {
                    Name = "Calendar"
                },
                new Permission
                {
                    Name = "WorkOrders"
                },
                new Permission
                {
                    Name = "Inspections"
                },
                new Permission
                {
                    Name = "ReadBuildings"
                },
                new Permission
                {
                    Name = "CleaningReport"
                },
                new Permission
                {
                    Name = "WorkOrderBillableReport"
                },
                new Permission
                {
                    Name = "Tickets"
                }
            };

            context.Permissions.AddRange(roles);
        }
    }
}
