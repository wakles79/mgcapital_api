// -----------------------------------------------------------------------
// <copyright file="InspectionItemAttachmentsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------

using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.InspectionItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionItemAttachmentsRepository : BaseRepository<InspectionItemAttachment, int>, IInspectionItemAttachmentsRepository
    {
       private readonly IBaseDapperRepository _baseDapperRepository;

       public InspectionItemAttachmentsRepository(
       MGCapDbContext dbContext,
       IBaseDapperRepository baseDapperRepository
       ) : base(dbContext)
       {
            this._baseDapperRepository = baseDapperRepository;
       }

        public async Task<DataSource<InspectionItemAttachmentBaseViewModel>> ReadAllDapperAsync(DataSourceRequest request, int inspectionItemId)
        {
            var result = new DataSource<InspectionItemAttachmentBaseViewModel>
            {
                Count = 0,
                Payload = new List<InspectionItemAttachmentBaseViewModel>()
            };

            string query = @"
                SELECT 
                            [ID],
                            [CreatedDate],
                            [CreatedBy],
                            [UpdatedDate],
                            [UpdatedBy],
                            [Description],
                            [BlobName],
                            [FullUrl],
                            [InspectionItemId],
                            [ImageTakenDate],
                            [Title]
                 FROM [dbo].[InspectionItemAttachments] as iiat
                 WHERE   
                 iiat.[InspectionItemId] = @InspectionItemId";

            var pars = new DynamicParameters();
            pars.Add("@InspectionItemId", inspectionItemId);

            var payload = await _baseDapperRepository.QueryAsync<InspectionItemAttachmentBaseViewModel>(query, pars);

            result.Count = payload?.Count() ?? 0;
            result.Payload = payload;

            return result;
        }
    }
}
