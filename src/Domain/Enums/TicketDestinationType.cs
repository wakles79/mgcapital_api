using System;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum TicketDestinationType
    {
        Undefined = 0,
        WorkOrder = 1,
        CleaningItem = 2,
        FindingItem = 4
    }
}
