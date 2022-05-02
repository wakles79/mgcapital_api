using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItemTicket
{
    public class InspectionItemTicketBaseViewModel
    {
        public int InspectionItemId { get; set; }

        public int TicketId { get; set; }

        public TicketDestinationType? DestinationType { get; set; }

        public int? entityId { get; set; }
    }
}
