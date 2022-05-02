import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { DailyWoReportByOperationsManagerGridModel } from '@app/core/models/work-order/wo-daily-report-operations-manager-grid.model';
import { BaseListService } from '@app/core/services/base-list.service';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class PublicDailyReportService extends BaseListService<DailyWoReportByOperationsManagerGridModel> {

  dailyReportOmDateFrom = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');
  dailyReportOmdateTo = moment(moment().add(-1, 'd').toDate()).format('YYYY-MM-DD');

  selectedOperationsManager: any = {};
  // user who generated the report
  loggedUser: any = {};

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'workorders', http);
  }

  getReport(
    loggedEmployeeGuid,
    operationsManagerGuid,
    params: { [key: string]: string } = {},
  ): Promise<any> {
    return Promise.all([
      this.getLoggedUser(loggedEmployeeGuid),
      this.getOperationsManager(operationsManagerGuid),
      this.getWokOrderList('ReadDailyReportByOperationsManager', '', '', '', 0, 100, params)]).then();
  }

  getOperationsManager(guid: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.apiBaseUrl}api/employees/publicget/${guid}`).subscribe((response: any) => {
        this.selectedOperationsManager = response;
        resolve(response);
      },
        (error) => reject(error));
    });
  }

  getLoggedUser(guid: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get(`${this.apiBaseUrl}api/employees/publicget/${guid}`).subscribe((response: any) => {
        this.loggedUser = response;
        resolve(response);
      },
        (error) => reject(error));
    });
  }

  getWokOrderList(
    action = 'ReadDailyReportByOperationsManager',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 50,
    params: { [key: string]: string } = {}
  ): Promise<any> {

    if (params.hasOwnProperty('dateFrom')) {
      this.dailyReportOmDateFrom = params['dateFrom'];
    }
    if (params.hasOwnProperty('dateTo')) {
      this.dailyReportOmdateTo = params['dateTo'];
    }

    return new Promise((resolve, reject) => {
      this.getAll(action, filter, sortField, sortOrder, pageNumber, pageSize, params)
        .subscribe((response: { count: number, payload: DailyWoReportByOperationsManagerGridModel[] }) => {
          this.allElementsToList = response.payload;
          this.elementsCount = response.count;
          this.allElementsChanged.next(this.allElementsToList);

          resolve(response);
        },
          (error) => {
            reject(error);
            this.loadingSubject.next(false);
          },
          () => {
            this.loadingSubject.next(false);
          }
        );

    });
  }

}
