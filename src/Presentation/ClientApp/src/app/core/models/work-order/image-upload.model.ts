export class ImageUploadViewModel{
  fileName: string;
  blobName: string;
  fullUrl: string;
  imageTakenDate: Date;

  constructor(imageUpload: ImageUploadViewModel) {
    this.blobName = imageUpload.blobName || '';
    this.fullUrl = imageUpload.fullUrl || '';
    this.imageTakenDate = imageUpload.imageTakenDate || null;
  }
}
