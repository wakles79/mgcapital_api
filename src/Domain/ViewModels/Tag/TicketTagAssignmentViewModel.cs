using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Tag
{
    public class TicketTagAssignmentViewModel
    {
        public int TicketTagId { get; set; }

        public int TagId { get; set; }

        public string Description { get; set; }

        public string HexColor { get; set; }
    }
}
