// -----------------------------------------------------------------------
// <copyright file="IWorkOrderServiceCategoriesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.WorkOrderService;
using MGCap.Domain.ViewModels.WorkOrderServiceCategory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IWorkOrderServiceCategoriesApplicationService : IBaseApplicationService<WorkOrderServiceCategory, int>
    {
        Task<DataSource<WorkOrderServiceCategoryGridViewModel>> ReadAllCategoriesAsync(DataSourceRequest request);

        Task<DataSource<WorkOrderServiceCategoryListViewModel>> ReadAllCboCategoriesAsync();

        #region Services
        Task<DataSource<WorkOrderServiceGridViewModel>> ReadAllServicesAsync(DataSourceRequest request, int categoryId = -1);

        Task<DataSource<WorkOrderServiceListBoxViewModel>> ReadAllCboServicesAsync(int categoryId = -1, IEnumerable<int> categoryIds = null);

        Task<WorkOrderService> GetServiceById(int id);

        Task<WorkOrderService> AddServiceAsync(WorkOrderService obj);

        Task<WorkOrderService> UpdateServiceAsync(WorkOrderService obj);

        Task DeleteServiceAsync(int id);
        #endregion
    }
}
