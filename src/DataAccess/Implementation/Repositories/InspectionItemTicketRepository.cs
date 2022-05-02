// -----------------------------------------------------------------------
// <copyright file="InspectionItemTicketRepository.cs" company="Axzes">
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
using MGCap.Domain.ViewModels.InspectionItemTicket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionItemTicketRepository : BaseRepository<InspectionItemTicket, int>, IInspectionItemTicketsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public InspectionItemTicketRepository(
            MGCapDbContext dbContext,
            IBaseDapperRepository baseDapperRepository
            ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<InspectionItemTicketDetailViewModel>> ReadAllWorkOrderFromInspection(int inspectionId)
        {
            string query = $@"
                SELECT 
	                IT.InspectionItemId,
	                IT.TicketId,
	                IT.DestinationType,
	                IT.entityId,
	                CASE	
                        WHEN II.[Status] = {(int)InspectionItemStatus.Closed} THEN II.Status
                        WHEN WO.ID IS NOT NULL AND WO.[StatusId] != {(int)WorkOrderStatus.Closed} THEN WO.[StatusId] + 3
                        WHEN TI.[ID] IS NOT NULL AND (TI.[Status] = {(int)TicketStatus.Undefined} OR TI.[Status] = {(int)TicketStatus.Draft}) THEN {(int)InspectionItemStatus.Ticket}
                        ELSE {(int)InspectionItemStatus.Open}
	                END AS TicketStatus
                FROM [dbo].[InspectionItemTickets] AS IT
                    LEFT JOIN Tickets TI ON TI.ID = IT.TicketId
	                LEFT JOIN WorkOrders AS WO ON IT.entityId = WO.ID AND IT.DestinationType = 1                    
	                LEFT JOIN CleaningReportItems AS CI ON IT.entityId = CI.ID AND IT.DestinationType >= 2
	                INNER JOIN InspectionItems II ON IT.InspectionItemId = II.ID
                WHERE II.InspectionId = @inspectionId";

            DynamicParameters pars = new DynamicParameters();
            pars.Add("@inspectionId", inspectionId);

            IEnumerable<InspectionItemTicketDetailViewModel> workOrders = await this._baseDapperRepository.QueryAsync<InspectionItemTicketDetailViewModel>(query, pars);
            
            return workOrders;
        }
    }
}
