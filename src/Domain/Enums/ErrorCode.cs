using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    public enum ErrorCode
    {
        OK = 2001,
        PartialContent = 2006,
        BadRequest = 4000,
        Unauthorized = 4001,
        Forbidden = 4003,
        PreconditionFailed = 4012,
        ExpectationFailed = 4017,
        Other = 50000
    }
}
