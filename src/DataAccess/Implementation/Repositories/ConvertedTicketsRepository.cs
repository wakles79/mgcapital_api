using MGCap.DataAccess.Abstract.Repository;
using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class ConvertedTicketsRepository : BaseRepository<ConvertedTicket, int>, IConvertedTicketsRepository
    {
        private IBaseDapperRepository _baseDapperRepository { get; }

        public ConvertedTicketsRepository(
            MGCapDbContext context,
            IBaseDapperRepository baseDapperRepository
            ) : base(context)
        {
            this._baseDapperRepository = baseDapperRepository;
        }
    }
}
