using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Models
{
    public class CleaningReportNote : AuditableEntity
    {
        public int CleaningReportId { get; set; }

        public CleaningReportNoteDirection Direction { get; set; }

        public int? SenderId { get; set; }

        public string Message { get; set; }

        public override void BeforeCreate(string userEmail = "", int companyId = -1)
        {
            base.BeforeCreate(userEmail, companyId);

            if (SenderId.HasValue.Equals(false))
            {
                this.CreatedBy = "Customer";
            }
        }
    }
}
