using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Department
{
    public class DepartmentGridViewModel : DepartmentBaseViewModel, IGridViewModel
    {
        public DateTime CreatedDate { get; set; }

        public int EpochCreatedDate => this.CreatedDate.ToEpoch();

        public Guid Guid { get; set; }

        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
