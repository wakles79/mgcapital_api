import { WorkOrderBaseModel, WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model';

export class WorkOrderCreateModel extends WorkOrderBaseModel {

  notes: any[];
  tasks: any[];
  attachments: any[];
  keepCloningReference: boolean;
  followUpOnClosingNotes: boolean;
  sendNotifications: boolean;
  scheduleSettings: any;
  assignedEmployees: any[];
  sourceCode: number;

  constructor(create: WorkOrderCreateModel) {
    super(create);

    this.notes = create.notes || [];
    this.tasks = create.tasks || [];
    this.attachments = create.attachments || [];
    this.keepCloningReference = create.keepCloningReference || false;
    this.followUpOnClosingNotes = create.followUpOnClosingNotes || false;
    this.sendNotifications = create.sendNotifications || false;
    this.scheduleSettings = create.scheduleSettings || null;
    this.assignedEmployees = create.assignedEmployees || [];
    this.sourceCode = create.sourceCode || WorkOrderSourceCode.Other; // EMail MG-23

  }
}
