using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.InspectionItemTask;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MGCap.Domain.Utils;

namespace MGCap.Domain.ViewModels.InspectionItem
{
    public class InspectionItemGridViewModel : InspectionItemBaseViewModel,
        IEntityParentViewModel<InspectionItemAttachmentUpdateViewModel, InspectionItemTaskUpdateViewModel, InspectionItemNoteUpdateViewModel>
    {
        public string StatusName => this.Status.ToString().SplitCamelCase();

        public string PriorityName => this.Priority.ToString().SplitCamelCase();

        public string TypeName => this.Type.ToString().SplitCamelCase();

        public IEnumerable<InspectionItemAttachmentUpdateViewModel> Attachments => Children1 as IList<InspectionItemAttachmentUpdateViewModel>;

        public IList<InspectionItemAttachmentUpdateViewModel> Children1 { get; set; }

        public IEnumerable<InspectionItemTaskUpdateViewModel> Tasks => Children2 as IList<InspectionItemTaskUpdateViewModel>;

        public IEnumerable<InspectionItemNoteUpdateViewModel> Notes => Children3 as IList<InspectionItemNoteUpdateViewModel>;

        public IList<InspectionItemTaskUpdateViewModel> Children2 { get; set; }

        public IList<InspectionItemNoteUpdateViewModel> Children3 { get; set; }

        public bool EnableToCheckTasks { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
