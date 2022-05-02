using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum CalendarEventType
    {
        Ticket,
        Inspection,
        WorkOrder
    }
}
