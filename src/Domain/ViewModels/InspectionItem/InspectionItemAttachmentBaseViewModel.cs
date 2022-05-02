using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItem
{
    public class InspectionItemAttachmentBaseViewModel : EntityViewModel
    {
        public string BlobName { get; set; }

        public string FullUrl { get; set; }

        public string Title { get; set; }

        public int InspectionItemId { get; set; }

        public string Description { get; set; }

        public DateTime ImageTakenDate { get; set; }
    }
}
