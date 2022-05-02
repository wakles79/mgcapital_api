using MGCap.Domain.ViewModels.Common;

namespace MGCap.Domain.ViewModels.Employee
{
    public class EmployeeListBoxViewModel : ListBoxViewModel
    {
        public string RoleName { get; set; }

        public string Email { get; set; }

        public int Level { get; set; }
    }

    public class EmployeeWithRoleLevelListBoxViewModel : EmployeeListBoxViewModel
    {
        public int RoleLevel { get; set; }
    }
}
