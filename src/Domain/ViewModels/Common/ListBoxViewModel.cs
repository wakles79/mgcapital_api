using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MGCap.Domain.Utils;

namespace MGCap.Domain.ViewModels.Common
{
    public class ListBoxViewModel
    {
        private string _name;

        public int ID { get; set; }

        public string Name { get => _name.RemoveDuplicatedSpaces(); set => _name = value; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
