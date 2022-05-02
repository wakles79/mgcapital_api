using MGCap.Business.Abstract.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class MobileAppVersionService : BaseApplicationService<MobileAppVersion, int>, IMobileAppVersionService
    {
        public new IMobileAppVersionRepository Repository => base.Repository as IMobileAppVersionRepository;

        public MobileAppVersionService(IMobileAppVersionRepository repository) : base(repository)
        { }

        public async Task<MobileAppVersion> Latest(MobileApp mobileApp, MobilePlatform platform)
        {
            var result = await Repository.Latest(mobileApp, platform);

            return result;
        }

        public IEnumerable<MobileAppVersion> Latest()
        {
            return Repository.Latest();
        }
    }
}
