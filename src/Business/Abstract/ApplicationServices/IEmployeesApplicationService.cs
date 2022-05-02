// -----------------------------------------------------------------------
// <copyright file="IEmployeesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Role;
using MGCap.Domain.ViewModels.Employee;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.Enums;

namespace MGCap.Business.Abstract.ApplicationServices
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
    public interface IEmployeesApplicationService : IBaseApplicationService<Employee, int>
    {
        /// <summary>
        ///     Gets the <value>companyId</value> of the current User
        /// </summary>
        int CompanyId { get; }

        /// <summary>
        ///     Gets all the permissions associated with an Employee,
        ///     such employee is known by a (companyId, email) tuple
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetPermissionsAsync();

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="Employee"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="userEmail">Current logged user's email</param>
        /// <returns>A list with all the Employees with the given Email</returns>
        Task<List<Employee>> ReadWithCompanyAsync(string userEmail);

        Task<IEnumerable<EmployeeLoginReponseViewModel>> ReadWithCompanyDapperAsync(string employeeEmail);
        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Employee"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Employee>> ReadAllAsync();

        bool Exists(string email);

        Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int? id = null, int? employeeId = null, int? roleLevel = null);

        Task<DataSource<EmployeebBuildingListBoxViewModel>> ReadAllByBuildingCboDapperAsync(int buildingId);

        /// <summary>
        ///     Assing a new role to an Employee.     
        /// </summary>
        /// <param name="filter">Object employee and a func with the new role</param>
        /// <returns>Return the updated employee</returns>
        Employee AssignRolePermissions(Employee obj, Func<Role, bool> func = null);

        DataSource<RoleListBoxViewModel> ReadAllCboRoles();

        Task<DataSource<EmployeeGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int? roleLevel = null, int? roleId = null);

        Task<DataSource<EmployeeListBoxViewModel>> ReadAllSupervisorsCboDapperAsync(DataSourceRequest request, int? buildingId);

        Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, ComparisonPredicate comparisonPredicate, int roleLevel, int? id = null);

        Task<Contact> GetContact(int id);
        Task<Role> GetEmployeeRoleAsync(int companyId, string userEmail);
        Task<string> GetEmailSignatureAsync();
        Task<string> GetCompanyEmailSignatureAsync();
    }
}
