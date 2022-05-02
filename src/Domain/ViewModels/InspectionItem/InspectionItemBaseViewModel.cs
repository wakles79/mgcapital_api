using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItem
{
    public class InspectionItemBaseViewModel : EntityViewModel
    {
        public int Number { get; set; }

        public string Position { get; set; }

        public string Description { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int InspectionId { get; set; }

        public WorkOrderPriority Priority { get; set; }

        public WorkOrderType Type { get; set; }

        public InspectionItemStatus Status { get; set; }
    }
}
