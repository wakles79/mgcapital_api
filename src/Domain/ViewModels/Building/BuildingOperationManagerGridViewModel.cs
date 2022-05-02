using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingOperationManagerGridViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public BuildingEmployeeType Type { get; set; }

        public BuildingEmployeeType CurrentType { get; set; }

        public string CurrentTypeName => this.CurrentType.ToString().SplitCamelCase();

        public bool IsShared { get; set; }
    }
}
