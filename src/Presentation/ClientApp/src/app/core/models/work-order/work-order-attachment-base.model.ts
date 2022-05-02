import { CompanyEntity } from '@core/models/common/company-entity.model';
import { AttachmentBaseModel } from '@app/core/models/common/attachment-base.model';

export class WorkOrderAttachmentBaseModel extends AttachmentBaseModel {

  workOrderId: number;
  employeeId: number;
  imageTakenDate: string;

  constructor(woAttachment: WorkOrderAttachmentBaseModel = null) {
    super(woAttachment);

    if (woAttachment) {
      this.workOrderId = woAttachment.workOrderId || null;
      this.employeeId = woAttachment.employeeId || null;
      this.imageTakenDate = woAttachment.imageTakenDate || '';
    }
    else {
      this.workOrderId = null;
      this.employeeId = null;
      this.imageTakenDate = '';
    }

  }
}
