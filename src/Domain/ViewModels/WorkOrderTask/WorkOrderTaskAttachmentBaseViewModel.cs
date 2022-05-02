using MGCap.Domain.ViewModels.Common;
using System;

namespace MGCap.Domain.ViewModels.WorkOrderTask
{
    public class WorkOrderTaskAttachmentBaseViewModel : EntityViewModel
    {
        public string Description { get; set; }

        public string BlobName { get; set; }

        public string FullUrl { get; set; }

        public string Title { get; set; }

        public DateTime ImageTakenDate { get; set; }

        public int WorkOrderTaskId { get; set; }
    }
}
