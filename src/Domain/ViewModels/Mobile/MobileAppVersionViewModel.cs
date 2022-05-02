using MGCap.Domain.Enums;
using System;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Mobile
{
    public class MobileAppVersionViewModel
    {
        private Version _version;

        public MobileApp MobileApp { get; set; }

        public MobilePlatform Platform { get; set; }

        public string VersionNumber { get; set; }

        public string Url { get; set; }

        public Version LastAvailableVersion
        {
            get
            {
                _version = new Version(VersionNumber);
                return _version;
            }
            set
            {
                _version = value;
                VersionNumber = _version.ToString();
            }
        }
    }
}
