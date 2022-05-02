import { EntityModel } from '@app/core/models/common/entity.model';

export class WorkOrderTaskAttachmentModel extends EntityModel {

  description: string;
  blobName: string;
  fullUrl: string;
  title: string;
  imageTakenDate: Date;
  workOrderTaskId: number;

  constructor(attachment) {
    super(attachment);

    this.description = attachment.description || '';
    this.blobName = attachment.blobName || '';
    this.fullUrl = attachment.fullUrl || '';
    this.title = attachment.title || '';
    this.imageTakenDate = attachment.imageTakenDate || null;
    this.workOrderTaskId = attachment.workOrderTaskId || 0;

  }
}
