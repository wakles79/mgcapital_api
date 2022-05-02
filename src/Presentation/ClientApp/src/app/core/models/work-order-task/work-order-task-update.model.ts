import { WorkOrderTaskModel } from '@app/core/models/work-order/work-order-task.model';
import { WorkOrderTaskAttachmentModel } from './work-order-task-attachment.model';

export class WorkOrderTaskUpdateModel extends WorkOrderTaskModel {

  attachments: WorkOrderTaskAttachmentModel[] = [];

  constructor(taskUpdateModel) {
    super(taskUpdateModel);

    this.attachments = taskUpdateModel.attachments || [];
  }
}
