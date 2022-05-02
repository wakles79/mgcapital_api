import { CompanyEntity } from '@core/models/common/company-entity.model';
import { WorkOrderTaskModel } from '@app/core/models/work-order/work-order-task.model';
import { WorkOrderNoteModel } from '@app/core/models/work-order/work-order-note.model';
import { WorkOrderAttachmentBaseModel } from '@app/core/models/work-order/work-order-attachment-base.model';

export class WorkOrderBaseModel {
  id: number;
  guid: string;
  buildingId: number;
  location: string;
  administratorId: number;
  assignedEmployeeId: number;
  customerContactId: number;
  priority: number;
  sendRequesterNotifications: any;
  sendPropertyManagersNotifications: any;

  requesterFullName: string;
  requesterEmail: string;

  statusId: number;
  number: number;
  description: string;
  dueDate: Date;
  epochDueDate: number;
  type: number;
  workOrderSourceId: number;
  workOrderSourceCode: number;

  billingName: string;
  billingEmail: string;
  billingNote: string;

  closingNotes: string;
  additionalNotes: number;
  closingNotesOther: string;
  followUpOnClosingNotes: boolean;

  originWorkOrderId: number;
  keepCloningReference: boolean;
  clonePath: string;
  originWorkOrderNumber: string;

  tasks: WorkOrderTaskModel[];
  notes: WorkOrderNoteModel[];
  attachments: any[];

  snoozeDate: Date;
  epochSnoozeDate: number;
  billingDateType: number;

  //#region  Schedule Setting
  clientApproved: any;
  scheduleDateConfirmed: any;
  scheduleDate: Date;
  scheduleCategoryId: number;
  scheduleSubCategoryId: number;
  epochScheduleDate: number;
  calendarItemFrequencyId: number;
  unscheduled: boolean;

  workOrderScheduleSettingId: number;
  //#endregion Schedule Setting

  sendNotifications: boolean;

  public constructor(workOrder: WorkOrderBaseModel = null) {
    if (workOrder) {
      this.id = workOrder.id;
      this.guid = workOrder.guid;
      this.buildingId = workOrder.buildingId || null;
      this.location = workOrder.location || '';
      this.administratorId = workOrder.administratorId || null;
      this.assignedEmployeeId = workOrder.assignedEmployeeId || null;
      this.customerContactId = workOrder.customerContactId || null;
      this.requesterFullName = workOrder.requesterFullName || '';
      this.requesterEmail = workOrder.requesterEmail || '';
      this.priority = workOrder.priority || 0;
      this.sendRequesterNotifications = workOrder.sendRequesterNotifications || false;
      this.sendPropertyManagersNotifications = workOrder.sendPropertyManagersNotifications || false;
      this.statusId = workOrder.statusId || 0;
      this.number = workOrder.number || 0;
      this.description = workOrder.description || '';
      this.dueDate = workOrder.dueDate || null;
      this.epochDueDate = workOrder.epochDueDate || 0;
      this.type = workOrder.type || 0;
      this.workOrderSourceId = workOrder.workOrderSourceId || null;
      this.workOrderSourceCode = workOrder.workOrderSourceCode || null;
      this.billingName = workOrder.billingName || '';
      this.billingEmail = workOrder.billingEmail || '';
      this.billingNote = workOrder.billingNote || '';
      this.closingNotes = workOrder.closingNotes || '';
      this.additionalNotes = workOrder.additionalNotes || 0;
      this.closingNotesOther = workOrder.closingNotesOther;
      this.followUpOnClosingNotes = workOrder.followUpOnClosingNotes;
      this.originWorkOrderId = workOrder.originWorkOrderId || null;
      this.keepCloningReference = workOrder.keepCloningReference;
      this.clonePath = workOrder.clonePath || '';
      this.originWorkOrderNumber = workOrder.originWorkOrderNumber || '';
      this.tasks = workOrder.tasks || [];
      this.notes = workOrder.notes || [];
      this.attachments = workOrder.attachments || [];
      this.snoozeDate = workOrder.snoozeDate || null;
      this.epochSnoozeDate = workOrder.epochSnoozeDate || 0;
      this.billingDateType = workOrder.billingDateType || 0;


      this.clientApproved = workOrder.clientApproved || false;
      this.scheduleDateConfirmed = workOrder.scheduleDateConfirmed || false;
      this.scheduleDate = workOrder.scheduleDate || null;
      this.scheduleCategoryId = workOrder.scheduleCategoryId || null;
      this.scheduleSubCategoryId = workOrder.scheduleSubCategoryId || null;
      this.epochScheduleDate = workOrder.epochScheduleDate || 0;
      this.calendarItemFrequencyId = workOrder.calendarItemFrequencyId || 0;
      this.sendNotifications = workOrder.sendNotifications;
      this.unscheduled = workOrder.unscheduled || false;

      this.workOrderScheduleSettingId = workOrder.workOrderScheduleSettingId || null;
    }
    else {
      this.id = null;
      this.guid = null;
      this.buildingId = null;
      this.location = '';
      this.administratorId = null;
      this.assignedEmployeeId = null;
      this.customerContactId = null;
      this.requesterFullName = '';
      this.requesterEmail = '';
      this.priority = 0;
      this.sendRequesterNotifications = false;
      this.sendPropertyManagersNotifications = false;
      this.statusId = 0;
      this.number = 0;
      this.description = '';

      this.dueDate = new Date();
      this.dueDate.setHours(18);

      this.epochDueDate = 0;
      this.type = 0;
      this.workOrderSourceId = null;
      this.workOrderSourceCode = null;
      this.billingName = '';
      this.billingEmail = '';
      this.billingNote = '';
      this.closingNotes = '';
      this.closingNotesOther = '';
      this.originWorkOrderId = null;
      this.keepCloningReference = false;
      this.clonePath = '';
      this.originWorkOrderNumber = '';
      this.tasks = [];
      this.notes = [];
      this.attachments = [];
      this.followUpOnClosingNotes = false;
      this.snoozeDate = null;
      this.epochSnoozeDate = 0;
      this.billingDateType = 0;

      this.clientApproved = false;
      this.scheduleDateConfirmed = false;
      this.scheduleDate = null;
      this.scheduleCategoryId = null;
      this.scheduleSubCategoryId = null;
      this.epochScheduleDate = 0;
      this.calendarItemFrequencyId = 0;
      this.sendNotifications = true;
      this.unscheduled = false;
      this.workOrderScheduleSettingId = null;
    }
  }
}


export enum WorkOrderEmployeeType {
  Any = 0,
  Supervisor = 1,
  OperationsManager = 2,
  TemporaryOperationsManager = 4,
  Inspector = 8
}

export enum WorkOrderSourceCode {
  Email = 0,
  Phone = 1,
  InPerson = 2,
  Recurring = 3,
  Cloned = 4,
  InternalMobile = 10,
  ExternalMobile = 11,
  LandingPage = 12,
  Calendar = 13,
  Ticket = 14,
  Other = 16
}

