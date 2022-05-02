using System;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum TicketStatus
    {
        Undefined = 0,
        Draft = 1,
        Converted = 2, // Converted to the "dark side" 
        Resolved = 4,
    }
}
