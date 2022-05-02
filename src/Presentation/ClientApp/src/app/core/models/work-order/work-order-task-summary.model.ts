import { EntityModel } from '@app/core/models/common/entity.model';
import { WorkOrderTaskModel } from '@app/core/models/work-order/work-order-task.model';

export class WorkOrderTaskSummaryModel extends EntityModel {
  originWorkOrderId: string;
  clonePath: string;
  number: number;
  clientApproved: boolean;
  scheduleDateConfirmed: boolean;
  scheduleDate: Date;
  epochScheduleDate: number;
  dueDate: Date;
  epochDueDate: number;
  notesCount: number;
  tasksCount: number;
  tasksDoneCount: number;
  statusId: number;
  totalBill: number;
  billingDateType: number;
  billingDateTypeName: string;
  sendRequesterNotifications: boolean;
  sendPropertyManagersNotifications: boolean;
  isExpired: number;
  unscheduled: boolean;

  tasks: WorkOrderTaskModel[];

  sequenceId: number;
  sequencePosition: number;
  elementsInSequence: number;

  toEdit = false;

  constructor(workOrderTaskSummary: WorkOrderTaskSummaryModel) {
    super(workOrderTaskSummary);

    this.originWorkOrderId = workOrderTaskSummary.originWorkOrderId || '';
    this.clonePath = workOrderTaskSummary.clonePath || '';
    this.number = workOrderTaskSummary.number || 0;
    this.tasks = workOrderTaskSummary.tasks || [];
    this.clientApproved = workOrderTaskSummary.clientApproved || false;
    this.scheduleDateConfirmed = workOrderTaskSummary.scheduleDateConfirmed || false;
    this.scheduleDate = workOrderTaskSummary.scheduleDate || null;
    this.epochScheduleDate = workOrderTaskSummary.epochScheduleDate || 0;
    this.dueDate = workOrderTaskSummary.dueDate || null;
    this.epochDueDate = workOrderTaskSummary.epochDueDate || 0;
    this.notesCount = workOrderTaskSummary.notesCount || 0;
    this.tasksCount = workOrderTaskSummary.tasksCount || 0;
    this.tasksDoneCount = workOrderTaskSummary.tasksDoneCount || 0;
    this.statusId = workOrderTaskSummary.statusId || 0;
    this.totalBill = workOrderTaskSummary.totalBill || 0;
    this.billingDateType = workOrderTaskSummary.billingDateType || 0;
    this.billingDateTypeName = workOrderTaskSummary.billingDateTypeName || '';
    this.sendRequesterNotifications = workOrderTaskSummary.sendRequesterNotifications || false;
    this.sendPropertyManagersNotifications = workOrderTaskSummary.sendPropertyManagersNotifications || false;
    this.isExpired = workOrderTaskSummary.isExpired || 0;
    this.unscheduled = workOrderTaskSummary.unscheduled || false;

    this.sequenceId = workOrderTaskSummary.sequenceId || 0;
    this.sequencePosition = workOrderTaskSummary.sequencePosition || 0;
    this.elementsInSequence = workOrderTaskSummary.elementsInSequence || 0;
  }
}
