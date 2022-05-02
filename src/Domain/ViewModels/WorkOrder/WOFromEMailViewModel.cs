using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WOFromEMailViewModel
    {
        [DisplayName("Requester name: ")]
        public string RequesterName { get; set; }

        public string RequesterEmail { get; set; }

        public string ReceivedDate { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }
    }
}
