using MGCap.Domain.Enums;
using System.Collections.Generic;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestTicket : DataSourceRequest
    {
        public TicketSource Source { get; set; }

        public TicketStatus Status { get; set; }

        public TicketDestinationType DestinationType { get; set; }

        public UserType UserType { get; set; }

        public int? UserId { get; set; }

        /// <summary>
        ///     Flag to display all snoozed tickets
        /// </summary>
        public bool? ShowSnoozed { get; set; }

        public bool OnlyAssigned { get; set; }

        public int? BuildingId { get; set; }

        public string StrTags { get; set; }

        public bool? IsDeleted { get; set; }

    }
}
