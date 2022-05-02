// -----------------------------------------------------------------------
// <copyright file="EmployeesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Http;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.Department;
using MGCap.Domain.Utils;

namespace MGCap.Business.Implementation.ApplicationServices
{
    /// <summary>
    /// <para>
    ///     Contains the declaration of the  necessary functionalities
    ///     to handle the operations on the <see cref="Department"/> entity.
    /// </para>
    /// <remarks>
    ///     This object handle the data of the <see cref="Department"/> entity
    ///     through the <see cref="IDepartmentsRepository"/> but when necessary
    ///     add some operations on the data before pass it to the DataAcces layer
    ///     or to the Presentation layer
    /// </remarks>
    /// </summary>
    public class DepartmentsApplicationService : BaseSessionApplicationService<Department, int>, IDepartmentsApplicationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmployeesApplicationService"/> class.
        /// </summary>
        /// <param name="repository">
        ///     To inject the implementation of <see cref="DepartmentsRepository"/>.
        ///     Through this object, the current layer access the DataAccess layer.
        /// </param>
        /// <param name="httpContextAccessor">
        ///     To access data in the current <see cref="HttpContext"/>
        /// </param>
        /// <param name="userResolverService">For getting some data from the current User</param>
        public DepartmentsApplicationService(
            IDepartmentsRepository repository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        /// <summary>
        ///     Gets the object that contains the operations of
        ///     the DataAcces layer
        /// </summary>
        public new IDepartmentsRepository Repository => base.Repository as IDepartmentsRepository;

        /// <summary>
        ///     Asynchronously read all the elements in the
        ///     table  <see cref="Department"/> for the current Company
        /// </summary>
        /// <returns>A list with all the Departments  for the current Company</returns>
        public async Task<IEnumerable<Department>> ReadAllAsync()
        {
            var departmentList = (await Repository.ReadAllAsync(entity => entity.CompanyId == this.CompanyId));
            return departmentList;
        }

        public Task<DataSource<DepartmentGridViewModel>> ReadAllDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllDapperAsync(this.CompanyId, request);
        }

    }
}
