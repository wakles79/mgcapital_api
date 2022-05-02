using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum TicketActivityType
    {
        FieldUpdated = 0,
        TicketMerged = 1,
        EmailReply = 2,
        None = 3,
        Forwarded = 4,
        AssignedEmployee = 8,
        TicketConverted = 16,
        TicketConvertedWorkOrderSequence = 32
    }
}
