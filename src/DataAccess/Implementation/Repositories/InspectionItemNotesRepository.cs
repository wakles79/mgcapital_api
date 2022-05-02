using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class InspectionItemNotesRepository : BaseRepository<InspectionItemNote, int>, IInspectionItemNotesRepository
    {

        private readonly IBaseDapperRepository _baseDapperRepository;

        public InspectionItemNotesRepository(
         MGCapDbContext dbContext,
         IBaseDapperRepository baseDapperRepository
         ) : base(dbContext)
        {
            this._baseDapperRepository = baseDapperRepository;
        }
    }
}
