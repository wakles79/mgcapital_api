using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketToMergeGridViewModel : EntityViewModel
    {
        public int Number { get; set; }

        public string Description { get; set; }

        public TicketStatus Status { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();
    }
}
