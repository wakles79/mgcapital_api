using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class CustomerWODataSourceRequest : DataSourceRequest
    {
        public int? BuildingId { get; set; }

        public string Statuses { get; set; }
    }
}
