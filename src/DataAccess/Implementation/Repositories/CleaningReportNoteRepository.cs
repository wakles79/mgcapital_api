using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class CleaningReportNoteRepository : BaseRepository<CleaningReportNote, int>, ICleaningReportNoteRepository
    {
        public CleaningReportNoteRepository(MGCapDbContext dbContext) : base(dbContext)
        {
        }
    }
}
