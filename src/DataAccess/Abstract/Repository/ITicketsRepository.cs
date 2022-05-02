// -----------------------------------------------------------------------
// <copyright file="ITicketsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Calendar;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Ticket;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    /// <summary>
    ///     Contains the declaration of the base
    ///     functionalities for the repositories
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity that the actual implementation
    ///     of this interface handles
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the <typeparamref name="TEntity"/>'s Primary Key
    /// </typeparam>
    public interface ITicketsRepository : IBaseRepository<Ticket, int>
    {
        Task<DataSource<TicketGridViewModel>> ReadAllDapperAsync(DataSourceRequestTicket request, int companyId, int? employeeId);

        Task<TicketDetailsViewModel> GetTicketDetailsDapperAsync(int id = -1, Guid? guid = null);

        Task<int> GetPendingTicketsCountDapperAsync(int companyId);

        Task<DataSource<CalendarGridViewModel>> GetTicketCalendar(DataSourceRequest request, int companyId);

        Task<DataSource<TicketToMergeGridViewModel>> ReadAllToMergeAsync(int companyId, int paramType, string value);

        Task<Ticket> UpdateByRequesterResponseAsync(int id, bool updateStatus);

        void AddDapperAsync(Ticket obj, string userEmail, int companyId);

        Task<Ticket> FirstOrDefaultDapperAsync(string threadId, string messageId);

        Task GMailUpdateByRequesterResponseAsync(int id, decimal historyId);

        Ticket GetLastGmailHistoryId(int companyId);

        Task<int> AddDapperV2Async(Ticket newTicket);
    }
}
