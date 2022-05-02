using MGCap.DataAccess.Implementation.Context;
using MGCap.DataAccess.Implementation.Repositories;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public class MobileAppVersionRepository : BaseRepository<MobileAppVersion, int>, IMobileAppVersionRepository
    {
        public MobileAppVersionRepository(MGCapDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<MobileAppVersion> AddAsync(MobileAppVersion obj)
        {
            var latest = await this.Latest(obj.MobileApp, obj.Platform);

            if (latest != null)
            {
                latest.Latest = false;
            }

            obj.Latest = true;

            obj = await base.AddAsync(obj);

            return obj;
        }

        public async Task<MobileAppVersion> Latest(MobileApp mobileApp, MobilePlatform platform)
        {
            var latest = await Entities.FirstOrDefaultAsync(v => IsLatest(v, mobileApp, platform));

            return latest;
        }

        public IEnumerable<MobileAppVersion> Latest()
        {
            return Entities.Where(v => IsLatest(v, v.MobileApp, v.Platform));
        }

        private bool IsLatest(MobileAppVersion version, MobileApp mobileApp, MobilePlatform platform)
        {
            return version.MobileApp.Equals(mobileApp) && version.Platform.Equals(platform) && version.Latest.Equals(true);
        }
    }
}
