using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Permission
{
    public class PermissionAssignmentViewModel : PermissionBaseViewModel
    {
        public string FullName => this.Name.SplitCamelCase();

        public bool IsAssigned { get; set; }
    }
}
