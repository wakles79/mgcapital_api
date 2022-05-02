// -----------------------------------------------------------------------
// <copyright file="IDepartmentsRepository.cs" company="Axzes">
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

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the functionalities
    ///     for handling operations on the <see cref="Department"/>
    /// </summary>
    public interface IDepartmentsRepository : IBaseRepository<Department, int>
    {
        Task<DataSource<DepartmentGridViewModel>> ReadAllDapperAsync(int companyId, DataSourceRequest request);
    }
}
