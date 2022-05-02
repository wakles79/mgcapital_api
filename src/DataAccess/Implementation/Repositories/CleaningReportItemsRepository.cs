// -----------------------------------------------------------------------
// <copyright file="CleaningReportItemsRepository.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------
using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.CleaningReportItem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    /// <summary>
    ///     Contains the implementation of the functionalities
    ///     for handling operations on the <see cref="CleaningReportItem"/>
    /// </summary>
    public class CleaningReportItemsRepository : BaseRepository<CleaningReportItem,int>, ICleaningReportItemsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CleaningReportItemsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/>  implementation</param>
        public CleaningReportItemsRepository(
            MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository)
            : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }

        public async Task<IEnumerable<CleaningReportItemGridViewModel>> GetCleaningReportItemsDapper(int companyId,int cleaningReportId, int? type = null)
        {
            IEnumerable< CleaningReportItemGridViewModel> result = new List<CleaningReportItemGridViewModel>();

            var typeWhere = type!=null ? $" AND Type = @type;" : ";";

            var query = $@"
	                    SELECT 
	                    [dbo].[CleaningReportItems].[ID],
						[dbo].[CleaningReportItems].CleaningReportId,
						[dbo].[CleaningReportItems].[BuildingId],
						[dbo].[CleaningReportItems].[Location],
						[dbo].[CleaningReportItems].[Observances],
						[dbo].[CleaningReportItems].[Time],
						[dbo].[CleaningReportItems].[Type],
						([dbo].[Buildings].[Name]) as BuildingName,
						ISNULL([dbo].[CleaningReportItemAttachments].ID, -1) AS [Id],
						[dbo].[CleaningReportItemAttachments].BlobName,
						[dbo].[CleaningReportItemAttachments].[FullUrl],
						[dbo].[CleaningReportItemAttachments].[Title],
						[dbo].[CleaningReportItemAttachments].[CleaningReportItemId],
						[dbo].[CleaningReportItemAttachments].[ImageTakenDate]

	                    FROM [dbo].[CleaningReportItems]
								INNER JOIN [dbo].[Buildings] ON [dbo].[Buildings].ID = [dbo].[CleaningReportItems].[BuildingId]
								LEFT OUTER JOIN [dbo].[CleaningReportItemAttachments] ON [dbo].[CleaningReportItemAttachments].[CleaningReportItemId] = [dbo].[CleaningReportItems].ID

					                   
                        WHERE CompanyId = @companyId AND CleaningReportId = @cleaningReportId
                                {typeWhere}
                        ";

            var pars = new DynamicParameters();
            pars.Add("@cleaningReportId", cleaningReportId);
            pars.Add("@companyId", companyId);
            pars.Add("@type", type);

            var payload = await _baseDapperRepository.QueryChildListAsync<CleaningReportItemGridViewModel, CleaningReportItemAttachmentUpdateViewModel>(query, pars);
            result = payload;
            return result;

        }


        public async Task<CleaningReportItemUpdateViewModel> GetCleaningReportItemDapperAsync(int id)
        {
            var result = new CleaningReportItemUpdateViewModel();

            var query = @"
	                    SELECT 
	                    [dbo].[CleaningReportItems].[ID],
						[dbo].[CleaningReportItems].CleaningReportId,
						[dbo].[CleaningReportItems].[BuildingId],
						[dbo].[CleaningReportItems].[Location],
						[dbo].[CleaningReportItems].[Observances],
						[dbo].[CleaningReportItems].[Time],
						[dbo].[CleaningReportItems].[Type],
						([dbo].[Buildings].[Name]) as BuildingName,
						ISNULL([dbo].[CleaningReportItemAttachments].ID, -1) AS [Id],
						[dbo].[CleaningReportItemAttachments].BlobName,
						[dbo].[CleaningReportItemAttachments].[FullUrl],
						[dbo].[CleaningReportItemAttachments].[Title],
						[dbo].[CleaningReportItemAttachments].[CleaningReportItemId],
						[dbo].[CleaningReportItemAttachments].[ImageTakenDate]

	                    FROM [dbo].[CleaningReportItems]
								INNER JOIN [dbo].[Buildings] ON [dbo].[Buildings].ID = [dbo].[CleaningReportItems].[BuildingId]
								LEFT OUTER JOIN [dbo].[CleaningReportItemAttachments] ON [dbo].[CleaningReportItemAttachments].[CleaningReportItemId] = [dbo].[CleaningReportItems].ID

					                   
                        WHERE [dbo].[CleaningReportItems].ID = @id;                        
                        ";

            var pars = new DynamicParameters();
            pars.Add("@id", id);

            var res = await _baseDapperRepository.QueryChildListAsync<CleaningReportItemUpdateViewModel, CleaningReportItemAttachmentUpdateViewModel>(query, pars);
            result = res.FirstOrDefault();

            return result;
        }

        public override async Task<CleaningReportItem> SingleOrDefaultAsync(Func<CleaningReportItem, bool> filter)
        {
            return await this.Entities
                            .Include(item => item.CleaningReportItemAttachments)
                            .SingleOrDefaultAsync(ent => filter.Invoke(ent));
        }

    }
}
