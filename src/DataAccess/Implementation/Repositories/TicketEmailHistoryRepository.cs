using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class TicketEmailHistoryRepository : BaseRepository<TicketEmailHistory, int>, ITicketEmailHistoryRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public TicketEmailHistoryRepository(MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public Task<ulong> GetLastHistoryIdDapperAsync()
        {
            var query = $@" SELECT isnull(max([HistoryId]),0) as [HistoryId] FROM [dbo].[TicketEmailHistory]  ";
            return this._baseDapperRepository.QuerySingleOrDefaultAsync<ulong>(query,null);
        }

        public Task<TicketEmailHistory> FirstOrDefaultDapperAsync(string msgId)
        {
            const string query = @"
                SELECT TOP 1 *
                FROM dbo.TicketEmailHistory
                WHERE MessageId = @msgId
                ORDER BY Timestamp DESC
            ";
            
            var pars = new DynamicParameters();
            pars.Add("@msgId", msgId);

            return this._baseDapperRepository.QuerySingleOrDefaultAsync<TicketEmailHistory>(query, pars);
        }

        public Task UpdateHistoryIdAsync(int emailHistoryId, ulong msgHistoryId)
        {
            const string query = @"
                UPDATE dbo.TicketEmailHistory
                SET HistoryId = @historyId
                WHERE ID = @id
            "; 
            
            var pars = new DynamicParameters();
            pars.Add("@id", emailHistoryId);
            pars.Add("@historyId", msgHistoryId);

            return this._baseDapperRepository.ExecuteAsync(query, pars);
        }

        public Task AddDapperAsync(TicketEmailHistory ticketEmailHistory)
        {
            const string query = @"
                INSERT INTO dbo.TicketEmailHistory
                    (HistoryId, ThreadId, MessageId, RawMessage, Timestamp)
                VALUES (@historyId, @threadId, @messageId, @rawMessage, @timestamp)
            "; 
            
            var pars = new DynamicParameters();
            pars.Add("@historyId", ticketEmailHistory.HistoryId);
            pars.Add("@threadId", ticketEmailHistory.ThreadId);
            pars.Add("@messageId", ticketEmailHistory.MessageId);
            pars.Add("@rawMessage", ticketEmailHistory.RawMessage);
            pars.Add("@timestamp", ticketEmailHistory.Timestamp);

            return this._baseDapperRepository.ExecuteAsync(query, pars);
        }
    }
}
