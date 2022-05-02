import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { CleaningReportDetailsModel } from '@app/core/models/reports/cleaning-report/cleaning.report.details.model';
import { BehaviorSubject, Subscription, Observable } from 'rxjs';
import { CLEANING_REPORT_ITEM_TYPES } from '@app/core/models/reports/cleaning-report/item-type.model';
import { CleaningReportSendEmailModel } from '@app/core/models/reports/cleaning-report/cleaning.report-send-email.model';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CleaningReportDetailsService extends BaseListService<CleaningReportDetailsModel>  {

  cleaningReportDetails: CleaningReportDetailsModel;
  onCleaningReportDetailsChange: BehaviorSubject<CleaningReportDetailsModel> = new BehaviorSubject<CleaningReportDetailsModel>(null);

  onCleaningReportItemsChange: BehaviorSubject<any> = new BehaviorSubject<any>([]);
  cleaningItems: any[] = [];

  onCleaningReportFindingsChange: BehaviorSubject<any> = new BehaviorSubject<any>([]);
  findingItems: any[] = [];

  onNotesChange: BehaviorSubject<any> = new BehaviorSubject<any>([]);
  notes: any[] = [];

  itemTypes: any[] = CLEANING_REPORT_ITEM_TYPES;

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'cleaningreports', http);
  }

  getDetails(reportId: number): Promise<any> {
    const params = new HttpParams()
      .set('cleaningreportid', reportId.toString());
    return this.getReportData('getcleaningreportdetails', params);
  }

  getPublicDetails(guid: string): Promise<any> {
    const params = new HttpParams()
      .set('guid', guid);
    return this.getReportData('publicgetcleaningreport', params);
  }

  private getReportData(action: string, params: HttpParams): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: params })
        .subscribe((response: any) => {
          resolve(response);

          this.cleaningReportDetails = response;
          this.onCleaningReportDetailsChange.next(this.cleaningReportDetails);

          this.cleaningItems = this.cleaningReportDetails.cleaningItems || [];
          this.onCleaningReportItemsChange.next(this.cleaningItems);

          this.findingItems = this.cleaningReportDetails.findingItems || [];
          this.onCleaningReportFindingsChange.next(this.findingItems);

          this.notes = this.cleaningReportDetails.notes || [];
          this.onNotesChange.next(this.notes);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createCleaningReportNote(cleaningReportNote: any, action = 'addNote'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(cleaningReportNote, action).subscribe(response => {
        resolve(response);
        this.getDetails(this.cleaningReportDetails.id);
      },
        (error) => {
          reject(error);
        });
    });
  }

  createCleaningReportPublicNote(cleaningReportNote, action = 'addNote'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(cleaningReportNote, action).subscribe(response => {
        resolve(response);
        this.getPublicDetails(this.cleaningReportDetails.guid);
      },
        (error) => {
          reject(error);
        });
    });
  }

  createCleaningReportItem(cleaningReportItem, action = 'add'): Promise<any> {
    /* const itemType = this.getCleaningReportItemType(cleaningReportItem); */

    return new Promise((resolve, reject) => {
      this.create(cleaningReportItem, action)
        .subscribe(response => {

          resolve(response);
          this.getDetails(this.cleaningReportDetails.id);
          // this.getCleaningReportItems(itemType, cleaningReportItem.cleaningReportId);
        },
          (error) => {
            reject(error);
          });
    });
  }

  updateCleaningReportItem(cleaningReportItem, action = 'update'): Promise<any> {
    /* const itemType = this.getCleaningReportItemType(cleaningReportItem); */

    return new Promise((resolve, reject) => {
      this.update(cleaningReportItem, action)
        .subscribe(response => {

          resolve(response);
          this.getDetails(this.cleaningReportDetails.id);
          // this.getCleaningReportItems(itemType, cleaningReportItem.cleaningReportId);
        },
          (error) => {
            reject(error);
          });
    });
  }

  deleteCleaningReportItem(cleaningReportItemToDelete, action = 'delete'): Promise<any> {
    /* const itemType = this.getCleaningReportItemType(cleaningReportItemToDelete);
    let element;
    let elementIndex; */

    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/${cleaningReportItemToDelete.id}`, { observe: 'response' })
        .subscribe((response: any) => {

          resolve(response);

          if (response.status === 200) {
            this.getDetails(this.cleaningReportDetails.id);

            /* if (itemType.value === 'cleaningItem') {
              element = this.cleaningItems.find(e => e.id === cleaningReportItemToDelete.id);
              elementIndex = this.cleaningItems.indexOf(element);
              this.cleaningItems.splice(elementIndex, 1);
              this.onCleaningReportItemsChange.next(this.cleaningItems);
            }
            else {
              element = this.findingItems.find(e => e.id === cleaningReportItemToDelete.id);
              elementIndex = this.findingItems.indexOf(element);
              this.findingItems.splice(elementIndex, 1);
              this.onCleaningReportFindingsChange.next(this.findingItems);
            } */
          }
        },
          (error) => {
            reject(error.error);
          });
    });

  }

  getCleaningReportItemType(cleaningReportItem): any {
    return this.itemTypes.find(item => item.id === cleaningReportItem.type);
  }

  getCleaningReportItems(itemType: any, cleaningReportId: any): any {
    this.loadingSubject.next(true);

    const queryParams = new HttpParams()
      .set('type', itemType.id.toString())
      .set('cleaningReportId', cleaningReportId.toString());

    return this.http.get(`${this.fullUrl}/GetCleaningReportItemsDetails`, { params: queryParams })
      .subscribe((response: any) => {

        if (itemType.value === 'cleaningItem') {
          this.cleaningItems = response;
          this.onCleaningReportItemsChange.next(this.cleaningItems);
        }
        else {
          this.findingItems = response;
          this.onCleaningReportFindingsChange.next(this.findingItems);
        }
        this.loadingSubject.next(false);

      },
        (error) => {
          this.loadingSubject.next(false);
        },
        () => {
          this.loadingSubject.next(false);
        }
      );
  }

  uploadFile(filesToUpload: any): Observable<any> {
    const formData: FormData = new FormData();
    for (let i = 0; i < filesToUpload.length; i++) {
      formData.append('woImageAttachment', filesToUpload[i], filesToUpload[i].name);
    }
    return this.http.post(`${this.apiBaseUrl}api/files/uploadattachments`, formData, { observe: 'response' });
  }

  deleteAttachmentByBlobName(blobName): Observable<any> {
    return this.http.delete(`${this.apiBaseUrl}api/files/DeleteAttachmentByBlobName`, blobName);
  }

  sendByEmailCleaningReportLink(cleaningReportToSend: CleaningReportSendEmailModel): Promise<any> {

    return new Promise((resolve, reject) => {
      this.http.post(`${this.fullUrl}/sendCleaningReport`, cleaningReportToSend, { observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getCleaningReportLogActivity(
    cleaningReportId: number,
    filter = 'id', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: any } = {},
    action = 'getallactivitylog'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + cleaningReportId, {
      params: queryParams
    });
  }
}
