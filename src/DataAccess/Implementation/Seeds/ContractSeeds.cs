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
    ///     Contract's table
    /// </summary>
    public static class ContractSeeds
    {
        public static void AddOrUpdate(MGCapDbContext context, int count = 50)
        {

            // HACK: This is temporary since we added all creation in building seeds
            return;
            if (context.Contracts.Any())
            {
                return;
            }           

            //Get all companies
            var companiesId = context.Companies.Select(c => c.ID).ToList();            

            //For each company, get all buildings and customers
            foreach ( var companyId in companiesId) {

                var customers = context.Customers
                                      .Where(c => c.CompanyId == companyId)
                                      ?.ToList();

                var buildings = context.Buildings
                                       .Where(b => b.CompanyId == companyId)
                                       ?.ToList();

                // For each building create a contract with status'Active'
                foreach (var building in buildings) {

                    // For random values (ContractNumber, Area, NumberOfPeople)
                    var rand = new Random();
                         
                    // For random customer
                    Random rnd = new Random();
                    // The same random selected customer will be used for create the contact signer and the contract
                    var customerId = customers[rnd.Next(customers.Count)].ID;

                    // Create a contact for 'ContactSigner'
                    var contact = new Contact
                    {
                        CompanyId = companyId,
                        CompanyName = "Axzes",
                        FirstName = "Axzes",
                        LastName = "LLC",
                        CreatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "Seed",
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.Contacts.Add(contact);

                    // Associate the created contact with the random selected customer
                    var contactSigner = new CustomerContact
                    {
                        CustomerId = customerId,
                        ContactId = contact.ID,
                        Type =  "CEO",
                        Default = true,
                        SelectedForMarketing = true,
                    };
                    context.CustomerContacts.Add(contactSigner);

                    // Create the contract
                    var contract = new Contract
                    {
                        ContractNumber = rand.Next(100, 500).ToString(),
                        Area = rand.Next(1000, 2000),
                        NumberOfPeople = rand.Next(5, 20),
                        BuildingId = building.ID,
                        CustomerId = customerId,
                        ContactSignerId = contact.ID,
                        Status = 1,
                        CompanyId = companyId,
                        CreatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "Seed",
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.Contracts.Add(contract);
                }
            }

        }
    }
}

