import { Injectable, Inject } from '@angular/core';
import { DataService } from '@app/core/services/data.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FilesService extends DataService {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string, http: HttpClient) {
    super(apiBaseUrl, 'files', http);
  }

  uploadFile(filesToUpload: any): Observable<any> {
    const formData: FormData = new FormData();
    for (let i = 0; i < filesToUpload.length; i++) {
      formData.append('attachments', filesToUpload[i], filesToUpload[i].name);
    }
    return this.http.post(`${this.fullUrl}/uploadattachments`, formData, { observe: 'response' });
  }

  deleteAttachmentByBlobName(blobName): Observable<any> {
    return this.http.delete(`${this.fullUrl}/deleteAttachmentByBlobName`, blobName);
  }

}
