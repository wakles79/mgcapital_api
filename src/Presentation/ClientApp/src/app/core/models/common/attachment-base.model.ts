
export class AttachmentBaseModel {

  id: number;
  guid: string;
  blobName: string;
  fullUrl: string;
  description: string;

  get src() {
    return this.fullUrl;
  }

  constructor(attachment: AttachmentBaseModel = null) {
    if (attachment) {
      this.id = attachment.id || -1;
      this.guid = attachment.guid;
      this.blobName = attachment.blobName || '';
      this.fullUrl = attachment.fullUrl || '';
      this.description = attachment.description || '';
    }
    else {
      this.id = -1;
      this.guid = '';
      this.blobName = '';
      this.fullUrl = '';
      this.description = '';
    }
  }
}
