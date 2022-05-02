using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryEntityBaseViewModel : EntityViewModel
    {
        private string _name;

        public string Name { get => _name.RemoveDuplicatedSpaces(); set => _name = value; }
    }
}
