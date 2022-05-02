using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionDetailViewModel : InspectionBaseViewModel
    {
        public Guid Guid { get; set; }

        public string StatusName => this.Status.ToString().SplitCamelCase();
    }
}
