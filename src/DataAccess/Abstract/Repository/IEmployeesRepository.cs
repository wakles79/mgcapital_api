// -----------------------------------------------------------------------
// <copyright file="IEmployeesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Account;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.Role;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the functionalities
    ///     for handling operations on the <see cref="Employee"/>
    /// </summary>
    public interface IEmployeesRepository : IBaseRepository<Employee, int>
    {
        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table that <see cref="Employee"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="employeeEmail">The current logged in user Email</param>
        /// <returns>A list with all the Employees with the given Email</returns>
        Task<List<Employee>> ReadWithCompanyAsync(string employeeEmail);

        Task<IEnumerable<EmployeeLoginReponseViewModel>> ReadWithCompanyDapperAsync(string employeeEmail);

        /// <summary>
        ///     Gets all the permissions associated with an Employee,
        ///     such employee is known by a (companyId, email) tuple
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetPermissionsAsync(int companyId, string email);

        Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int companyId, int? id = null, int? employeeId = null, int? roleLevel = null);

        Task<DataSource<EmployeebBuildingListBoxViewModel>> ReadAllByBuildingCboDapperAsync(int buildingId);

        Employee AssignRolePermissions(Employee obj, Func<Role, bool> func = null);

        DataSource<RoleListBoxViewModel> ReadAllCboRoles();

        Task<DataSource<EmployeeGridViewModel>> ReadAllAsyncDapper(DataSourceRequest request, int companyId, int? roleLevel = null, int? roleId = null);

        Task<int> GetEmployeeIdByEmailAndCompanyIdDapperAsync(string email, int companyId);

        int GetEmployeeIdByEmailAndCompanyIdDapper(string email, int companyId);

        Task<DataSource<EmployeeListBoxViewModel>> ReadAllSupervisorsCboDapperAsync(DataSourceRequest request, int companyId, int? buildingId);

        Task<DataSource<EmployeeListBoxViewModel>> ReadAllCboAsyncDapper(DataSourceRequest request, int companyId, ComparisonPredicate comparisonPredicate, int roleLevel, int? id = null);

        Task<EmployeeUpdateViewModel> SingleOrDefaultDapperAsync(int employeeId, int companyId);

        Task<IEnumerable<WorkOrderContactViewModel>> ReadAllOfficeStaffOrMastersDapperAsync(int companyId);

        Task<IEnumerable<EmployeeListBoxViewModel>> ReadAllWorkOrderEmployeesAsync(int workOrderId);

        Task<Employee> GetByEmailAndCompanyAsync(string email, int companyId);
        Task<Role> GetEmployeeRoleAsync(int companyId, string userEmail);
        Task<string> GetEmailSignatureAsync(string userEmail, int companyId);
        Task<string> GetCompanyEmailSignatureAsync(int companyId);
    }
}
