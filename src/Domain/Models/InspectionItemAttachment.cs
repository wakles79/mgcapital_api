using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class InspectionItemAttachment : Attachment
    {
        public string Title { get; set; }

        public int InspectionItemId { get; set; }

        [ForeignKey("InspectionItemId")]
        public InspectionItem InspectionItem  { get; set; }

        public DateTime ImageTakenDate { get; set; }
    }
}
