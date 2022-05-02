import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { InspectionDetailModel } from '@app/core/models/inspections/inspection-detail.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { InspectionSendEmailModel } from '@app/core/models/inspections/inspection-send-email.model';

@Injectable({
  providedIn: 'root'
})
export class InspectionDetailService extends BaseListService<InspectionDetailModel> {

  inspectionDetail: InspectionDetailModel;

  onInspectionDetailChanged: BehaviorSubject<InspectionDetailModel> = new BehaviorSubject<InspectionDetailModel>(null);

  inspectionItems: any[] = [];
  inspectionNotes: any[] = [];
  onInspectionItemsChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  onInspectionNotesChanged: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'inspections', http);
  }

  getDetails(inspectionId: number): Promise<any> {
    const params = new HttpParams()
      .set('id', inspectionId.toString());
    return this.getInspectionDetailData('getInspectionDetails', params);
  }

  getPublicDetails(inspectionGuid: string): Promise<any> {
    const params = new HttpParams()
      .set('guid', inspectionGuid);

    return this.getInspectionDetailData('publicgetinspectiondetails', params);
  }

  getDetailsItem(inspectionItemId: number): Promise<any> {
    const params = new HttpParams()
      .set('id', inspectionItemId.toString());
    return this.getInspectionItemDetailData('GetInspectionItemDetails', params);
  }

  getItemAttachment(InspectionId: number): void {
  }

  createInspectionItem(inspectionItem, action = 'add'): Promise<any> {
    /* const itemType = this.getCleaningReportItemType(cleaningReportItem); */

    return new Promise((resolve, reject) => {
      this.create(inspectionItem, action)
        .subscribe(response => {

          resolve(response);
          this.getDetails(this.inspectionDetail.id);
          // this.getCleaningReportItems(itemType, cleaningReportItem.cleaningReportId);
        },
          (error) => {
            reject(error);
          });
    });
  }

  private getInspectionDetailData(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: params })
        .subscribe((response: any) => {
          resolve(response);

          this.inspectionDetail = response;
          this.onInspectionDetailChanged.next(this.inspectionDetail);

          this.inspectionItems = this.inspectionDetail.inspectionItem;
          this.inspectionNotes = this.inspectionDetail.InspectionNote;
          this.onInspectionItemsChanged.next(this.inspectionItems);
          this.onInspectionNotesChanged.next(this.inspectionNotes);

        }, (error) => { reject(error); });
    });
  }

  private getInspectionItemDetailData(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: params })
        .subscribe((response: any) => {
          resolve(response);
          this.inspectionItems = response;
        }, (error) => { reject(error); });
    });
  }

  uploadFile(filesToUpload: any): Observable<any> {
    const formData: FormData = new FormData();
    for (let i = 0; i < filesToUpload.length; i++) {
      formData.append('iiImageAttachment', filesToUpload[i], filesToUpload[i].name);
    }
    return this.http.post(`${this.apiBaseUrl}api/files/uploadattachments`, formData, { observe: 'response' });
  }

  deleteAttachmentByBlobName(blobName): Observable<any> {
    return this.http.delete(`${this.fullUrl}/deleteAttachmentByBlobName`, blobName);
  }

  deleteInspectionItem(inspectionItemToDelete, action = 'delete'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/${inspectionItemToDelete.id}`)
        .subscribe((response: any) => {
          this.getDetails(this.inspectionDetail.id);
        },
          (error) => {
            reject(error.error);
          });
    });

  }

  updateInspectionItem(inspectionItem, action = 'update'): Promise<any> {
    /* const itemType = this.getCleaningReportItemType(cleaningReportItem); */

    return new Promise((resolve, reject) => {
      this.update(inspectionItem, action)
        .subscribe(response => {

          resolve(response);
          this.getDetails(this.inspectionDetail.id);
          // this.getCleaningReportItems(itemType, cleaningReportItem.cleaningReportId);
        },
          (error) => {
            reject(error);
          });
    });
  }

  closeInspectionItem(id: number, action = 'CloseInspectionItem'): Observable<any> {

    const queryParams = new HttpParams()
      .set('id', id.toString());


    return this.http.post(`${this.fullUrl}/${action}`, null, {
      params: queryParams
    });

  }

  getPdfReportUrl(id: number, action: string = 'getInspectionPdfUrl'): Observable<any> {
    const params = new HttpParams()
      .set('id', id.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: params });
  }

  sendInspectionByEmail(inspectionToSend: InspectionSendEmailModel): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.post(`${this.fullUrl}/sendInspectionByEmail`, inspectionToSend, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getInspectionActivityLog(
    inspectionId: number,
    filter = 'id', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'getActivityLog'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + inspectionId, {
      params: queryParams
    });

  }

  createInspectionNote(inspectionItem, action = 'add'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(inspectionItem, action)
        .subscribe(response => {
          resolve(response);
          this.getDetails(this.inspectionDetail.id);
        },
          (error) => {
            reject(error);
          });
    });
  }

  getInspectionNotes(
    inspectionId: number,
    filter = 'id', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'ReadAllNotes'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + inspectionId, {
      params: queryParams
    });

  }

  updateCompletedStatusToTask(id: number, isComplete: boolean, action = 'CloseInspectionItemTask'): Observable<any> {
    const queryParams = new HttpParams()
      .set('id', id.toString())
      .set('isCompleted', isComplete.toString());

    return this.http.post(`${this.fullUrl}/${action}`, null, {
      params: queryParams
    });
  }
}
