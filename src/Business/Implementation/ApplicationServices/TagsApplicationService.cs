// -----------------------------------------------------------------------
// <copyright file="TagApplicationService.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Tag;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class TagsApplicationService : BaseSessionApplicationService<Tag, int>, ITagsApplicationService
    {
        private new ITagRepository Repository => base.Repository as ITagRepository;
        private readonly ITicketTagRepository _ticketTagRepository;

        public TagsApplicationService(
            ITagRepository repository,
            IHttpContextAccessor httpContextAccessor,
            ITicketTagRepository ticketTagRepository
            ) : base(repository, httpContextAccessor)
        {
            this._ticketTagRepository = ticketTagRepository;
        }

        public Task<DataSource<TagGridViewModel>> ReadAllDapperAsync(DataSourceRequest request)
        {
            return this.Repository.ReadAllDapperAsync(request, this.CompanyId);
        }

        public Task<IEnumerable<ListBoxViewModel>> ReadAllCboDapperAsync()
        {
            return this.Repository.ReadAllCboDapperAsync(this.CompanyId);
        }

        #region Ticket Tags

        public Task<IEnumerable<TicketTagAssignmentViewModel>> ReadAllTicketTags(int ticketId)
        {
            return this.Repository.ReadAllTicketTags(ticketId);
        }

        public async Task<TicketTag> AddTicketTagAsync(TicketTag ticketTag)
        {
            var assignment = await this._ticketTagRepository.SingleOrDefaultAsync(t => t.TicketId == ticketTag.TicketId && t.TagId == ticketTag.TagId);
            if (assignment == null)
            {
                return await this._ticketTagRepository.AddAsync(ticketTag);
            }
            return null;
        }

        public Task RemoveTicketTagAsync(int ticketTagId)
        {
            return this._ticketTagRepository.RemoveAsync(ticketTagId);
        }
        #endregion
    }
}
