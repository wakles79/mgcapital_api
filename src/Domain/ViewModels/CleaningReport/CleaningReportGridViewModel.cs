using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportGridViewModel : CleaningReportBaseViewModel, IGridViewModel
    {
        public Guid Guid { get; set; }

        public string PreparedFor { get; set; }

        public string To { get; set; }

        public int CleaningItems { get; set;}

        public int FindingItems { get; set; }

        public string CompanyName { get; set; }

        public string FormattedTo
        {
            get
            {
                return To + " / " + CompanyName;
            }
        }

        public string Status
        {
            get
            {
                return Submitted > 0 ? CleaningReportStatus.Sent.ToString().ToLower() : CleaningReportStatus.Draft.ToString().ToLower();
            }
        }


        [IgnoreDataMember]
        public int Count { get; set; }

        public int LastCommentDirection { get; set; }
    }
}
