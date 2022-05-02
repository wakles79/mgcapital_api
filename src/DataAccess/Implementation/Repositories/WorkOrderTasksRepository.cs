// -----------------------------------------------------------------------
// <copyright file="WorkOrderTasksRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrder;
using MGCap.Domain.ViewModels.WorkOrderTask;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="WorkOrderTask"/>
    /// </summary>
    public class WorkOrderTasksRepository : BaseRepository<WorkOrderTask, int>, IWorkOrderTasksRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkOrderTasksRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public WorkOrderTasksRepository(
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

        public async Task<DataSource<WorkOrderTaskBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int workOrderId)
        {
            var result = new DataSource<WorkOrderTaskBaseViewModel>
            {
                Payload = new List<WorkOrderTaskBaseViewModel>(),
                Count = 0
            };

            string query = $@"
                SELECT 
                       [ID]
                      ,[CreatedDate]
                      ,[Description]
                      ,[IsComplete]
                      ,[UpdatedDate]
                      ,[WorkOrderId]
                      ,[Note]
                  FROM [dbo].[WorkOrderTasks]
                  WHERE [WorkOrderId] = @workOrderId;";

            var pars = new DynamicParameters();
            pars.Add("@workOrderId", workOrderId);
            pars.Add("@filter", request.Filter);
            pars.Add("@pageNumber", request.PageNumber);
            pars.Add("@pageSize", request.PageSize);

            var payload = await _baseDapperRepository.QueryAsync<WorkOrderTaskBaseViewModel>(query, pars);
            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }

        public async Task<IEnumerable<WorkOrderTaskGridViewModel>> ReadAllDapperAsync(int workOrderId)
        {
            string query = $@"
                SELECT 
	                WT.[ID],
	                WT.[Location],
	                WT.[Description],
                    WT.[ServiceId],
                    WT.[WorkOrderServiceId],
	                ISNULL(WT.[Quantity], 0) As [Quantity],
	                CASE 
		                WHEN WT.WorkOrderServiceId IS NOT NULL THEN WT.[Rate] 
		                ELSE WT.[UnitPrice] 
	                END AS [Rate],
	                CASE 
		                WHEN WT.WorkOrderServiceId IS NOT NULL THEN WS.[Name]
		                WHEN WT.ServiceId IS NOT NULL THEN S.[Name]  
		                ELSE ''
	                END AS [ServiceName],
	                ISNULL(C.[Name],'') AS [CategoryName],
                    WT.[IsComplete],
	                WT.[UnitFactor],
                    CASE
                        WHEN S.[ID] IS NOT NULL AND WS.ID IS NULL THEN 1
                        ELSE 0
                    END AS [OldVersion],
                    ISNULL(WS.[RequiresScheduling],0) AS [RequiresScheduling],
                    WT.[QuantityRequiredAtClose],
                    ISNULL(WS.[HoursRequiredAtClose],0) AS [HoursRequiredAtClose],
                    WT.[QuantityExecuted],
                    WT.[HoursExecuted],
                    WT.[Frequency],
                    WT.[CompletedDate]
                FROM [WorkOrderTasks] AS WT 
	                LEFT JOIN [Services] AS S ON S.ID = WT.ServiceId
	                LEFT JOIN [WorkOrderServiceCategories] AS C ON C.[ID] = WT.[WorkOrderServiceCategoryId]
	                LEFT JOIN [WorkOrderServices] AS WS ON WS.[ID] = WT.[WorkOrderServiceId]
                WHERE WorkOrderId = @workOrderId
                ORDER BY 
                CASE 
                    WHEN WT.[Frequency] = {(int)WorkOrderServiceFrequency.Monthly}  THEN 1 ELSE 2
                END,
                ID DESC";

            var pars = new DynamicParameters();
            pars.Add("@workOrderId", workOrderId);

            var rows = await _baseDapperRepository.QueryAsync<WorkOrderTaskGridViewModel>(query, pars);
            return rows.AsEnumerable();
        }

        public override async Task<WorkOrderTask> SingleOrDefaultAsync(Func<WorkOrderTask, bool> filter)
        {
            var result = await this.Entities
                                    .Include(t => t.Service)
                                    .SingleOrDefaultAsync(t => filter.Invoke(t));

            return result;
        }

        public async Task<WorkOrderTaskDetailsViewModel> GetAsync(int id)
        {
            string query = $@"
                SELECT 
                    WT.[ID],
                    WT.[WorkOrderId],
                    WT.[IsComplete],
                    WT.[Description],
                    WT.[ServiceId],
                    WT.[UnitPrice],
                    WT.[Quantity],
                    WT.[DiscountPercentage],
                    WT.[Note],
                    WT.[GeneralNote],
                    WT.[LastCheckedDate],
                    WT.[Location],
                    WT.[WorkOrderServiceCategoryId],
                    WT.[WorkOrderServiceId],
                    WT.[UnitFactor],
                    WT.[Frequency],
                    WT.[Rate],
                    WT.[CreatedDate],
                    WT.[LastCheckedDate],
                    WT.[QuantityExecuted],
                    WT.[HoursExecuted],
                    WT.[CompletedDate],
                    WT.[QuantityRequiredAtClose],
	                CASE 
		                WHEN WT.ServiceId IS NOT NULL THEN S.[Name] 
		                WHEN WT.WorkOrderServiceId IS NOT NULL THEN WS.[Name] 
		                ELSE ''
	                END AS [ServiceName],
                    S.[UnitFactor],
                    ISNULL(A.[ID], -1) AS [ID],
                    A.[BlobName],
                    A.[Description],
                    A.[BlobName],
                    A.[FullUrl],
                    A.[Title],
                    A.[ImageTakenDate]
                FROM [WorkOrderTasks] AS WT
                    LEFT JOIN [Services] AS S ON S.[ID] = WT.[ServiceId]
	                LEFT JOIN [WorkOrderServices] AS WS ON WS.[ID] = WT.[WorkOrderServiceId]
                    LEFT OUTER JOIN WorkOrderTaskAttachments AS A ON A.[WorkOrderTaskId] = WT.[ID]
                WHERE WT.ID = @TaskId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TaskId", id);

            var rows = await this._baseDapperRepository.QueryChildListAsync<WorkOrderTaskDetailsViewModel, WorkOrderTaskAttachmentBaseViewModel>(query, parameters);
            return rows.Any() ? rows.First() : null;
        }

        public async Task<IEnumerable<WorkOrderTaskUpdateViewModel>> ReadAllUpdateDapperAsync(int workOrderId)
        {
            string query = $@"
                SELECT 
                    T.[ID],
                    T.[CreatedDate],
                    T.[UpdatedDate],
                    T.WorkOrderId,
                    T.[IsComplete],
                    T.[Description],
                    T.[ServiceId],
                    T.[UnitPrice],
                    ISNULL(T.[Quantity], 0) As [Quantity],
                    T.[DiscountPercentage],
                    T.[Note],
                    T.[LastCheckedDate],
                    T.[Location],
                    T.[WorkOrderServiceCategoryId],
                    T.[WorkOrderServiceId],
                    T.[UnitFactor],
                    T.[Frequency],
                    CASE 
		                WHEN T.WorkOrderServiceId IS NOT NULL THEN T.[Rate] 
		                ELSE T.[UnitPrice] 
	                END AS [Rate],
                    T.[QuantityExecuted],
                    T.[HoursExecuted],
                    CASE 
		                WHEN T.[WorkOrderServiceId] IS NOT NULL THEN WS.[Name]
		                WHEN T.[ServiceId] IS NOT NULL THEN S.[Name]  
		                ELSE ''
	                END AS [ServiceName],
                    CASE
                        WHEN S.[ID] IS NOT NULL AND WS.ID IS NULL THEN 1
                        ELSE 0
                    END AS [OldVersion],
                    T.[CompletedDate]
                FROM [WorkOrderTasks] AS T
                    LEFT JOIN [Services] AS S ON S.ID = T.[ServiceId]
                    LEFT JOIN [WorkOrderServiceCategories] AS C ON C.[ID] = T.[WorkOrderServiceCategoryId]
                    LEFT JOIN [WorkOrderServices] AS WS ON WS.[ID] = T.[WorkOrderServiceId]
                WHERE T.[WorkOrderId] = @workOrderId
                ORDER BY T.[UpdatedDate] DESC ";

            var pars = new DynamicParameters();
            pars.Add("@workOrderId", workOrderId);

            var rows = await _baseDapperRepository.QueryAsync<WorkOrderTaskUpdateViewModel>(query, pars);
            return rows.AsEnumerable();
        }
    }
}
