import { AttachmentBaseModel } from '@app/core/models/common/attachment-base.model';

export class CleaningReportItemAttachmentModel extends AttachmentBaseModel {

  cleaningReportItemId: number;
  title: string;

  constructor(entity: CleaningReportItemAttachmentModel) {
    super(entity);

    this.cleaningReportItemId = entity.cleaningReportItemId;
    this.title = entity.title;

  }
}
