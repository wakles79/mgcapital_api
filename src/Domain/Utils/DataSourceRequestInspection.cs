using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestInspection : DataSourceRequest
    {
        public int? LoggedEmployeId { get; set; }

        public bool? ShowSnoozed { get; set; }

        public DateTime? BeforeSnoozeDate { get; set; }
    }
}
