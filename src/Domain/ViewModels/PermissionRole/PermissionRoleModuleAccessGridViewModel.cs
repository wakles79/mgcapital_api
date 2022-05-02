using MGCap.Domain.Enums;
using MGCap.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PermissionRole
{
    public class PermissionRoleModuleAccessGridViewModel
    {
        public ApplicationModule Module { get; set; }

        public string ModuleName => this.Module.ToString().SplitCamelCase();

        public AccessType Type { get; set; }
    }
}
