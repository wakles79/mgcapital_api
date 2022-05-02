using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrderTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskUpdateViewModel : WorkOrderTaskBaseViewModel
    {
        public string ServiceName { get; set; }

        public IEnumerable<WorkOrderTaskAttachmentCreateViewModel> Attachments { get; set; }

        public WorkOrderTaskUpdateViewModel()
        {
            this.Attachments = new HashSet<WorkOrderTaskAttachmentCreateViewModel>();
        }
    }
}
