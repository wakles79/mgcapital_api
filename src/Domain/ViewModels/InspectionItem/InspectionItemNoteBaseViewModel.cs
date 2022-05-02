using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MGCap.Domain.ViewModels.InspectionItem
{
    public class InspectionItemNoteBaseViewModel : AuditableEntityViewModel
    {
            public int InspectionItemId { get; set; }

            [Required]
            public string Note { get; set; }

            [Required]
            public int EmployeeId { get; set; }

            public string EmployeeEmail { get; set; }

            public string EmployeeFullName { get; set; }
    }
}
