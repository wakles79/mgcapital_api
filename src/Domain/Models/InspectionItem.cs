using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MGCap.Domain.Models
{
    public class InspectionItem : AuditableEntity
    {
        public int Number { get; set; }

        public string Position { get; set; }

        public string Description { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public WorkOrderPriority Priority { get; set; }

        public WorkOrderType Type { get; set; }

        public int InspectionId { get; set; }

        [ForeignKey("InspectionId")]
        public Inspection Inspection { get; set; }

        public ICollection<InspectionItemAttachment> Attachments { get; set; }

        public ICollection<InspectionItemTask> Tasks { get; set; }

        public int? Status { get; set; }

        public ICollection<InspectionItemNote> Notes { get; set; }

        public InspectionItem()
        {
            this.Attachments = new HashSet<InspectionItemAttachment>();
            this.Tasks = new HashSet<InspectionItemTask>();
            this.Notes = new HashSet<InspectionItemNote>();
        }
    }
}
