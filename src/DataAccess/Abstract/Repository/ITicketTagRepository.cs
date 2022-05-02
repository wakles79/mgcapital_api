// -----------------------------------------------------------------------
// <copyright file="ITicketTagRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface ITicketTagRepository : IBaseRepository<TicketTag, int>
    {
    }
}
