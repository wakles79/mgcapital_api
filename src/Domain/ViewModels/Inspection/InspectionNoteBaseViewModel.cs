using MGCap.Domain.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionNoteBaseViewModel : AuditableEntityViewModel
    {
            public int InspectionId { get; set; }
            [Required]
            public string Note { get; set; }
            [Required]
            public int EmployeeId { get; set; }
            public string EmployeeEmail { get; set; }
            public string EmployeeFullName { get; set; }        
    }
}
