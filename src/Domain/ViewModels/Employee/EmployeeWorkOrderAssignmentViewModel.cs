using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.Employee
{
    public class EmployeeWorkOrderAssignmentViewModel : EntityViewModel
    {
        public string Name { get; set; }

        public WorkOrderEmployeeType Type { get; set; }
    }
}
