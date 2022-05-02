using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.TicketCategory
{
    public class TicketCategoryGridViewModel : TicketCategoryBaseViewModel, IGridViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
