using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReportItem
{
    public class CleaningReportItemUpdateViewModel : CleaningReportItemBaseViewModel, IEntityParentViewModel<CleaningReportItemAttachmentUpdateViewModel>
    {

        public string BuildingName { get; set; }

        public IEnumerable<CleaningReportItemAttachmentUpdateViewModel> Attachments { get => _children1; set { _children1 = value as IList<CleaningReportItemAttachmentUpdateViewModel>; } }

        protected IList<CleaningReportItemAttachmentUpdateViewModel> _children1;

        public IList<CleaningReportItemAttachmentUpdateViewModel> Children1 { get => _children1; set { _children1 = value; } }
    }
}
