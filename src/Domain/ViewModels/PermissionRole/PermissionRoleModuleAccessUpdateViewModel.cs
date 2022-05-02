using MGCap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.PermissionRole
{
    public class PermissionRoleModuleAccessUpdateViewModel
    {
        public int RoleId { get; set; }

        public ApplicationModule Module { get; set; }

        public AccessType Type { get; set; }
    }
}
