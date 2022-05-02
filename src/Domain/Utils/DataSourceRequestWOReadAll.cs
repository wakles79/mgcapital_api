using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestWOReadAll : DataSourceRequest
    {
        public int? EmployeeId { get; set; }

        public int? LoggedEmployeId { get; set; }

        /// <summary>
        /// Represents all possible status separated by colon.
        /// </summary>
        public string Statuses { get; set; }

        /// <summary>
        /// Gets or sets the status identifiers.
        /// </summary>
        /// <value>The status identifiers.</value>
        public IEnumerable<int> StatusIds { get; set; }

        /// <summary>
        /// Gets or sets the building identifiers.
        /// </summary>
        /// <value>The building identifiers.</value>
        public IEnumerable<int> BuildingIds { get; set; }

        public bool? IsExpired { get; set; }

        public bool? DueToday { get; set; }

        public bool? IsActive { get; set; }

        public IEnumerable<int> ServiceId { get; set; } = new HashSet<int>();

        public bool? IsBillable { get; set; }
    }
}
