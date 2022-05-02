using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReportItem
{
    public class CleaningReportItemGridViewModel : CleaningReportItemBaseViewModel, IEntityParentViewModel<CleaningReportItemAttachmentUpdateViewModel>
    {
        public string BuildingName { get; set; }

        public IEnumerable<CleaningReportItemAttachmentUpdateViewModel> Attachments => Children1 as IList<CleaningReportItemAttachmentUpdateViewModel>;

        public IList<CleaningReportItemAttachmentUpdateViewModel> Children1 { get; set; }
        [IgnoreDataMember]
        public int Count { get; set; }

    }
}
