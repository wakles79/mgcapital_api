using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketMergeViewModel
    {
        /// <summary>
        /// Identifier of the primary ticket
        /// </summary>
        public int TicketId { get; set; }

        /// <summary>
        /// Identifier of the tickets that will be merged
        /// </summary>
        public IEnumerable<int> TicketsId { get; set; }

        public TicketMergeViewModel()
        {
            this.TicketsId = new HashSet<int>();
        }
    }
}
