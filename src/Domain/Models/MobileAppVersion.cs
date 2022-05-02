using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Models
{
    public class MobileAppVersion : Entity
    {
        public MobileApp MobileApp { get; set; }

        public MobilePlatform Platform { get; set; }

        public string VersionNumber { get; set; }

        public string Url { get; set; }

        public bool Latest { get; set; }
    }
}
