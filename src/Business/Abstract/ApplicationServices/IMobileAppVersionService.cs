using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.Business.Abstract.ApplicationServices
{
    public interface IMobileAppVersionService : IBaseApplicationService<MobileAppVersion, int>
    {
        Task<MobileAppVersion> Latest(MobileApp mobileApp, MobilePlatform platform);

        /// <summary>
        /// Returns all latest mobile versions for every platform
        /// </summary>
        /// <returns></returns>
        IEnumerable<MobileAppVersion> Latest();
    }
}
