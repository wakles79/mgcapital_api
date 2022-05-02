using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingCsvViewModel
    {      
        public string BuildingCode { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string CustomerCode { get; set; }

        public string OperationManagerName { get; set; }

        public string EmergencyPhone { get; set; }

        public string IsComplete { get; set; }

        public string IsActive { get; set; }
    }
}
