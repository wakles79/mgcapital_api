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
    ///     WorkOrderSource's table
    /// </summary>
    public static class WorkOrderSourceSeeds
    {
        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifiyng the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.WorkOrderSources.Any())
            {
                return;
            }

            var wOSources = new List<WorkOrderSource>
            {   
                new WorkOrderSource 
                {
                    Name = "Email",
                    Code = 0,
                },
                new WorkOrderSource
                {
                    Name = "Phone",
                    Code = 1,
                },
                new WorkOrderSource
                {
                    Name = "In Person",
                    Code = 2,
                },
                new WorkOrderSource
                {
                    Name = "Recurring",
                    Code = 3,
                },
                new WorkOrderSource
                {
                    Name = "Internal Mobile Application",
                    Code = 10,
                },
                new WorkOrderSource
                {
                    Name = "External Mobile Application",
                    Code = 11,
                },
                new WorkOrderSource
                {
                    Name = "Landing Page",
                    Code = 12,
                },                
            };

            context.WorkOrderSources.AddRange(wOSources);
        }


    }
}
