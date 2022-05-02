using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestBudget : DataSourceRequest
    {
        public DateTime? UpdatedDateFrom { get; set; }

        public DateTime? UpdatedDateTo { get; set; }

        public int? BuildingId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
