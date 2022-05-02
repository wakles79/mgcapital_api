using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderCreateFromTicketViewModel : WorkOrderCreateViewModel
    {
        [Required]
        public int TicketId { get; set; }
    }
}
