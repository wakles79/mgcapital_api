using System.Collections.Generic;
using System.Linq;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.InspectionItemTask;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketUpdateViewModel : TicketBaseViewModel, IEntityParentViewModel<TicketAttachmentUpdateViewModel, InspectionItemTaskUpdateViewModel>
    {
        public new IEnumerable<TicketAttachmentUpdateViewModel> Attachments { get => _children1?.OrderBy(a => a.ID); set => _children1 = value as IList<TicketAttachmentUpdateViewModel>; }

        protected IList<TicketAttachmentUpdateViewModel> _children1;

        public IList<TicketAttachmentUpdateViewModel> Children1 { get => _children1; set { _children1 = value; } }

        public IEnumerable<InspectionItemTaskUpdateViewModel> Tasks => Children2 as IList<InspectionItemTaskUpdateViewModel>;

        public IList<InspectionItemTaskUpdateViewModel> Children2 { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; set; }
    }
}
