using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class TicketAttachment : Attachment
    {
        public int TicketId { get; set; }

        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        public string GmailId { get; set; }
    }
}
