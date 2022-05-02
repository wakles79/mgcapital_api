using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Directory
{
    public class DirectoryChildBaseViewModel : EntityViewModel
    {
        private string _data;

        public string Label { get; set; }

        public string Data { get => _data.RemoveDuplicatedSpaces(); set => _data = value; }
    }
}
