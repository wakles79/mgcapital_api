using MGCap.Domain.Enums;
using MGCap.Domain.ViewModels.Employee;
using MGCap.Domain.ViewModels.WorkOrderScheduleSetting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderCreateViewModel : WorkOrderWithRequesterViewModel
    {
        public virtual IEnumerable<WorkOrderNoteCreateViewModel> Notes { get; set; }

        public virtual IList<WorkOrderTaskCreateViewModel> Tasks { get; set; }

        public virtual IEnumerable<WorkOrderAttachmentCreateViewModel> Attachments { get; set; }

        public bool KeepCloningReference { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating if the closing notes will have a different treatment
        /// </summary>
        public bool? FollowUpOnClosingNotes { get; set; }

        public bool SendNotifications { get; set; }

        public WorkOrderScheduleSettingCreateViewModel ScheduleSettings { get; set; }

        public IEnumerable<EmployeeWorkOrderAssignmentViewModel> AssignedEmployees { get; set; }

        public WorkOrderSourceCode SourceCode { get; set; }

        public WorkOrderCreateViewModel()
        {
            SendNotifications = true;
            this.AssignedEmployees = new HashSet<EmployeeWorkOrderAssignmentViewModel>();
        }
    }
}
