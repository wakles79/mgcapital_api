﻿// -----------------------------------------------------------------------
// <copyright file="IWorkOrderTasksRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public interface IWorkOrderTasksRepository : IBaseRepository<WorkOrderTask, int>
    {
        Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int workOrderId, int? id = null);

        Task<DataSource<WorkOrderTaskBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int workOrderId);

        Task<IEnumerable<WorkOrderTaskGridViewModel>> ReadAllDapperAsync(int workOrderId);

        Task<WorkOrderTaskDetailsViewModel> GetAsync(int id);

        Task<IEnumerable<WorkOrderTaskUpdateViewModel>> ReadAllUpdateDapperAsync(int workOrderId);
    }
}
