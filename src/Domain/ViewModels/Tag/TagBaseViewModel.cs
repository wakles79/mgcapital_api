using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Tag
{
    public class TagBaseViewModel : EntityViewModel
    {
        public string Description { get; set; }

        public TagType Type { get; set; }

        public string HexColor { get; set; }
    }
}
