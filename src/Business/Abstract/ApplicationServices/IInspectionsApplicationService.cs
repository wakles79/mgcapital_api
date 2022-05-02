// -----------------------------------------------------------------------
// <copyright file="IInspectionsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Inspection;
using MGCap.Domain.ViewModels.InspectionActivityLog;
using MGCap.Domain.ViewModels.InspectionItem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IInspectionsApplicationService : IBaseApplicationService<Inspection, int>
    {
        Task<DataSource<InspectionGridViewModel>> ReadAllDapperAsync(DataSourceRequestInspection request, int? status = -1, int? buildingId = null, int? employeeId = null);
        Task SendInspectionReportByEmail(InspectionReportDetailViewModel vm, IEnumerable<InspectionAdditionalRecipientViewModel> additionalRecipients, bool commentResponse = false);
        /// <summary>
        /// Send notifications to: Office Staff, Supervisors and Operations Managers
        /// </summary>
        /// <returns></returns>
        Task<bool> SendNotificationToManagersAsync(Inspection inspection);

        Task<InspectionReportDetailViewModel> GetInspectionDetailsDapperAsync(int? proposalId, Guid? guid);
        Task<InspectionItem> AddInspectionItemAsync(InspectionItem inspectionItem);
        Task<InspectionItem> UpdateInspectionItemAsync(InspectionItem inspectionItem);
        Task<InspectionItem> CloseInspectionItemAsync(int id);
        Task<InspectionItem> GetInspectionItemByIdAsync(int id);
        Task<DataSource<InspectionItemGridViewModel>> ReadAllInspectionItemDapperAsync(DataSourceRequest request, int inspectionItemId);

        #region Attachments

        Task<DataSource<InspectionItemAttachmentBaseViewModel>> ReadAllAttachemntsAsync(DataSourceRequest request, int workOrderId);

        Task<InspectionItemAttachment> AddAttachmentAsync(InspectionItemAttachment obj);

        Task<InspectionItemAttachment> UpdateAttachmentsAsync(InspectionItemAttachment obj);

        Task<InspectionItemAttachment> GetAttachmentAsync(Func<InspectionItemAttachment, bool> filter);

        Task RemoveAttachmentsAsync(int objId);

        Task<InspectionItem> SingleOrDefaultItemAsync(Func<InspectionItem, bool> filter);

        Task DeleteItemAsync(InspectionItem itemObj);
        Task<InspectionItemUpdateViewModel> GetInspectionItemUpdateByIdAsync(int id);

        #endregion Attachments

        #region Tasks
        Task RemoveTasksAsync(int objId);

        Task<InspectionItemTask> AddTaskAsync(InspectionItemTask obj);

        Task<InspectionItemTask> UpdateCompletedStatusTaskAsync(int id, bool isCompleted);
        #endregion

        #region Pdf
        Task<string> GetInspectionReportUrl(int? proposalId, Guid? guid);
        #endregion

        #region Log
        Task<DataSource<InspectionActivityLogGridViewModel>> GetAllActivityLogDapperAsync(DataSourceRequest request, int inspectionId);
        #endregion

        #region Ticket
        Task<InspectionItemTicket> AddTicketFromInspectionItemAsync(InspectionItemTicket ticketVm);
        #endregion

        #region Notes
        Task<DataSource<InspectionNoteGridViewModel>> ReadAllNotesDapperAsync(DataSourceRequest request, int inspectionID);

        Task<InspectionNote> GetNoteAsync(Func<InspectionNote, bool> filter);

        Task RemoveNoteAsync(int objId);

        Task<InspectionNote> AddNoteAsync(InspectionNote obj);

        Task<InspectionNote> UpdateNoteAsync(InspectionNote obj);

        Task<DataSource<InspectionNoteGridViewModel>> GetAllgetInspectionNotesDapperAsync(DataSourceRequest request, int inspectionId);

        Task<InspectionItemNote> AddItemNoteAsync(InspectionItemNote obj);

        Task RemoveItemNoteAsync(int objId);
        #endregion
    }
}
