using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class EmployeePermissionsSeeds
    {
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.EmployeePermissions.Any())
            {
                return;
            }

            // All employees with axzes email will be masters,
            // The rest wiill be office staff
            var masterRole = context.Roles.FirstOrDefault(r => r.Name == "Master");
            var officeStaffRole = context.Roles.FirstOrDefault(r => r.Name == "Office Staff");
            var masterPermissions = new List<PermissionRole>();
            var officeStaffPermissions = new List<PermissionRole>();
            if (masterRole != null)
            {
                masterPermissions = context.PermissionRoles.Where(pr => pr.RoleId == masterRole.ID)?.ToList();
            }

            if (officeStaffRole != null)
            {
                officeStaffPermissions = context.PermissionRoles.Where(pr => pr.RoleId == officeStaffRole.ID)?.ToList();
            }

            foreach (var employee in context.Employees)
            {
                var permissions = officeStaffPermissions;
                if (employee.Email == "axzesllc@gmail.com")
                {
                    permissions = masterPermissions;
                }

                foreach (var per in permissions)
                {
                    var ep = new EmployeePermission
                    {
                        EmployeeId = employee.ID,
                        PermissionId = per.PermissionId
                    };
                    context.EmployeePermissions.Add(ep);
                }
            }
        }
    }
}
