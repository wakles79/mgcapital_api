using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class CleaningReportItemAttachmentsRepository : BaseRepository<CleaningReportItemAttachment,int>, ICleaningReportItemAttachmentsRepository
    {
        protected readonly IBaseDapperRepository _baseDapperRepository;

        public CleaningReportItemAttachmentsRepository(MGCapDbContext dbContext, IBaseDapperRepository baseDapperRepository) : base(dbContext)
        {
            _baseDapperRepository = baseDapperRepository;
        }
    }
}
