using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Inspection
{
    public class InspectionNoteGridViewModel : InspectionNoteBaseViewModel
    {
        [IgnoreDataMember]
        public int Count { get; set; }
    }
}
