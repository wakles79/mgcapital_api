using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Permission
{
    public class PermissionBaseViewModel : EntityViewModel
    {
        public string Name { get; set; }

        public ApplicationModule Module { get; set; }

        public ActionType Type { get; set; }
    }
}
