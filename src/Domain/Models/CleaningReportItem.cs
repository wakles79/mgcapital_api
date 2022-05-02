using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CleaningReportItem : AuditableEntity
    {
        public int CleaningReportId { get; set; }

        [ForeignKey("CleaningReportId")]
        public CleaningReport CleaningReport { get; set; }

        public int BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        public Building Building { get; set; }

        [MaxLength(80)]
        public string Location { get; set; }

        [MaxLength(16)]
        public string Time { get; set; }

        public string Observances { get; set; }

        public CleaningReportType Type { get; set; }

        public ICollection<CleaningReportItemAttachment> CleaningReportItemAttachments { get; set; }

        public CleaningReportItem()
        {
            this.CleaningReportItemAttachments = new HashSet<CleaningReportItemAttachment>();
        }
    }
}
