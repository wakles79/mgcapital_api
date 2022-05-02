import { WorkOrderAttachmentBaseModel } from '@core/models/work-order/work-order-attachment-base.model';

export class WorkOrderAttachmentUpdateModel extends WorkOrderAttachmentBaseModel {

  constructor(woAttachment: WorkOrderAttachmentUpdateModel) {
    super(woAttachment);
  }
}
