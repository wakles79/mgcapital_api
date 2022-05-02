using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Common
{
    public interface IGridViewModel
    {
        [IgnoreDataMember]
        int Count { get; set; }
    }
}
