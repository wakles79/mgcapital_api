using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItemTask
{
    public class InspectionItemTaskBaseViewModel : AuditableEntityViewModel
    {
        public int InspectionItemId { get; set; }

        public bool IsComplete { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
