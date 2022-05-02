
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.WorkOrderTask;
using System.Collections.Generic;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderTaskDetailsViewModel : WorkOrderTaskBaseViewModel, IEntityParentViewModel<WorkOrderTaskAttachmentBaseViewModel>
    {
        public string ServiceName { get; set; }

        public double ServicePrice { get; set; }

        protected IList<WorkOrderTaskAttachmentBaseViewModel> _children1;

        public IList<WorkOrderTaskAttachmentBaseViewModel> Children1 { get => this._children1; set { this._children1 = value; } }

        public IEnumerable<WorkOrderTaskAttachmentBaseViewModel> Attachments => Children1 as IList<WorkOrderTaskAttachmentBaseViewModel>;
    }
}
