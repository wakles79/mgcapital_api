// -----------------------------------------------------------------------
// <copyright file="IProposalsApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Proposal;
using MGCap.Domain.ViewModels.ProposalService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IProposalsApplicationService : IBaseApplicationService<Proposal, int>
    {
        Task<DataSource<ProposalGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int? status = -1);
        Task<ProposalReportDetailViewModel> GetProposalReportDetailsDapperAsync(int? proposalId, Guid? guid);
        Task<Proposal> UpdateStatusAsync(int proposalId, int newStatus, string billToName, string billToEmail, int? billTo = -1);
        Task SendProposalReport(ProposalReportDetailViewModel vm, IEnumerable<ProposalAdditionalRecipientViewModel> additionalRecipients, bool commentResponse = false);

        Task<ProposalService> AddProposalServiceAsync(ProposalService proposalService);
        Task<ProposalService> UpdateProposalServiceAsync(ProposalService proposalService);
        Task<ProposalService> GetProposalServiceByIdAsync(int id);
        Task<DataSource<ProposalServiceGridViewModel>> ReadAllProposalServicesDapperAsync(DataSourceRequest request, int proposalId);
    }
}
