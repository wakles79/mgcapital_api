using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReportItem
{
    public class CleaningReportItemCreateViewModel : CleaningReportItemBaseViewModel
    {
        public virtual IEnumerable<CleaningReportItemAttachmentCreateViewModel> Attachments { get; set; }
    }
}
