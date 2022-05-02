import { WorkOrderAttachmentBaseModel } from '@core/models/work-order/work-order-attachment-base.model';

export class WorkOrderAttachmentCreateModel extends WorkOrderAttachmentBaseModel {

  constructor(woAttachment: WorkOrderAttachmentCreateModel) {
    super(woAttachment);
  }
}
