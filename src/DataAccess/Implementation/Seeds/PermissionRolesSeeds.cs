using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class PermissionRolesSeeds
    {
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.PermissionRoles.Any())
            {
                return;
            }

            var permissions = context.Permissions.ToList();
            var masterRole = context.Roles.FirstOrDefault(r => r.Name == "Master");
            var officeStaffRole = context.Roles.FirstOrDefault(r => r.Name == "Office Staff");
            var operationManagerRole = context.Roles.FirstOrDefault(r => r.Name == "Operation Manager");
            var supervisorRole = context.Roles.FirstOrDefault(r => r.Name == "Supervisor");

            // Adding all permissions to master template
            if (masterRole != null)
            {
                foreach (var per in permissions)
                {
                    var pr = new PermissionRole
                    {
                        PermissionId = per.ID,
                        RoleId = masterRole.ID
                    };
                    context.PermissionRoles.Add(pr);
                }
            }

            // Adding dashboard permission to all roles 
            var roles = context.Roles.Where(r => r.Level > 10);
            var dashboardPermission = context.Permissions.Where(p => p.Module == Domain.Enums.ApplicationModule.Dashboard && p.Type == Domain.Enums.ActionType.Read);
            foreach (var role in roles)
            {
                foreach (var permission in dashboardPermission)
                {
                    var pr = new PermissionRole
                    {
                        PermissionId = permission.ID,
                        RoleId = role.ID
                    };
                    context.PermissionRoles.Add(pr);
                }
            }


            // Adding all except management permissions to staff
            //if (officeStaffRole != null)
            //{
            //    var filteredPer = permissions
            //                    .Where(p => !((new List<string> { "Users", "Permissions", "Proposals", "Analytics" }).Contains(p.Name)));
            //    foreach (var per in filteredPer)
            //    {
            //        var pr = new PermissionRole
            //        {
            //            PermissionId = per.ID,
            //            RoleId = officeStaffRole.ID
            //        };
            //        context.PermissionRoles.Add(pr);
            //    }
            //}


            // Getting only basic permissions
            //var basicPer = permissions.Where(p => p.Name == "WorkOrders" || p.Name == "Inspections" || p.Name == "ReadBuildings");
            //var basicRoles = (new List<Role> { operationManagerRole, supervisorRole }).Where(obj => obj != null);
            //foreach (var per in basicPer)
            //{
            //    foreach (var role in basicRoles)
            //    {
            //        var pr = new PermissionRole
            //        {
            //            PermissionId = per.ID,
            //            RoleId = role.ID
            //        };
            //        context.PermissionRoles.Add(pr);
            //    }
            //}
        }
    }
}
