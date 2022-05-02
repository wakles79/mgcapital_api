// -----------------------------------------------------------------------
// <copyright file="TicketTagRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class TicketTagRepository : BaseRepository<TicketTag, int>, ITicketTagRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public TicketTagRepository(
            MGCapDbContext context,
            IBaseDapperRepository baseDapperRepository
            ) : base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }
    }
}
