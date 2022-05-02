using MGCap.Domain.ViewModels.Contact;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.ViewModels.Building
{
    public class BuildingReportGridViewModel : BuildingGridViewModel
    {
        public string EmergencyNotes { get; set; }

        public string ManagementCompanyFullName { get; set; }

        public IEnumerable<ContactGridViewModel> Contacts { get; set; }
    }
}
