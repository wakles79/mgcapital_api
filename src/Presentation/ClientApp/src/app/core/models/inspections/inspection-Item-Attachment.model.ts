import { AttachmentBaseModel } from '@app/core/models/common/attachment-base.model';

export class InspectionItemAttachmentModel extends AttachmentBaseModel {

  cleaningReportItemId: number;
  title: string;

  constructor(entity: InspectionItemAttachmentModel) {
    super(entity);

    this.cleaningReportItemId = entity.cleaningReportItemId;
    this.title = entity.title;

  }
}
