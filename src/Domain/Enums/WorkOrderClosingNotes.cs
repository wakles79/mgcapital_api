using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum WorkOrderClosingNotes
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 4,
        Fourth = 8,
        Fifth = 16,
        Sixth = 32
    }
}
