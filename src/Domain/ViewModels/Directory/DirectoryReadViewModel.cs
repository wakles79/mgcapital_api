using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryReadViewModel : DirectoryEntityBaseViewModel
    {        
        public DirectoryContactType Type { get; set; }
    }
}
