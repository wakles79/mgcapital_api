using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IMobileAppVersionRepository : IBaseRepository<MobileAppVersion, int>
    {
        Task<MobileAppVersion> Latest(MobileApp mobileApp, MobilePlatform platform);

        /// <summary>
        /// Returns all latest mobile versions for every platform
        /// </summary>
        /// <returns></returns>
        IEnumerable<MobileAppVersion> Latest();
    }
}
