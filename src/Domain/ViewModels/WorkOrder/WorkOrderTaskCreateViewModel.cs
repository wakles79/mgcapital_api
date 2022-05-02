using MGCap.Domain.ViewModels.WorkOrderTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskCreateViewModel : WorkOrderTaskBaseViewModel
    {
        public IEnumerable<WorkOrderTaskAttachmentCreateViewModel> Attachments { get; set; }

        public WorkOrderTaskCreateViewModel()
        {
            this.Attachments = new HashSet<WorkOrderTaskAttachmentCreateViewModel>();
        }
    }
}
