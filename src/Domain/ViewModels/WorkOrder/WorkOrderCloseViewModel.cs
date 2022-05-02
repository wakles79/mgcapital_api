using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderCloseViewModel
    {
        public int WorkOrderId { get; set; }

        public string ClosingNotes { get; set; }

        public WorkOrderClosingNotes AdditionalNotes { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating if the closing notes will have a different treatment
        /// </summary>
        public bool FollowUpOnClosingNotes { get; set; }

        public string ClosingNotesOther { get; set; }
    }
}
