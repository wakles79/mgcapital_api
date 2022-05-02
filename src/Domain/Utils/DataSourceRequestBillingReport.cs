using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestBillingReport : DataSourceRequest
    {
        public int? CustomerId { get; set; }

        public IEnumerable<int> BuildingIds { get; set; } = new HashSet<int>();

    }
}
