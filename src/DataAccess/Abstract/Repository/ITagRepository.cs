// -----------------------------------------------------------------------
// <copyright file="ITagRepository.cs" company="Axzes">
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

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface ITagRepository : IBaseRepository<Tag, int>
    {
        Task<DataSource<TagGridViewModel>> ReadAllDapperAsync(DataSourceRequest request, int companyId);

        Task<IEnumerable<ListBoxViewModel>> ReadAllCboDapperAsync(int companyId);

        Task<IEnumerable<TicketTagAssignmentViewModel>> ReadAllTicketTags(int ticketId);
    }
}
