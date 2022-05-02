import { AttachmentBaseModel } from '@app/core/models/common/attachment-base.model';

export class CleaningReportItemAttachmentCreateFromTicketModel extends AttachmentBaseModel {

  title: string;

  constructor(attachment: CleaningReportItemAttachmentCreateFromTicketModel = null) {
    super(attachment);
    if (attachment) {
      this.title = attachment.title;
    }
    else {
      this.title = '';
    }
  }
}
