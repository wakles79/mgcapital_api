using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Ticket
{
    /// <summary>
    ///     Class to encapsulate a given status change to many tickets
    /// </summary>
    public class TicketUpdateStatusViewModel : EntityIdCollectionViewModel
    {
        [Required] 
        public TicketStatus Status { get; set; }
    }

    public class TicketUpdateSingleStatusViewModel : EntityViewModel
    {
        [Required] 
        public TicketStatus Status { get; set; }
    }
}