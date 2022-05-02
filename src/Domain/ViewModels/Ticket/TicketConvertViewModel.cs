using MGCap.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketConvertViewModel
    {
        [Required]
        public int TicketId { get; set; }
        [Required]
        public TicketDestinationType DestinationType { get; set; }
        [Required]
        public int DestinationEntityId { get; set; }
    }
}
