// -----------------------------------------------------------------------
// <copyright file="WorkOrderNotesRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using Microsoft.EntityFrameworkCore;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGCap.Domain.ViewModels.WorkOrder;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="WorkOrderNote"/>
    /// </summary>
    public class WorkOrderNotesRepository : BaseRepository<WorkOrderNote, int>, IWorkOrderNotesRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkOrderNotesRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public WorkOrderNotesRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<ListBoxViewModel>> ReadAllCboDapperAsync(DataSourceRequest request, int workOrderId, int? id = null)
        {
            var result = new DataSource<ListBoxViewModel>
            {
                Payload = new List<ListBoxViewModel>(),
                Count = 0
            };

            //TODO: define this query
            string query = @"";

            var pars = new DynamicParameters();
            pars.Add("@id", id);
            pars.Add("@workOrderId", workOrderId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<ListBoxViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<DataSource<WorkOrderNoteBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int workOrderId)
        {
            var result = new DataSource<WorkOrderNoteBaseViewModel>
            {
                Payload = new List<WorkOrderNoteBaseViewModel>(),
                Count = 0
            };

            string query = $@"
                    SELECT 
                           wo.[ID]
                          ,wo.[CreatedDate]
                          ,wo.[UpdatedDate]
                          ,wo.[WorkOrderId]
                          ,wo.[Note]
                          ,emp.[Email] as EmployeeEmail
                          ,emp.[ID] as EmployeeId
                    FROM [dbo].[WorkOrderNotes] as wo
                        INNER JOIN [dbo].[Employees] as emp ON wo.EmployeeId = emp.ID
                    WHERE [WorkOrderId] = @workOrderId";

            var pars = new DynamicParameters();
            pars.Add("@workOrderId", workOrderId);

            var payload = await _baseDapperRepository.QueryAsync<WorkOrderNoteBaseViewModel>(query, pars);
            
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
