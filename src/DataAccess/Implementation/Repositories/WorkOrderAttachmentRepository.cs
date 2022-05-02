using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class WorkOrderAttachmentRepository : BaseRepository<WorkOrderAttachment, int>, IWorkOrderAttachmentRepository
    {

        protected readonly IBaseDapperRepository _baseDapperRepository;

        public WorkOrderAttachmentRepository(MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<DataSource<WorkOrderAttachmentBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int workOrderId)
        {
            var result = new DataSource<WorkOrderAttachmentBaseViewModel>
            {
                Count = 0,
                Payload = new List<WorkOrderAttachmentBaseViewModel>()
            };

            string query = @"
                SELECT 
                        woat.[Id] AS Id,
                        woat.[BlobName] AS BlobName,
                        woat.[FullUrl] AS FullUrl,
                        woat.[Description] AS Description,
                        woat.[ImageTakenDate] AS ImageTakenDate,
                        woat.[WorkOrderId] AS WorkOrderId,
                        woat.[EmployeeId] AS EmployeeId 
                FROM    
                        [dbo].[WorkOrderAttachments] AS woat
                WHERE   
                        woat.[WorkOrderId] = @WorkOrderId 
                ORDER BY
                        woat.[ImageTakenDate] ";

            var pars = new DynamicParameters();
            pars.Add("@WorkOrderId", workOrderId);

            var payload = await _baseDapperRepository.QueryAsync<WorkOrderAttachmentBaseViewModel>(query, pars);

            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
