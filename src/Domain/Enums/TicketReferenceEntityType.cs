using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum TicketReferenceEntityType
    {
        None = 0,
        Ticket = 1,
        WorkOrder = 2,
        Employee = 4,
        CleaningReportItem = 8
    }
}
