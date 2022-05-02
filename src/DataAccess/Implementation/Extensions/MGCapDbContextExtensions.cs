// -----------------------------------------------------------------------
// <copyright file="MGCapDbContextExtensions.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MGCap.DataAccess.Implementation.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MGCap.DataAccess.Implementation.Extensions
{
    /// <summary>
    ///     Contains functionalities that extends the
    ///     <see cref="MGCapDbContext"/>
    /// </summary>
    public static class MGCapDbContextExtensions
    {
        /// <summary>
        ///     Applies the seeds to the context
        /// </summary>
        /// <param name="context">
        ///     The <see cref="DbContext"/> where to
        ///     apply the seeds
        /// </param>
        /// 
        public static void EnsureSeedData(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<MGCapDbContext>();
            if (context.AllMigrationsApplied())
            {
                // Ordered name list
                var orderedSeedNames = new List<string>
                {
                    "CompanySeeds",
                    "DepartmentsSeds",
                    "CustomerSeeds",
                    "BuildingSeeds",
                    "PropertyManagersSeeds",
                    "ModulePermissionsSeeds",
                    "PermissionsSeeds",
                    "RolesSeeds",
                    "EmployeeSeeds",
                    "PermissionRolesSeeds",
                    "EmployeePermissionsSeeds",
                    "ContractSeeds"
                };

                // All types
                var exportedTypes = new HashSet<Type>(Assembly.Load(new AssemblyName("MGCap.DataAccess.Implementation"))
                                            .GetTypes()
                                            .Where(type => type.Namespace == "MGCap.DataAccess.Implementation.Seeds" && !type.IsNested));

                // Ordered types
                var orderedSeedTypes = new List<Type>();

                // Getting the ordered types first
                foreach (var name in orderedSeedNames)
                {
                    var t = exportedTypes.FirstOrDefault(seedType => seedType.FullName.Contains(name));
                    if (t != null)
                    {
                        exportedTypes.Remove(t);
                        orderedSeedTypes.Add(t);
                    }
                }

                // Add reminders in case they exists
                if (exportedTypes.Count > 0)
                {
                    orderedSeedTypes.AddRange(exportedTypes);
                }

                foreach (var exportedType in orderedSeedTypes)
                {
                    if (!exportedType.FullName.Contains("IdentitySeeds"))
                    {
                        exportedType.GetMethod("AddOrUpdate")
                                .Invoke(null, new List<object>() { context, 10 }
                                .ToArray());
                        context.SaveChanges();
                    }
                }
                context.Dispose();
            }
        }
    }
}
