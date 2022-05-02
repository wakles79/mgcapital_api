using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.WorkOrderServiceCategory
{
    public class WorkOrderServiceCategoryGridViewModel : WorkOrderServiceCategoryBaseViewModel, IGridViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
