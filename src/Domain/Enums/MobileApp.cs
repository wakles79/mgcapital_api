
using System;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum MobileApp : short
    {
        MGManager = 1 << 0,
        MGClient = 1 << 1,
    }

    [Flags]
    public enum MobilePlatform : short
    {
        IOS = 1 << 0,
        Android = 1 << 1
    }
}