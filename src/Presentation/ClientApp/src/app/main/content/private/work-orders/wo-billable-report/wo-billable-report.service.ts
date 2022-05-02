import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { WorkOrderBillableReportGridModel } from '@app/core/models/work-order-billable-report/wo-billable-report-grid.model';
import { BaseListService } from '@app/core/services/base-list.service';
import * as moment from 'moment';
import { Subscription, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WoBillableReportService extends BaseListService<WorkOrderBillableReportGridModel> {

  dateFrom = moment().subtract(1, 'months').toDate();
  dateTo = moment().toDate();

  filterBy: { [key: string]: string } = {
    'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
    'dateTo': moment(this.dateTo).format('YYYY-MM-DD')
  };

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'workorders', http);
  }

  getElements(
    action = 'ReadBillingReport',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Subscription {
    if (!filter || filter === '') {
      filter = this.searchText;
    }

    this.loadingSubject.next(true);

    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    return this.getAll(action, filter, sortField, sortOrder, pageNumber, pageSize, extendedParams)
      .subscribe((response: { count: number, payload: WorkOrderBillableReportGridModel[] }) => {
        this.allElementsToList = response.payload;
        this.elementsCount = response.count;
        this.allElementsChanged.next(this.allElementsToList);
      },
        (error) => {
          this.loadingSubject.next(false);
        },
        () => {
          this.loadingSubject.next(false);
        }
      );
  }

  exportReportToCsv(
    action = 'readBillingReportCsv',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Promise<any> {

    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    const strFilter = (this.searchText === undefined || this.searchText === null) ? '' : this.searchText;

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', strFilter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, extendedParams);

    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: queryParams, observe: 'response', responseType: 'text' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getDocumentReportUrl(
    action = 'getBillingDocumentReportUrl',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Observable<any> {
    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', this.searchText ? this.searchText : '')
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, extendedParams);

    return this.http.get(`${this.fullUrl}/${action}`, { params: queryParams });
  }

  getReportBase64(
    action = 'getBillingReportDocumentBase64',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Promise<any> {

    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, extendedParams);

    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: queryParams, observe: 'response' })
        .subscribe((response: any) => {
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  getTask(id: any, action = 'GetWorkOrderTask'): Observable<any> {
    const queryParams = new HttpParams()
      .set('id', id);

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }

  uploadFile(filesToUpload: any): Observable<any> {
    const formData: FormData = new FormData();
    for (let i = 0; i < filesToUpload.length; i++) {
      formData.append('woImageAttachment', filesToUpload[i], filesToUpload[i].name);
    }
    return this.http.post(`${this.fullUrl}/uploadattachments`, formData, { observe: 'response' });
  }
}
