using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.CleaningReport
{
    public class CleaningReportCustomerBaseViewModel : EntityViewModel, IGridViewModel
    {
        public Guid Guid { get; set; }

        public string Location { get; set; }

        public DateTime Date { get; set; }

        public int CleaningItemsCount { get; set; }

        public int FindingItemsCount { get; set; }

        public int Count { get; set; }
    }
}
