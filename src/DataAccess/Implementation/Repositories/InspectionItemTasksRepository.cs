// -----------------------------------------------------------------------
// <copyright file="InspectionItemTaksRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.InspectionItemTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionItemTasksRepository : BaseRepository<InspectionItemTask, int>, IInspectionItemTasksRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;

        public InspectionItemTasksRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        public Task<DataSource<InspectionItemTaskBaseViewModel>> ReadAll(DataSourceRequest request, int inspectionItemId)
        {
            throw new NotImplementedException();
        }
    }
}
