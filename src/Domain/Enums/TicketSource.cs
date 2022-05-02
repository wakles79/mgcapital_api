using System;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum TicketSource
    {
        Undefined = 0,
        ClientApp = 1,
        WorkOrderForm = 2,
        ManagerApp = 4,
        InternalTicket = 8,
        Inspection = 16,
        Email = 64
    }
}
