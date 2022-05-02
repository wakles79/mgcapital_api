using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Permission;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PermissionRole
{
    public class PermissionRoleModuleViewModel : EntityViewModel
    {
        public ApplicationModule Module { get; set; }

        public string ModuleName => this.Module.ToString().SplitCamelCase();

        public IEnumerable<PermissionAssignmentViewModel> Permissions { get; set; }

        public PermissionRoleModuleViewModel()
        {
            this.Permissions = new HashSet<PermissionAssignmentViewModel>();
        }
    }
}
