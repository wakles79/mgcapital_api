using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Enums
{
    public enum CleaningReportType
    {
        Cleaning = 0,
        Findings = 1
    }

    public enum CleaningReportNoteDirection
    {
        Incoming = -1,
        Outgoing = 1
    }
}
