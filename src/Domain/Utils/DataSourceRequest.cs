using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequest
    {
        public int PageNumber { get; set; } = 0;

        public int PageSize { get; set; } = 20;

        public string Filter { get; set; } = "";

        public string SortField { get; set; } = "";

        // FIXME: This is temporary
        public string SortOrder { get; set; } = "";

        /// <summary>
        ///  Gets or sets time offset in minutes
        /// </summary>
        public int TimezoneOffset { get; set; } = 300;

        /// <summary>
        ///     Gets or sets the starting date of the source filter
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        ///     Gets or sets the ending date of the source filter
        /// </summary>
        public DateTime? DateTo { get; set; }
    }
}
