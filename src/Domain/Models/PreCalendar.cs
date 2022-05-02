using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class PreCalendar : AuditableCompanyEntity
    {
        public int Quantity { get; set; }

        public CalendarPeriodicity Periodicity { get; set; }

        public CalendarEventType Type { get; set; }

        public DateTime? SnoozeDate { get; set; }

        public string Description { get; set; }

        public int? BuildingId { get; set; }

        public int? EmployeeId { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public ICollection<PreCalendarTask> Tasks { get; set; }

        public PreCalendar()
        {
            this.Tasks = new HashSet<PreCalendarTask>();
        }
    }
}
