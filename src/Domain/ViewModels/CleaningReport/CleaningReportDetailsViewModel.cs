using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.CleaningReportItem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportDetailsViewModel : CleaningReportBaseViewModel
    {
        public Guid Guid { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Companyname { get; set; }

        public string ToEmail { get; set; }

        public string FormattedTo
        {
            get
            {
                return To + " / " + Companyname;
            }
        }

        public IEnumerable<CleaningReportItemGridViewModel> CleaningItems { get; set; }

        public IEnumerable<CleaningReportItemGridViewModel> FindingItems { get; set; }

        public IEnumerable<CleaningReportNoteViewModel> Notes { get; set; }
    }
}
