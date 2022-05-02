using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class DataSourceRequestCalendar : DataSourceRequest
    {
        public int? CustomerId { get; set; }

        public int? BuildingId { get; set; }

        public int? TypeId { get; set; }

        /// <summary>
        /// <para>0 => approved</para>
        /// <para>1 => unapproved</para>
        /// </summary>
        public int? ApprovedStatus { get; set; }

        /// <summary>
        /// <para>0 => confirmed</para>
        /// <para>1 => not confirmed</para>
        /// </summary>
        public int? ScheduleDateConfirmed { get; set; }

        /// <summary>
        /// <para>0 => no due date</para>
        /// <para>1 => has due date</para>
        /// </summary>
        public int? DueDateStatus { get; set; }

        public int[] ScheduleCategory { get; set; }

        public int[] ScheduleSubCategory { get; set; }
    }
}
