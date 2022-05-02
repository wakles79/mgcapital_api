// -----------------------------------------------------------------------
// <copyright file="IWorkOrdersApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderBillingReport;
using MGCap.Domain.ViewModels.WorkOrderTask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IWorkOrdersApplicationService : IBaseApplicationService<WorkOrder, int>
    {
        /// <summary>
        ///     Checks if logged employee is accessing a given work order
        ///     and if such work order is in "stand by" status
        ///     then it must be changed to "active"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<bool> CheckAssignedEmployeeAsync(WorkOrder obj);


        /// <summary>
        ///     Overload that receives only workOrderId and workOrderStatusId
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="workOrderStatusId"></param>
        /// <returns></returns>
        Task<bool> CheckAssignedEmployeeAsync(int workOrderId, WorkOrderStatus workOrderStatusId);

        /// <summary>
        ///     Reads all elements in a list-like form
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null);

        Task<DataSource<WorkOrderGridViewModel>> ReadAllDapperAsync(DataSourceRequestWOReadAll request, int? administratorId = null, int? statusId = null, int? buildingId = null, int? typeId = null, bool unscheduled = false);

        Task<DataSource<WOSourcesListBoxViewModel>> ReadAllWOSourceCboDapperAsync(DataSourceRequest request);

        Task<WorkOrderSource> GetWOSourceDapperAsync(WorkOrderSourceCode code);

        Task<DataSource<WorkOrderDashboardViewModel>> GetDashboardDataDapperAsync(int? employeeId = null);

        Task<WorkOrderUpdateViewModel> GetFullWorkOrderDapperAsync(int workOrderId = -1, Guid? workOrderGuid = null);

        #region Employees

        Task AssignEmployee(int workOrderId, int employeeId, int? type = 1);

        Task UnassignEmployee(int workOrderId, int employeeId);

        Task<IEnumerable<int>> GetEmployeesIdsAsync(int workOrderId, WorkOrderEmployeeType type);

        #endregion

        #region Notes

        /// <summary>
        ///     Reads all notes in a list-like form
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataSource<WorkOrderNoteBaseViewModel>> ReadAllNotesDapperAsync(DataSourceRequest request, int workOrderId);

        /// <summary>
        ///     Adds an object to the table <see cref="WorkOrderNote"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<WorkOrderNote> AddNoteAsync(WorkOrderNote obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<WorkOrderNote> UpdateNoteAsync(WorkOrderNote obj);

        Task<WorkOrderNote> GetNoteAsync(Func<WorkOrderNote, bool> filter);

        /// <summary>
        ///     Removes a given obj by its ID
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task RemoveNoteAsync(int objId);

        #endregion

        #region Tasks
        /// <summary>
        ///     Reads all tasks in a list-like form
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataSource<WorkOrderTaskBaseViewModel>> ReadAllTasksDapperAsync(DataSourceRequest request, int workOrderId);

        /// <summary>
        ///     Adds an object to the table <see cref="WorkOrderTask"/>
        /// </summary>
        /// <param name="obj">The object to be added</param>
        /// <returns>Returns the <paramref name="obj"/> after being inserted</returns>
        Task<WorkOrderTask> AddTaskAsync(WorkOrderTask obj);

        /// <summary>
        ///     Begins tracking the given param
        /// </summary>
        /// <param name="obj">The object to be marked</param>
        /// <returns>The given <paramref name="obj"/> after being inserted</returns>
        Task<WorkOrderTask> UpdateTaskAsync(WorkOrderTask obj);

        Task<WorkOrderTask> GetTaskAsync(Func<WorkOrderTask, bool> filter);

        /// <summary>
        ///     Removes a given obj by its ID
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task RemoveTaskAsync(int objId);

        #endregion

        #region Attachments

        Task<DataSource<WorkOrderAttachmentBaseViewModel>> ReadAllAttachemntsAsync(DataSourceRequest request, int workOrderId);

        Task<WorkOrderAttachment> AddAttachmentAsync(WorkOrderAttachment obj);

        Task<WorkOrderAttachment> UpdateAttachmentsAsync(WorkOrderAttachment obj);

        Task<WorkOrderAttachment> GetAttachmentAsync(Func<WorkOrderAttachment, bool> filter);

        Task RemoveAttachmentsAsync(int objId);

        Task<IEnumerable<WorkOrderAttachment>> CheckAttachmentsAsync(IEnumerable<WorkOrderAttachmentUpdateViewModel> obj, int workOrderId);

        #endregion Attachments

        Task<DataSource<WorkOrderDailyReportViewModel>> DailyReportByOperationsManagerDapperAsync(DataSourceRequestWOReadAll request, int? operationsManagerId = null);
        Task<DataSource<WorkOrderBillingReportGridViewModel>> ReadBillingReportDapperAsync(DataSourceRequestBillingReport request);

        Task<DataSource<WorkOrderGridViewModel>> ReadAllAppDapperAsync(DataSourceRequestWOReadAll request, int? administratorId = null, int? statusId = null, int? buildingId = null, int? supervisorId = null, int? operationsManagerId = null, int? number = null, int? typeId = null);

        Task<string> GetBillingDocumentReportUrl(IEnumerable<WorkOrderBillingReportGridViewModel> data);
        Task<WorkOrder> SingleOrDefaultDapperAsync(int iD);
        void MarkWorkOrderAsChanged(WorkOrder obj);
        Building GetBuilding(int buildingId);

        #region Inspection
        Task<bool> ValidateExistingInspectionReferenced(int workOrderId);
        Task<int> SaveChangesAsync(bool updateAuditableFields = true, bool sendNotifications = true);
        #endregion

        #region Calendar
        Task<WorkOrder> UpdateWorkOrderTaskSummary(WorkOrderTaskSummaryUpdateViewModel wo);
        #endregion

        #region Work Order Schedule Setting
        Task<WorkOrderScheduleSetting> AddScheduleSettings(WorkOrderScheduleSetting scheduleSetting);

        Task<IEnumerable<string>> ReadAllWorkOrderNumberFromSequence(int workOrderScheduleSettingId);
        #endregion

        #region Employees
        Task UnassignEmployeesByWorkOrderIdAsync(int workOrderId);
        #endregion

        #region Tasks
        Task<IEnumerable<WorkOrderTaskGridViewModel>> ReadAllTasksAsync(int workOrderId);

        Task<IEnumerable<WorkOrderTaskUpdateViewModel>> ReadAllUpdateTasksAsync(int workOrderId);

        Task<WorkOrderTaskDetailsViewModel> GetWorkOrderTaskAsync(int id);
        #endregion

        #region Notification
        Task SendNotificationsAsync();
        #endregion
    }
}
