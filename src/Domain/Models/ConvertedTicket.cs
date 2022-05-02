using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class ConvertedTicket : Entity
    {
        public int TicketId { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        public TicketDestinationType DestinationType { get; set; }

        /// <summary>
        ///     Destination Entity's ID
        /// </summary>
        public int DestinationEntityId { get; set; }

        public DateTime ConvertedDate { get; set; }
    }
}
