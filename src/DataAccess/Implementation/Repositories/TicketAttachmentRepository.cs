using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Ticket;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class TicketAttachmentRepository : BaseRepository<TicketAttachment, int>, ITicketAttachmentRepository
    {

        protected readonly IBaseDapperRepository _baseDapperRepository;

        public TicketAttachmentRepository(MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task AddDapperAsync(TicketAttachment ticketAttachment)
        {
            var query = @"
                INSERT INTO TicketAttachments
                (CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, BlobName, FullUrl, TicketId, Description, GmailId)
                VALUES (@CreatedDate,
                        @CreatedBy,
                        @UpdatedDate,
                        @UpdatedBy,
                        @BlobName,
                        @FullUrl,
                        @TicketId,
                        @Description,
                        @GmailId)
            ";

            var pars = new DynamicParameters();
            pars.Add("@CreatedDate", ticketAttachment.CreatedDate);
            pars.Add("@CreatedBy", ticketAttachment.CreatedBy);
            pars.Add("@UpdatedDate", ticketAttachment.UpdatedDate);
            pars.Add("@UpdatedBy", ticketAttachment.UpdatedBy);
            pars.Add("@BlobName", ticketAttachment.BlobName);
            pars.Add("@FullUrl", ticketAttachment.FullUrl);
            pars.Add("@TicketId", ticketAttachment.TicketId);
            pars.Add("@Description", ticketAttachment.Description);
            pars.Add("@GmailId", ticketAttachment.GmailId);

            await _baseDapperRepository.ExecuteAsync(query, pars);
        }

        public Task<TicketAttachment> GetTicketAttachmentByGmailIdAsync(string gmailId)
        {
            var query = "SELECT TOP 1 * FROM TicketAttachments WHERE GmailId = @GmailId";
            var pars = new DynamicParameters();
            pars.Add("@GmailId", gmailId);
            return this._baseDapperRepository.QuerySingleOrDefaultAsync<TicketAttachment>(query, pars);
        }

        public async Task<DataSource<TicketAttachmentBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int TicketId)
        {
            var result = new DataSource<TicketAttachmentBaseViewModel>
            {
                Count = 0,
                Payload = new List<TicketAttachmentBaseViewModel>()
            };

            string query = @"
                SELECT 
                        TA.[Id] AS Id,
                        TA.[BlobName] AS BlobName,
                        TA.[FullUrl] AS FullUrl,
                        TA.[Description] AS Description,
                        TA.[TicketId] AS TicketId,

                FROM    
                        [dbo].[TicketAttachments] AS TA
                WHERE   
                        TA.[TicketId] = @ticketId ";

            var pars = new DynamicParameters();
            pars.Add("@ticketId", TicketId);

            var payload = await _baseDapperRepository.QueryAsync<TicketAttachmentBaseViewModel>(query, pars);

            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
