using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using MGCap.Domain.ViewModels.Employee;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGCap.Domain.ViewModels.WorkOrder
{
    public class WorkOrderUpdateViewModel : WorkOrderWithRequesterViewModel, IEntityParentViewModel<WorkOrderNoteUpdateViewModel,
                                                                                                    WorkOrderAttachmentUpdateViewModel>
    {
        public string BuildingName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int EpochCreatedDate => this.CreatedDate.ToEpoch();

        public bool IsExpired { get; set; }

        public IEnumerable<WorkOrderTaskUpdateViewModel> Tasks { get; set; }

        public IEnumerable<WorkOrderNoteUpdateViewModel> Notes { get => _children1?.OrderBy(n => n.CreatedDate); set => _children1 = value as IList<WorkOrderNoteUpdateViewModel>; }

        public IEnumerable<WorkOrderAttachmentUpdateViewModel> Attachments { get => _children2?.OrderBy(a => a.ImageTakenDate); set => _children2 = value as IList<WorkOrderAttachmentUpdateViewModel>; }

        /// <summary>
        ///     Gets or sets a value indicating if the Work Order
        ///     has changed status during any given operation
        /// </summary>
        public bool HasChangedStatus { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating if the closing notes will have a different treatment
        /// </summary>
        public bool? FollowUpOnClosingNotes { get; set; } 

        protected IList<WorkOrderNoteUpdateViewModel> _children1;
        protected IList<WorkOrderAttachmentUpdateViewModel> _children2;

        public IList<WorkOrderNoteUpdateViewModel> Children1 { get => _children1; set => _children1 = value; }

        public IList<WorkOrderAttachmentUpdateViewModel> Children2 { get => _children2; set => _children2 = value; }

        public IEnumerable<EmployeeListBoxViewModel> Employees { get; set; }

        public IEnumerable<EmployeeWorkOrderAssignmentViewModel> AssignedEmployees { get; set; }

        /// <summary>
        /// Flag to determinate if update tasks, this is in case of update form detail view
        /// </summary>
        public bool UpdateTasks { get; set; } = false;

        public WorkOrderUpdateViewModel()
        {
            this.Employees = new HashSet<EmployeeListBoxViewModel>();

            this.AssignedEmployees = new HashSet<EmployeeWorkOrderAssignmentViewModel>();
            this.Tasks = new HashSet<WorkOrderTaskUpdateViewModel>();
        }
    }
}
