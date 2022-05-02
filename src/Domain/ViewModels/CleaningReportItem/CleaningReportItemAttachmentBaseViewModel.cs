using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReportItem
{
    public class CleaningReportItemAttachmentBaseViewModel : EntityViewModel
    {
        public string BlobName { get; set; }

        public string FullUrl { get; set; }

        public string Title { get; set; }

        public int CleaningReportItemId { get; set; }

        public DateTime ImageTakenDate { get; set; }
    }
}
