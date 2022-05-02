import { CompanyEntity } from '@core/models/common/company-entity.model';

export class WorkOrderBillableReportGridModel extends CompanyEntity {

  number: number;
  location: string;
  clonePath: string;
  originworkOrderId: number;
  workOrderId: number;
  workOrderGuid: string;

  buildingName: string;
  buildingBillingFullName: string;
  buildingBillingEmail: string;
  buildingNoteToBilling: string;

  taskName: string;
  taskCreatedDate: Date;
  epochTaskCreatedDate: number;
  workOrderCreatedDate: Date;
  epochWorkOrderCreatedDate: number;
  isTaskChecked: boolean;
  workOrderCompletedDate: Date;
  completedDate: Date;
  epochWorkOrderCompletedDate: number;
  epochTaskCompletedDate: number;
  taskUnitPrice: number;
  taskNote: string;
  closingNotes: string;
  serviceName: string;
  /*   serviceQuantity: number;
    servicePrice: number;
    serviceTotal: number; */
  oldVersion: boolean;
  ticketId: number;
  billingNote: string; // hce013

  constructor(workOrderTask: WorkOrderBillableReportGridModel) {
    super(workOrderTask);

    this.number = workOrderTask.number || 0;
    this.location = workOrderTask.location || '';
    this.clonePath = workOrderTask.clonePath || '';
    this.workOrderId = workOrderTask.workOrderId;
    this.workOrderGuid = workOrderTask.workOrderGuid || '';
    this.originworkOrderId = workOrderTask.originworkOrderId || null;

    this.buildingName = workOrderTask.buildingName || '';
    this.buildingBillingFullName = workOrderTask.buildingBillingFullName || '';
    this.buildingBillingEmail = workOrderTask.buildingBillingEmail || '';
    this.buildingNoteToBilling = workOrderTask.buildingNoteToBilling || '';

    this.taskName = workOrderTask.taskName || '';
    this.taskCreatedDate = workOrderTask.taskCreatedDate || null;
    this.epochTaskCreatedDate = workOrderTask.epochTaskCreatedDate || 0;
    this.workOrderCreatedDate = workOrderTask.workOrderCreatedDate || null;
    this.epochWorkOrderCreatedDate = workOrderTask.epochWorkOrderCreatedDate || 0;
    this.isTaskChecked = workOrderTask.isTaskChecked;
    this.workOrderCompletedDate = workOrderTask.workOrderCompletedDate || null;
    this.completedDate = workOrderTask.completedDate || new Date(0);
    this.epochTaskCompletedDate = workOrderTask.epochTaskCompletedDate || null;
    this.epochWorkOrderCompletedDate = workOrderTask.epochWorkOrderCompletedDate || 0;
    this.taskUnitPrice = workOrderTask.taskUnitPrice || 0;
    this.taskNote = workOrderTask.taskNote || '';
    this.closingNotes = workOrderTask.closingNotes || '';
    this.serviceName = workOrderTask.serviceName || '';
    /*     this.serviceQuantity = workOrderTask.serviceQuantity || 0;
        this.servicePrice = workOrderTask.servicePrice || 0;
        this.serviceTotal = workOrderTask.serviceTotal || 0; */
    this.oldVersion = workOrderTask.oldVersion || false;
    this.ticketId = workOrderTask.ticketId || 0;
    this.billingNote = workOrderTask.billingNote || ''; // hce013
  }
}
