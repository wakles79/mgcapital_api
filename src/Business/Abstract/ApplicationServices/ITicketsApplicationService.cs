// -----------------------------------------------------------------------
// <copyright file="ITicketsApplicationServicecs.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Freshdesk;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.TicketActivityLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ITicketsApplicationService : IBaseApplicationService<Ticket, int>, IBaseEntityWithAttachmentsService<TicketAttachment, int>
    {
        Task<DataSource<TicketGridViewModel>> ReadAllDapperAsync(DataSourceRequestTicket request);

        Task<TicketDetailsViewModel> GetTicketDetailsDapperAsync(int id = -1, Guid? guid = null);

        Task<DataSource<TicketToMergeGridViewModel>> ReadAllToMergeAsync(int paramType, string value);

        Task MergeTicketsAsync(TicketMergeViewModel mergeViewModel);

        Task ForwardTicket(TicketForwardViewModel vm);

        Task UpdateAssignedTicketEmployee(int ticketId, int employeeId);

        Task<bool> AssignCurrentEmployeeToTicket(int ticketId);

        Task RegisterEmployeeAssignment(int ticketId, int employeeId);

        #region Attachments

        Task<DataSource<TicketAttachmentBaseViewModel>> ReadAllAttachemntsAsync(DataSourceRequest request, int ticketId);
        Task<Ticket> UpdateEntityReferenceAsync(Ticket obj, bool saveActivity = true, bool sequenceConverted = false, string textLog = "");
        Task<int> GetPendingTicketsCountDapperAsync();
        Task<TicketAttachment> AddTicketAttachmentAsync(TicketAttachmentCreateViewModel vm);
        Task<int> SaveChangesAsync(bool updateAuditableFields = true, bool sendNotifications = true);// MG-15
        Task<IEnumerable<TicketAttachment>> CheckAttachmentsAsync(IEnumerable<TicketAttachmentUpdateViewModel> obj, int workOrderId);// MG-15
        #endregion Attachments


        #region Inspection Item
        Task<bool> CloseReferencedInspectionItem(int ticketId);
        Task<Ticket> AddAsync(Ticket obj, bool sendNotifications = true);
        Task<Ticket> AddAsyncExternal(Ticket obj, bool sendNotifications = true);
        #endregion

        #region External
        Task<Ticket> AddExternalAsync(TicketFreshdeskCreateViewModel ticketVm);
        Task GMailAddExternalAsync(GMailRequesterResponseViewModel ticketVm);
        Task<Ticket> RegisterRequesterResponse(FreshdeskRequesterResponseViewModel vm);
        Task GMailRegisterRequesterResponseAsync(int ticketID, ulong historyId);
        Task<int> SaveChangesAsyncExternal();// MG-115
        #endregion

        #region Activity Log
        Task<IEnumerable<TicketActivityLogGridViewModel>> ReadAllTicketAcitivityLogAsync(int ticketId, IEnumerable<int> activityTypes = null);

        Task<TicketActivityLog> ExistWorkOrderSequenceLog(int ticketId, int workOrderScheduleSettingId);

        Task RemoveActivityLog(int id);
        #endregion

        #region Freshdesk
        Task<TicketAttachment> CopyFreshdeskAttachmentToTicket(TicketCopyFreshdeskImageViewModel viewModel);
        #endregion
    }
}
