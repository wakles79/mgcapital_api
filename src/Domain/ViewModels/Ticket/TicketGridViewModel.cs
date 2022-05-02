using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Tag;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGCap.Domain.ViewModels.Ticket
{
    public class TicketGridViewModel : TicketBaseViewModel, IGridViewModel, IEntityParentViewModel<TagBaseViewModel>
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public Guid Guid { get; set; }
        public string BuildingName { get; set; }
        public int AttachmentsCount { get; set; }

        public string AssignedEmployeeName { get; set; }

        public IList<TagBaseViewModel> Children1 { get; set; }

        public IEnumerable<TagBaseViewModel> Tags => Children1 as IList<TagBaseViewModel>;

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
