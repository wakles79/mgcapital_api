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
    ///     Customer table
    /// </summary>
    public class CustomerSeeds
    {
        public static List<string> Names = new List<string>
        {
            "Avison Young",
            "American Real Estate Partners Mgt",
            "Blackwell Street Management",
            "Capital Associates",
            "Capridge Partners",
            "Cotton Corporation",
            "Cushman and Wakefield",
            "CB Richard Ellis",
            "Craig Davis Properties",
            "Conduent",
            "Dilweg",
            "Epic Regency",
            "Epic Games",
            "First Citizens Bank",
            "Foundry Commercial",
            "Genesis Commercial Real Estate Services",
            "Griffin Partner's",
            "Grubb Ventures",
            "Highwoods",
            "Longfellow Real Estate Partners",
            "Mason Properties",
            "Q2 Solutions Lab",
            "RUN Property Management",
            "Rhyne management Associate",
            "Stoltz Management",
            "Spectrum Properties",
            "St. David's School",
            "SVN Alliance Asset & Property Management",
            "Trinity Partners",
            "TradeMark Properties",
            "Tri Properties",
            "The Focus Properties, Inc.",
            "The North Carolina State Bar",
            "TriLand Property Commercial",
            "Urben Commercial"
        };
        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifiyng the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapDbContext context, int count = 50)
        {
            if (context.Customers.Any())
            {
                return;
            }

            var companiesIds = context.Companies
                          .Select(c => c.ID)
                          .ToList();
            var customers = new List<Customer>();
            foreach (var companyId in companiesIds)
            {

                for (int i = 0; i < Names.Count; i++)
                {
                    var customer = new Customer
                    {
                        Code = $"{Guid.NewGuid().ToString().Substring(0, 5)}",
                        Name = Names[i],
                        CompanyId = companyId,
                        CreatedBy = "Seed",
                        UpdatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,

                    };
                    customers.Add(customer);
                }
            }
            context.Customers.AddRange(customers);
        }
    }
}
