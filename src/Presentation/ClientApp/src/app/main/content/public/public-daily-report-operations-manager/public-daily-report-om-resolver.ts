import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DailyWoReportByOperationsManagerGridModel } from '@app/core/models/work-order/wo-daily-report-operations-manager-grid.model';
import { PublicDailyReportService } from './public-daily-report.service';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';

@Injectable({
  providedIn: 'root'
})
export class PublicDailyReportOperationsManagerResolver implements Resolve<DailyWoReportByOperationsManagerGridModel>
{
  constructor(private publicDailyReportService: PublicDailyReportService) {
  }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {

    const loggedEmployeeGuid = route.params.loggedEmployeeGuid;
    const operationsManagerId = route.params.operationsManagerId;
    const operationsManagerGuid = route.params.operationsManagerGuid;
    const dateFrom = route.params.dateFrom;
    const dateTo = route.params.dateTo;

    const initalStatusToFilter = WORK_ORDER_STATUS.find(status => status.value === 'stand-by').key + '_'
      + WORK_ORDER_STATUS.find(status => status.value === 'active').key;

    return this.publicDailyReportService.getReport(loggedEmployeeGuid, operationsManagerGuid, {
      operationsManagerId: operationsManagerId,
      dateFrom: dateFrom, dateTo: dateTo,
      statuses: initalStatusToFilter
    });
  }

}
