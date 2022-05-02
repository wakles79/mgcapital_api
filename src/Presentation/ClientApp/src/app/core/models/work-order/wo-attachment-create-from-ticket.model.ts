import { AttachmentBaseModel } from '@app/core/models/common/attachment-base.model';

export class WoAttachmentCreateFromTicketModel extends AttachmentBaseModel {

  employeeId: number;
  imageTakenDate: Date;

  constructor(attachment: WoAttachmentCreateFromTicketModel = null) {
    super(attachment);
    if (attachment) {
      this.employeeId = attachment.employeeId;
      this.imageTakenDate = attachment.imageTakenDate || new Date(0);
    }
    else {
      this.employeeId = null;
      this.imageTakenDate = new Date(0);
    }
  }
}
