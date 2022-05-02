// -----------------------------------------------------------------------
// <copyright file="WorkOrderServiceCategoriesApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.WorkOrderService;
using MGCap.Domain.ViewModels.WorkOrderServiceCategory;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class WorkOrderServiceCategoriesApplicationService : BaseSessionApplicationService<WorkOrderServiceCategory, int>, IWorkOrderServiceCategoriesApplicationService
    {
        private new IWorkOrderServiceCategoriesRepository Repository => base.Repository as IWorkOrderServiceCategoriesRepository;
        private readonly IWorkOrderServicesRepository _workOrderServicesRepository;

        public WorkOrderServiceCategoriesApplicationService(
            IWorkOrderServiceCategoriesRepository repository,
            IWorkOrderServicesRepository workOrderServicesRepository,
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor)
        {
            this._workOrderServicesRepository = workOrderServicesRepository;
        }

        public Task<DataSource<WorkOrderServiceCategoryGridViewModel>> ReadAllCategoriesAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId);
        }

        public Task<DataSource<WorkOrderServiceCategoryListViewModel>> ReadAllCboCategoriesAsync()
        {
            return this.Repository.ReadAllCboDapperAsync(this.CompanyId);
        }

        #region Services
        public Task<DataSource<WorkOrderServiceGridViewModel>> ReadAllServicesAsync(DataSourceRequest request, int categoryId = -1)
        {
            return this._workOrderServicesRepository.ReadAllDapperAsync(request, this.CompanyId, categoryId);
        }

        public Task<DataSource<WorkOrderServiceListBoxViewModel>> ReadAllCboServicesAsync(int categoryId = -1, IEnumerable<int> categoryIds = null)
        {
            return this._workOrderServicesRepository.ReadAllCboDapperAsync(this.CompanyId, categoryId, categoryIds);
        }

        public Task<WorkOrderService> GetServiceById(int id)
        {
            return this._workOrderServicesRepository.SingleOrDefaultAsync(s => s.ID == id);
        }

        public Task<WorkOrderService> AddServiceAsync(WorkOrderService obj)
        {
            return this._workOrderServicesRepository.AddAsync(obj);
        }

        public Task<WorkOrderService> UpdateServiceAsync(WorkOrderService obj)
        {
            return this._workOrderServicesRepository.UpdateAsync(obj);
        }

        public Task DeleteServiceAsync(int id)
        {
            return this._workOrderServicesRepository.RemoveAsync(id);
        }
        #endregion
    }
}
