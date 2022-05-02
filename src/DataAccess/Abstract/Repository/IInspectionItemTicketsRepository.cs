// -----------------------------------------------------------------------
// <copyright file="IInspectionItemTicketRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.InspectionItemTicket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IInspectionItemTicketsRepository : IBaseRepository<InspectionItemTicket, int>
    {
        Task<IEnumerable<InspectionItemTicketDetailViewModel>> ReadAllWorkOrderFromInspection(int inspectionId);
    }
}
