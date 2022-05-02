import { CompanyEntity } from '@core/models/common/company-entity.model';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';

export class WorkOrderListModel extends CompanyEntity {

  locationName: string;
  buildingName: string;
  description: string;
  // assignedEmployeeFullName: string;
  requesterFullName: string;
  // requesterEmail: string;
  // dateSubmitted: Date;
  epochDateSubmitted: number;
  // dueDate: Date;
  epochDueDate: number;
  statusId: any;
  notesCount: number;
  tasksCount: number;
  tasksDoneCount: number;
  tasksBillableCount: number;
  isExpired: number;
  sequenceId: number;
  sequencePosition: number;
  elementsInSequence: number;
  ticketId?: number;

  constructor(workOrder: WorkOrderListModel) {
    super(workOrder);
    this.locationName = workOrder.locationName || '';
    this.buildingName = workOrder.buildingName || '';
    this.description = workOrder.description || '';
    this.statusId = workOrder.statusId || -1;
    this.requesterFullName = workOrder.requesterFullName || '';
    // this.requesterEmail = workOrder.requesterEmail || '';
    // this.dueDate = workOrder.dueDate || null;
    this.epochDueDate = workOrder.epochDueDate || 0;
    // this.dateSubmitted = workOrder.dateSubmitted || null;
    this.epochDateSubmitted = workOrder.epochDateSubmitted || 0;
    this.statusId = workOrder.statusId || 0;
    this.notesCount = workOrder.notesCount || 0;
    this.tasksCount = workOrder.tasksCount || 0;
    this.tasksDoneCount = workOrder.tasksDoneCount || 0;
    this.tasksBillableCount = workOrder.tasksBillableCount || 0;
    this.isExpired = workOrder.isExpired || 0;
    this.sequenceId = workOrder.sequenceId || 0;
    this.sequencePosition = workOrder.sequencePosition || 0;
    this.elementsInSequence = workOrder.elementsInSequence || 0;
    this.ticketId = workOrder.ticketId;

  }
}
