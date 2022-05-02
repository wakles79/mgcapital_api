using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.InspectionItemTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItem
{
    public class InspectionItemUpdateViewModel : InspectionItemBaseViewModel, IEntityParentViewModel<InspectionItemAttachmentUpdateViewModel,
        InspectionItemTaskUpdateViewModel, InspectionItemNoteUpdateViewModel>
    {
        public IEnumerable<InspectionItemAttachmentUpdateViewModel> Attachments { get => _children1; set { _children1 = value as IList<InspectionItemAttachmentUpdateViewModel>; } }
        public IEnumerable<InspectionItemTaskUpdateViewModel> Tasks { get => _children2; set { _children2 = value as IList<InspectionItemTaskUpdateViewModel>; } }
        public IEnumerable<InspectionItemNoteUpdateViewModel> Notes { get => _children3; set { _children3 = value as IList<InspectionItemNoteUpdateViewModel>; } }


        protected IList<InspectionItemAttachmentUpdateViewModel> _children1;
        protected IList<InspectionItemTaskUpdateViewModel> _children2;
        protected IList<InspectionItemNoteUpdateViewModel> _children3;

        public IList<InspectionItemAttachmentUpdateViewModel> Children1 { get => _children1; set { _children1 = value; } }
        public IList<InspectionItemTaskUpdateViewModel> Children2 { get => _children2; set { _children2 = value; } }
        public IList<InspectionItemNoteUpdateViewModel> Children3 { get => _children3; set { _children3 = value; } }
    }
}
