using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestBuildingsByOperationsManager : DataSourceRequest
    {
        public int? CurrentEmployeeId { get; set; }

        public int? OperationsManagerId { get; set; }
    }
}
