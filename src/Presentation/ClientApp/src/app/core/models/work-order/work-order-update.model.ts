import { WorkOrderBaseModel } from '@core/models/work-order/work-order-base.model';
import { WorkOrderTaskModel } from '@core/models/work-order/work-order-task.model';
import { WorkOrderNoteModel } from '@core/models/work-order/work-order-note.model';
import { WorkOrderAttachmentBaseModel } from '@app/core/models/work-order/work-order-attachment-base.model';

export class WorkOrderUpdateModel extends WorkOrderBaseModel {

  requesterFirstName: string;
  requesterLastName: string;
  requesterFullName: string;
  requesterEmail: string;
  requesterPhone: string;
  requesterExt: string;
  fullAddress: string;
  employees: { id: number, name: string, roleName: string, email: string, level: number }[];
  assignedEmployees: any[];

  updateTasks: boolean;

  constructor(workOrder: WorkOrderUpdateModel) {
    super(workOrder);

    this.requesterFirstName = workOrder.requesterFirstName || '';
    this.requesterLastName = workOrder.requesterLastName || '';
    this.requesterFullName = workOrder.requesterFullName || '';
    this.requesterEmail = workOrder.requesterEmail || '';
    this.requesterPhone = workOrder.requesterPhone || '';
    this.requesterPhone = workOrder.requesterPhone || '';
    this.fullAddress = workOrder.fullAddress || '';
    this.employees = workOrder.employees || [];
    this.assignedEmployees = workOrder.assignedEmployees || [];

    this.updateTasks = workOrder.updateTasks || true;
  }
}
