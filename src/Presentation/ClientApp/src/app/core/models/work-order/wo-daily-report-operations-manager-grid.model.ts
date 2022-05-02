import { CompanyEntity } from '@core/models/common/company-entity.model';

export class DailyWoReportByOperationsManagerGridModel extends CompanyEntity {

  number: number;
  buildingName: string;
  location: string;
  description: string;
  operationsManagerFullname: string;
  requesterFullName: string;
  requesterEmail: string;
  dateSubmitted: Date;
  epochDateSubmitted: number;
  dueDate: Date;
  epochDueDate: number;
  statusId: any;
  notesCount: number;
  tasksCount: number;
  tasksDoneCount: number;
  isExpired: number;
  operationsManagerFullName: string;
  clonePath: string;
  attachmentsCount: number;
  originWorkOrderId: number;

  constructor(workOrder: DailyWoReportByOperationsManagerGridModel) {
    super(workOrder);
    this.number = workOrder.number || 0;
    this.location = workOrder.location || '';
    this.buildingName = workOrder.buildingName || '';
    this.description = workOrder.description || '';
    this.statusId = workOrder.statusId || -1;
    this.requesterFullName = workOrder.requesterFullName || '';
    this.requesterEmail = workOrder.requesterEmail || '';
    this.dueDate = workOrder.dueDate || null;
    this.epochDueDate = workOrder.epochDueDate || 0;
    this.dateSubmitted = workOrder.dateSubmitted || null;
    this.epochDateSubmitted = workOrder.epochDateSubmitted || 0;
    this.statusId = workOrder.statusId || 0;
    this.notesCount = workOrder.notesCount || 0;
    this.tasksCount = workOrder.tasksCount || 0;
    this.tasksDoneCount = workOrder.tasksDoneCount || 0;
    this.isExpired = workOrder.isExpired || 0;
    this.operationsManagerFullname = workOrder.operationsManagerFullname || '';
    this.clonePath = workOrder.clonePath || '';
    this.attachmentsCount = workOrder.attachmentsCount || 0;
    this.originWorkOrderId = workOrder.originWorkOrderId || null;
  }
}
