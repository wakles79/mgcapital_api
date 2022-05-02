using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CleaningReportItemAttachment : Attachment
    {

        public string Title { get; set; }

        public int CleaningReportItemId { get; set; }

        public DateTime ImageTakenDate { get; set; }

        [ForeignKey("CleaningReportItemId")]
        public CleaningReportItem CleaningReportItem { get; set; }
    }
}
