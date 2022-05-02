// <copyright file="WorkOrderTaskAttachmentsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderTaskAttachmentsRepository : BaseRepository<WorkOrderTaskAttachment, int>, IWorkOrderTaskAttachmentsRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public WorkOrderTaskAttachmentsRepository(
            IBaseDapperRepository baseDapperRepository,
            MGCapDbContext context
            ): base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }
    }
}
