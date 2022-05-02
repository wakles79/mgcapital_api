using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItemTicket
{
    public class InspectionItemTicketDetailViewModel : InspectionItemTicketBaseViewModel
    {
        /// <summary>
        /// Can be:
        /// <para>0 => Undefined</para>
        /// <para>1 => Ticket</para>
        /// <para>2 => Draft</para>
        /// <para>3 => Stand By</para>
        /// <para>4 => Active</para>
        /// <para>5 => Closed</para>
        /// <para>6 => Cleaning Report Item</para>
        /// <para>7 => Cleaning Report Finding</para>
        /// <para>8 => Cancelled</para>
        /// <para>9 => Ticket Resolved</para>
        /// </summary>
        public int TicketStatus { get; set; }
    }
}
