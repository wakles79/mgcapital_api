import { WorkOrderBaseModel } from '@core/models/work-order/work-order-base.model';
import { WorkOrderTaskModel } from '@core/models/work-order/work-order-task.model';
import { WorkOrderNoteModel } from '@core/models/work-order/work-order-note.model';
import { WorkOrderAttachmentBaseModel } from '@app/core/models/work-order/work-order-attachment-base.model';

export class WorkOrderDetailsModel extends WorkOrderBaseModel {

  buildingName: string;
  tasks: WorkOrderTaskModel[];
  notes: WorkOrderNoteModel[];
  attachments: WorkOrderAttachmentBaseModel [];
  createdDate: Date;

  constructor(workOrder: WorkOrderDetailsModel) {
    super(workOrder);
    this.buildingName = workOrder.buildingName || '';
    this.tasks = workOrder.tasks || [];
    this.notes = workOrder.notes || [];
    this.attachments = workOrder.attachments || [];
    this.createdDate = workOrder.createdDate || null;
  }
}
