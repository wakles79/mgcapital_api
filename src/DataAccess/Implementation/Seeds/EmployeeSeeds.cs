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
    ///     Employees's related objects
    /// </summary>
    public static class EmployeeSeeds
    {

        public static void AddOrUpdate(MGCapDbContext context, int count = 1)
        {
            if (context.Employees.Any())
            {
                return;
            }

            var companiesIds = context.Companies
                          .Select(c => c.ID)
                          .ToList();

            var roleId = context.Roles
                                       ?.OrderBy(r => r.Level)
                                       ?.FirstOrDefault()
                                       ?.ID;

            var contactsList = new List<Contact>
            {
                new Contact
                {
                    CompanyId = companiesIds[0],
                    CompanyName = "Axzes",
                    FirstName = "Axzes",
                    LastName = "LLC",
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow
                },
                new Contact
                {
                    CompanyId = companiesIds[1],
                    CompanyName = "Axzes",
                    FirstName = "John",
                    LastName = "Doe",
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow
                },
                //new Contact
                //{
                //    CompanyId = companiesIds[2],
                //    CompanyName = "Axzes",
                //    FirstName = "Jane",
                //    LastName = "Doe",
                //    CreatedBy = "Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow
                //},
                //new Contact
                //{
                //    CompanyId = companiesIds[3],
                //    CompanyName = "Axzes",
                //    FirstName = "Janie",
                //    LastName = "Doe",
                //    CreatedBy = "Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow
                //},
                new Contact
                {
                    CompanyId = companiesIds[0],
                    CompanyName = "Axzes",
                    FirstName = "Jhonny",
                    LastName = "doe",
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow
                },
                new Contact
                {
                    CompanyId = companiesIds[1],
                    CompanyName = "Axzes",
                    FirstName = "Jimmy",
                    LastName = "Doe",
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow
                },
                //new Contact
                //{
                //    CompanyId = companiesIds[2],
                //    CompanyName = "Axzes",
                //    FirstName = "Janet",
                //    LastName = "Doe",
                //    CreatedBy = "Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow
                //},
                //new Contact
                //{
                //    CompanyId = companiesIds[3],
                //    CompanyName = "Axzes",
                //    FirstName = "Lisa",
                //    LastName = "Doe",
                //    CreatedBy = "Seed",
                //    CreatedDate = DateTime.UtcNow,
                //    UpdatedBy = "Seed",
                //    UpdatedDate = DateTime.UtcNow
                //}
            };

            context.Contacts.AddRange(contactsList);

            var departmentsIds = context.Departments
                .Select(d => new { d.ID, d.CompanyId })
                .ToDictionary(d => d.ID, d => d.CompanyId);

            var employeesList = new List<Employee>();

            var companyId = 0;
            foreach (var item in contactsList.OrderBy(ent => ent.CompanyId))
            {
                string email = "robin@axzes.com";
                if (item.CompanyId != companyId)
                {
                    companyId = item.CompanyId;
                }
                else
                {
                    email = "axzesllc@gmail.com";
                }
                var emp = new Employee
                {
                    ContactId = item.ID,
                    CompanyId = item.CompanyId,
                    Email = email,
                    EmployeeStatusId = 1,
                    DepartmentId = departmentsIds.FirstOrDefault(d => d.Value == item.CompanyId).Key,
                    RoleId = roleId,
                    CreatedBy = "Seed",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "Seed",
                    UpdatedDate = DateTime.UtcNow,
                    Guid = Guid.NewGuid()
                };
                employeesList.Add(emp);
            }

            context.Employees.AddRange(employeesList);
        }
    }
}
