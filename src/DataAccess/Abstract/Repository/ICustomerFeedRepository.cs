using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface ICustomerFeedRepository
    {
        Task<DataSource<WOCustomerGridViewModel>> WorkOrdersReadAllDapperAsync(CustomerWODataSourceRequest dataRequest, int companyId, string userEmail);

        Task<DataSource<ListBoxViewModel>> BuildingsByContactIdDapperAsync(int customerId, int companyId, string userEmail);

        Task<DataSource<CleaningReportCustomerBaseViewModel>> CleaningReportsReadAllDapperAsync(CustomerCleaningReportDataSourceRequest request, int companyId, string userEmail);
    }
}
