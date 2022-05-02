using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Role
{
    public class RoleBaseViewModel : EntityViewModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public RoleType Type { get; set; }
    }
}
