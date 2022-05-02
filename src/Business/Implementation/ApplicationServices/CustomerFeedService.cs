using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class CustomerFeedService : ICustomerFeedService
    {
        private readonly int CompanyId;
        private readonly string UserEmail;
        private readonly ICustomerFeedRepository CustomerFeedRepository;

        public CustomerFeedService(ICustomerFeedRepository repository, IHttpContextAccessor httpContext)
        {
            CustomerFeedRepository = repository;

            string companyIdStr = httpContext?.HttpContext?.Request?.Headers["CompanyId"];
            this.CompanyId = string.IsNullOrEmpty(companyIdStr) ? 1 : int.Parse(companyIdStr);
            this.UserEmail = httpContext?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";
        }

        public async Task<DataSource<WOCustomerGridViewModel>> WorkOrdersReadAllAsync(CustomerWODataSourceRequest dataRequest)
        {
            var result = await CustomerFeedRepository.WorkOrdersReadAllDapperAsync(dataRequest, CompanyId, UserEmail);
            return result;
        }

        public async Task<DataSource<ListBoxViewModel>> BuildingsByContactIdAsync(int contactId)
        {
            var result = await CustomerFeedRepository.BuildingsByContactIdDapperAsync(contactId, CompanyId, UserEmail);
            return result;
        }

        public async Task<DataSource<CleaningReportCustomerBaseViewModel>> CleaningReportsReadAllAsync(CustomerCleaningReportDataSourceRequest request)
        {
            var result = await CustomerFeedRepository.CleaningReportsReadAllDapperAsync(request, CompanyId, UserEmail);
            return result;
        }
    }
}
