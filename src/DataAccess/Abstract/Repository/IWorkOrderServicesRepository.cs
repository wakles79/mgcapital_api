// -----------------------------------------------------------------------
// <copyright file="IWorkOrderServicesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.WorkOrderService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IWorkOrderServicesRepository : IBaseRepository<WorkOrderService, int>
    {
        Task<DataSource<WorkOrderServiceGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId, int categoryId = -1);

        Task<DataSource<WorkOrderServiceListBoxViewModel>> ReadAllCboDapperAsync(int companyId, int categoryId = -1, IEnumerable<int> categoryIds = null);
    }
}
