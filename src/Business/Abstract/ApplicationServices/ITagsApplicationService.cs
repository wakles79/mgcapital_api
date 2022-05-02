// -----------------------------------------------------------------------
// <copyright file="ITagApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Tag;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface ITagsApplicationService : IBaseApplicationService<Tag, int>
    {
        Task<DataSource<TagGridViewModel>> ReadAllDapperAsync(DataSourceRequest request);

        Task<IEnumerable<ListBoxViewModel>> ReadAllCboDapperAsync();

        #region Ticket Tags        
        Task<IEnumerable<TicketTagAssignmentViewModel>> ReadAllTicketTags(int ticketId);
        Task<TicketTag> AddTicketTagAsync(TicketTag vm);
        Task RemoveTicketTagAsync(int ticketTagId);
        #endregion
    }
}
