// -----------------------------------------------------------------------
// <copyright file="InspectionItemsRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.InspectionItem;
using MGCap.Domain.ViewModels.InspectionItemTask;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionItemsRepository : BaseRepository<InspectionItem, int>, IInspectionItemsRepository
    {
        private readonly IBaseDapperRepository _baseDapperRepository;


        public InspectionItemsRepository(
        MGCapDbContext dbContext,
        IBaseDapperRepository baseDapperRepository
        ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        private int NextNumber(int inspectionId)
        {
            var maxNumber = 0;
            if (this.Entities != null && this.Entities.Count() > 0)
            {
                maxNumber = this.Entities
                                    ?.Where(ent => ent.InspectionId == inspectionId)
                                    ?.DefaultIfEmpty()
                                    ?.Max(ent => ent.Number) ?? 0;
            }
            return maxNumber + 1;
        }

        public override async Task<InspectionItem> AddAsync(InspectionItem obj)
        {
            return await Task.Run(() => { return Add(obj); });
        }

        public override InspectionItem Add(InspectionItem obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "The given object must not be null");
            }

            obj.Number = this.NextNumber(obj.InspectionId);

            return base.Add(obj);
        }

        public async Task<DataSource<InspectionItemGridViewModel>> ReadAllByInspectionDapperAsync(DataSourceRequest request, int inspectionId)
        {
            var result = new DataSource<InspectionItemGridViewModel>
            {
                Payload = new List<InspectionItemGridViewModel>(),
                Count = 0
            };

            string orders = string.IsNullOrEmpty(request.SortOrder) || string.IsNullOrEmpty(request.SortField) ? "ID" : $"{request.SortField} {request.SortOrder.ToUpper()}";

            string query = $@"
                -- payload query
                SELECT *, [Count] = COUNT(*) OVER() FROM (
                    SELECT 
	                    II.[ID],
	                    II.[CreatedDate],
	                    II.[UpdatedDate],
	                    II.[UpdatedBy],
	                    II.[Number],
	                    II.[Position],
	                    II.[Description],
	                    II.[Latitude],
	                    II.[Longitude],
	                    II.[InspectionId],
	                    CASE
		                    WHEN II.[Status] = {(int)InspectionItemStatus.Closed} THEN II.Status
                            WHEN WO.ID IS NOT NULL AND WO.[StatusId] != {(int)WorkOrderStatus.Closed} THEN WO.[StatusId] + 3
                            WHEN TI.[ID] IS NOT NULL AND (TI.[Status] = {(int)TicketStatus.Undefined} OR TI.[Status] = {(int)TicketStatus.Draft}) THEN {(int)InspectionItemStatus.Ticket}
                            ELSE {(int)InspectionItemStatus.Open}
	                    END AS [Status]
                    FROM dbo.[InspectionItems] AS II
	                    LEFT JOIN dbo.[InspectionItemTickets] IT ON IT.InspectionItemId = II.ID
	                    LEFT JOIN dbo.[Tickets] TI ON TI.ID  = IT.TicketId
	                    LEFT JOIN WorkOrders AS WO ON IT.entityId = WO.ID AND IT.DestinationType = 1
                    WHERE II.[InspectionId] = @InspectionId
                ) payload 
                ORDER BY {orders} OFFSET @PageSize * @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pars = new DynamicParameters();
            pars.Add("@InspectionId", inspectionId);
            pars.Add("@PageNumber", request.PageNumber);
            pars.Add("@PageSize", request.PageSize);
            pars.Add("@filter", request.Filter);

            var payload = await this._baseDapperRepository.QueryAsync<InspectionItemGridViewModel>(query, pars);
            result.Count = payload.FirstOrDefault()?.Count ?? 0;

            result.Payload = payload;

            return result;
        }
        public override async Task<InspectionItem> SingleOrDefaultAsync(Func<InspectionItem, bool> filter)
        {
            return await this.Entities
                            .Include(item => item.Attachments)
                            .Include(item => item.Tasks)
                            .Include(item => item.Notes)
                            .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

        public async Task<InspectionItemUpdateViewModel> GetInspectionItemDapperAsync(int id)
        {
            var result = new InspectionItemUpdateViewModel();

            var query = @"

  SELECT 
	                    [dbo].[InspectionItems].[ID],
						[dbo].[InspectionItems].InspectionId,
						[dbo].[InspectionItems].Latitude,
						[dbo].[InspectionItems].Longitude,
						[dbo].[InspectionItems].Number,
						[dbo].[InspectionItems].Position,
						[dbo].[InspectionItems].Description,
						[dbo].[InspectionItems].Priority,
						[dbo].[InspectionItems].Type,
						ISNULL([dbo].[InspectionItemAttachments].ID, -1) AS [Id],
						[dbo].[InspectionItemAttachments].BlobName,
						[dbo].[InspectionItemAttachments].[FullUrl],
						[dbo].[InspectionItemAttachments].[Title],
						[dbo].[InspectionItemAttachments].InspectionItemId,
						[dbo].[InspectionItemAttachments].[ImageTakenDate],
                        ISNULL([dbo].[InspectionItemTasks].ID, -1) AS [Id],
                        [dbo].InspectionItemTasks.InspectionItemId,
                        [dbo].[InspectionItemTasks].[IsComplete],
                        [dbo].[InspectionItemTasks].[Description],

			            ISNULL([dbo].[InspectionItemNotes].ID, -1) AS [Id],
                        [dbo].[InspectionItemNotes].InspectionItemId,
                        [dbo].[InspectionItemNotes].[EmployeeId],
                        [dbo].[InspectionItemNotes].[CreatedDate],
						[dbo].[InspectionItemNotes].[CreatedBy],
						[dbo].[InspectionItemNotes].[UpdatedBy],
						[dbo].[InspectionItemNotes].[UpdatedDate],
                        [dbo].[InspectionItemNotes].[Note],
						CONCAT([dbo].[Contacts].[FirstName], ' ', [dbo].[Contacts].[MiddleName], ' ', [dbo].[Contacts].[LastName]) AS [EmployeeFullName]


	                    FROM [dbo].[InspectionItems]
								LEFT OUTER JOIN [dbo].[InspectionItemAttachments] ON [dbo].[InspectionItemAttachments].InspectionItemId = [dbo].InspectionItems.ID
                                LEFT OUTER JOIN [dbo].[InspectionItemTasks] ON [dbo].InspectionItems.[ID] = [dbo].[InspectionItemTasks].InspectionItemId
					            LEFT OUTER JOIN [dbo].[InspectionItemNotes] ON [dbo].InspectionItems.[ID] = [dbo].[InspectionItemNotes].InspectionItemId       
								LEFT OUTER JOIN [dbo].[Employees] ON InspectionItemNotes.[EmployeeId] = [dbo].[Employees].[ID]
								LEFT OUTER JOIN [dbo].[Contacts] ON [dbo].[Employees].[ContactId] = [dbo].[Contacts].[ID]

                        WHERE [dbo].[InspectionItems].ID = @id;      
                        ";

            var pars = new DynamicParameters();
            pars.Add("@id", id);

            var res = await _baseDapperRepository.QueryChildrenListAsync<InspectionItemUpdateViewModel,
                                                                          InspectionItemAttachmentUpdateViewModel,
                                                                          InspectionItemTaskUpdateViewModel,
                                                                          InspectionItemNoteUpdateViewModel>(query, pars);
            result = res.FirstOrDefault();

            return result;
        }
    }
}