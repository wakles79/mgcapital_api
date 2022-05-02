// -----------------------------------------------------------------------
// <copyright file="ICleaningReportsApplicationServicecs.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.CleaningReport;
using MGCap.Domain.ViewModels.CleaningReportActivityLog;
using MGCap.Domain.ViewModels.CleaningReportItem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ICleaningReportsApplicationService : IBaseApplicationService<CleaningReport, int>
    {
        Task<DataSourceCleaningReport> ReadAllDapperAsync(DataSourceRequest request, int? contactId = null, int? employeeId = null, int? statusId = null, int? commentDirection = null);

        Task<DataSource<CleaningReportListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int? id = null);

        Task<CleaningReportItem> AddCleaningReportItemAsync(CleaningReportItem obj);

        Task<IEnumerable<CleaningReportItemGridViewModel>> GetCleaningReportItemsDapper(int cleaningReportId, int? type = null);

        Task<CleaningReportDetailsViewModel> GetCleaningReportDetailsDapperAsync(int cleaningReportId = -1, Guid? guid = null);

        Task<CleaningReportItem> SingleOrDefaultItemAsync(Func<CleaningReportItem, bool> filter);

        Task<CleaningReportItem> UpdateItemAsync(CleaningReportItem obj);

        Task DeleteItemAsync(CleaningReportItem obj);

        Task<CleaningReportItemAttachment> GetAttachmentAsync(Func<CleaningReportItemAttachment, bool> filter);

        Task RemoveAttachmentsAsync(int objId);

        Task<CleaningReportItemAttachment> AddAttachmentAsync(CleaningReportItemAttachment obj);

        Task<CleaningReportItemUpdateViewModel> GetCleaningReportItemAsync(int id);

        Task SendCleaningReport(CleaningReportDetailsViewModel reportVM, IEnumerable<CleaningReportAdditionalRecipientViewModel> AdditionalRecipients, bool commentResponse = false);

        Task<CleaningReportNote> AddCleaningReportNoteAsync(CleaningReportNote obj);

        Task<DataSource<CleaningReportActivityLogGridViewModel>> GetAllActivityLogDapperAsync(DataSourceRequest request, int cleaningReportId);

        Task<string> GetReportPDFBase64(int cleaningReportId);
    }
}
