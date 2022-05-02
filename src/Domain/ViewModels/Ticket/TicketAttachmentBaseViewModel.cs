using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketAttachmentBaseViewModel : EntityViewModel
    {
        public int TicketId { get; set; }

        public string BlobName { get; set; }

        public string FullUrl { get; set; }

        public string Description { get; set; }

    }
}
