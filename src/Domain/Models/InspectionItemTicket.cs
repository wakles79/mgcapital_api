using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class InspectionItemTicket : AuditableEntity
    {
        public int InspectionItemId { get; set; }

        [ForeignKey("InspectionItemId")]
        public InspectionItem InspectionItem { get; set; }

        public int TicketId { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        public TicketDestinationType? DestinationType { get; set; }

        public int? entityId { get; set; }
    }
}
