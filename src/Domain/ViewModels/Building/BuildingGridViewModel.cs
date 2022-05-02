using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingGridViewModel : EntityViewModel
    {
        public string Name { get; set; }

        public string FullAddress { get; set; }

        public string OperationsManagerFullName { get; set; }

        public int IsActive { get; set; }

        public int IsAvailable { get; set; }

        public int IsComplete { get; set; }

        public string EmergencyPhone { get; set; }

        public string EmergencyPhoneExt { get; set; }

        public string Code { get; set; }

        public string CustomerCode { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
