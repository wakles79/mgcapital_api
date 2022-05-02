using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderAttachmentBaseViewModel : EntityViewModel
    {
        public int WorkOrderId { get; set; }

        public int EmployeeId { get; set; }

        public string BlobName { get; set; }

        public string FullUrl { get; set; }

        public string Description { get; set; }

        public DateTime ImageTakenDate { get; set; }
    }
}
