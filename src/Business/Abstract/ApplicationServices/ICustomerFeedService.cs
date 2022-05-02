using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ICustomerFeedService
    {
        Task<DataSource<WOCustomerGridViewModel>> WorkOrdersReadAllAsync(CustomerWODataSourceRequest dataRequest);

        Task<DataSource<ListBoxViewModel>> BuildingsByContactIdAsync(int customerId);

        Task<DataSource<CleaningReportCustomerBaseViewModel>> CleaningReportsReadAllAsync(CustomerCleaningReportDataSourceRequest request);
    }
}
