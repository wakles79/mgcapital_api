// -----------------------------------------------------------------------
// <copyright file="IWorkOrdersRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderBillingReport;
using System;
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
    public interface IWorkOrdersRepository : IBaseRepository<WorkOrder, int>
    {
        Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int companyId, int? id = null);

        Task<DataSource<WorkOrderGridViewModel>> ReadAllDapperAsync(DataSourceRequestWOReadAll request, int companyId, int? administratorId = null, int? statusId = null, int? buildingId = null, int? typeId = null, bool unscheduled = false);

        Task<DataSource<WOSourcesListBoxViewModel>> ReadAllWOSourceCboDapperAsync(DataSourceRequest request);

        Task<IEnumerable<WorkOrderContactViewModel>> GetWOContactsDapperAsync(WorkOrder wo);

        Task<WorkOrderEmailDetailsViewModel> GetWODetailsDapperAsync(WorkOrder wo, int companyId, string userEmail);

        Task<IEnumerable<int>> GetEmployeesIdsDapperAsync(int workOrderId, WorkOrderEmployeeType type = WorkOrderEmployeeType.Any);
        Task<DataSource<WorkOrderDashboardViewModel>> GetDashboardDataDapperAsync(int companyId, int timezoneOffset = 300, int? employeeId = null);

        Task<WorkOrderUpdateViewModel> GetFullWorkOrderDapperAsync(int workOrderId = -1, Guid? workOrderGuid = null);

        // AT BOTTOM TO AVOID MERGE CONFLICTS
        Task<WorkOrderSource> GetWOSourceDapperAsync(WorkOrderSourceCode code);

        Task<DataSource<WorkOrderDailyReportViewModel>> DailyReportByOperationsManagerDapperAsync(DataSourceRequestWOReadAll request, int companyId, int? operationsManagerId = null);

        Task<IEnumerable<WorkOrderWithExpirationViewModel>> GetWorkOrderWithExpirationDapperAsync(int companyId, int timezoneOffset = 300);

        Task<IEnumerable<WorkOrderEmployeeContactViewModel>> GetSupervisorsAndOperationsManagersDapperAsync(int companyId);
        Task<DataSource<WorkOrderBillingReportGridViewModel>> ReadBillingReportDapperAsync(DataSourceRequestBillingReport request, int companyId);

        Task<DataSource<WorkOrderGridViewModel>> ReadAllAppDapperAsync(DataSourceRequestWOReadAll request,int companyId,int? administratorId = null, int? statusId = null, int? buildingId = null, int? supervisorId = null, int? operationsManagerId = null, int? number = null, int? typeId = null);

        Task<IEnumerable<WorkOrder>> ReadAllByIDs(IEnumerable<int> ids);

        Task<WorkOrder> SingleOrDefaultDapperAsync(int id);

        Building GetBuilding(int? buildingId);

        // CALENDAR
        Task<DataSource<CalendarGridViewModel>> ReadAllSnozeedDapperAsync(DataSourceRequest request, int companyId);

        Task<DataSource<WorkOrderCalendarGridViewModel>> ReadAllCalendarDapperAsync(DataSourceRequestCalendar request, int companyId);

        Task<IEnumerable<WorkOrderTaskSummaryViewModel>> ReadAllWorkOrderSequence(int calendarItemFrequencyId);
        
        // Employees
        Task UnassignEmployeesByWorkOrderIdAsync(int workOrderId);
    }
}
