using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class CustomerCleaningReportDataSourceRequest : DataSourceRequest
    {
        public int ContactId { get; set; }
    }
}
