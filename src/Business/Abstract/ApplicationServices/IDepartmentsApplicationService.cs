// -----------------------------------------------------------------------
// <copyright file="IDepartmentsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
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
    public interface IDepartmentsApplicationService : IBaseApplicationService<Department, int>
    {
        /// <summary>
        ///     Read all the elements in the
        ///     table that <see cref="Department"/> represents
        ///     applying a filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Department>> ReadAllAsync();

        Task<DataSource<DepartmentGridViewModel>> ReadAllDapperAsync(DataSourceRequest request);
    }
}
