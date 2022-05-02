import { Injectable, Inject } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseListService } from '@core/services/base-list.service';
import { DailyWoReportByOperationsManagerGridModel } from '@app/core/models/work-order/wo-daily-report-operations-manager-grid.model';
import * as moment from 'moment';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DailyWoReportByOperationsManagerService extends BaseListService<DailyWoReportByOperationsManagerGridModel>
{
  onFilterChanged: Subject<any> = new Subject();

  filterBy: { [key: string]: string } = {
    operationsManagerId: null,
    'dateFrom': moment(this.dateFrom).format('YYYY-MM-DD'),
    'dateTo': moment(this.dateTo).format('YYYY-MM-DD')
  };

  onSelectedOperationsManagerChange: Subject<any> = new Subject();
  selectedOperationsManager: any = {};

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'workorders', http);

    this.onFilterChanged.subscribe((filterBy: { [key: string]: string }) => {
      this.filterBy = filterBy;
      this.getElements('ReadDailyReportByOperationsManager', '', '', '', 0, 50, {});
    });

    this.onSelectedOperationsManagerChange.subscribe((element: any) => {
      this.selectedOperationsManager = element;
    });
  }
}
