// -----------------------------------------------------------------------
// <copyright file="EmployeesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    /// <summary>
    /// <para>
    ///     Contains the declaration of the  necessary functionalities
    ///     to handle the operations on the <see cref="Employee"/> entity.
    /// </para>
    /// <remarks>
    ///     This object handle the data of the <see cref="Employee"/> entity
    ///     through the <see cref="IEmployeesRepository"/> but when necessary
    ///     add some operations on the data before pass it to the DataAcces layer
    ///     or to the Presentation layer
    /// </remarks>
    /// </summary>
    public class EmployeesApplicationService : BaseSessionApplicationService<Employee, int>, IEmployeesApplicationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmployeesApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="EmployeesRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>
        public EmployeesApplicationService(
            IEmployeesRepository repository,
            IContactsRepository contactsRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
            this.ContactsRepository = contactsRepository;
        }

        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        public new IEmployeesRepository Repository => base.Repository as IEmployeesRepository;

        private readonly IContactsRepository ContactsRepository;

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table  <see cref="Employee"/> for the current Company
        /// </summary>
        /// <returns>A list with all the Employees  for the current Company</returns>
        public async Task<IEnumerable<Employee>> ReadAllAsync()
        {
            var employees = (await Repository.ReadAllAsync(entity => entity.CompanyId == this.CompanyId))
                                .Include(ent => ent.Contact)
                                    .ThenInclude(c => c.Addresses)
                                .Include(ent => ent.Contact)
                                    .ThenInclude(c => c.Emails)
                                .Include(ent => ent.Contact)
                                    .ThenInclude(c => c.Phones)
                                 .Include(ent => ent.Department)
                                 .Include(ent => ent.Role);
            return employees;
        }

        public async Task<List<Employee>> ReadWithCompanyAsync(string userEmail)
        {
            return await Repository.ReadWithCompanyAsync(userEmail);
        }

        public async Task<IEnumerable<EmployeeLoginReponseViewModel>> ReadWithCompanyDapperAsync(string employeeEmail)
        {
            var result = await Repository.ReadWithCompanyDapperAsync(employeeEmail);
            return result;
        }

        public bool Exists(string email)
        {
            var exists = Exists(ent => ent.Email == email);
            return exists;
        }

        public Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int? id = null, int? employeeId = null, int? roleLevel = null)
        {
            return Repository.ReadAllCboAsyncDapper(request, CompanyId, id, employeeId, roleLevel);
        }

        public Task<DataSource<EmployeebBuildingListBoxViewModel>> ReadAllByBuildingCboDapperAsync(int buildingId)
        {
            return this.Repository.ReadAllByBuildingCboDapperAsync(buildingId);
        }

        public Task<IEnumerable<string>> GetPermissionsAsync()
        {
            return this.Repository.GetPermissionsAsync(this.CompanyId, this.UserEmail);
        }

        // <summary>
        ///     Assign a new role with corresponding permissions to an Employee. </summary>

        /// <param name="obj">Employee to assign new role</param>
        /// <param name="func">Func with the new role</param>
        /// <returns>Return the updated employee</returns>
        public Employee AssignRolePermissions(Employee obj, Func<Role, bool> func = null)
        {
            return this.Repository.AssignRolePermissions(obj, func);
        }

        public DataSource<RoleListBoxViewModel> ReadAllCboRoles()
        {
            return this.Repository.ReadAllCboRoles();
        }

        public Task<DataSource<EmployeeGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int? roleLevel = null, int? roleId = null)
        {
            return Repository.ReadAllAsyncDapper(request, CompanyId, roleLevel, roleId);
        }

        public Task<DataSource<EmployeeListBoxViewModel>> ReadAllSupervisorsCboDapperAsync(DataSourceRequest request, int? buildingId)
        {
            return this.Repository.ReadAllSupervisorsCboDapperAsync(request, this.CompanyId, buildingId);
        }

        public Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, ComparisonPredicate comparisonPredicate, int roleLevel, int? id = null)
        {
            return this.Repository.ReadAllCboAsyncDapper(request, CompanyId, comparisonPredicate, roleLevel, id);
        }

        public Task<Contact> GetContact(int id)
        {
            return this.ContactsRepository.SingleOrDefaultAsync(c => c.ID == id);
        }

        public Task<Role> GetEmployeeRoleAsync(int companyId, string userEmail) =>
            this.Repository.GetEmployeeRoleAsync(companyId, userEmail);

        public Task<string> GetEmailSignatureAsync() => this.Repository.GetEmailSignatureAsync(this.UserEmail, this.CompanyId);

        public Task<string> GetCompanyEmailSignatureAsync() => this.Repository.GetCompanyEmailSignatureAsync(this.CompanyId);
    }
}
