using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    public enum InspectionStatus
    {
        Pending,
        Scheduled,
        Walkthrough,
        WalkthroughComplete,
        Active,
        Closed
    }
}
