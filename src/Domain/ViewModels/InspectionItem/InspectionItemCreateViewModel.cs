using MGCap.Domain.ViewModels.InspectionItemTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItem
{
    public class InspectionItemCreateViewModel : InspectionItemBaseViewModel
    {
        public virtual IEnumerable<InspectionItemAttachmentCreateViewModel> Attachments { get; set; }

        public virtual IEnumerable<InspectionItemTaskCreateViewModel> Tasks { get; set; }

        public virtual IEnumerable<InspectionItemNoteCreateViewModel> Notes { get; set; }
    }
}
