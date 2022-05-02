// -----------------------------------------------------------------------
// <copyright file="ITicketAttachmentRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface ITicketAttachmentRepository : IBaseRepository<TicketAttachment, int>
    {
        Task<DataSource<TicketAttachmentBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int ticketId);
        Task<TicketAttachment> GetTicketAttachmentByGmailIdAsync(string gmailId);
        Task AddDapperAsync(TicketAttachment ticketAttachment);
    }
}
