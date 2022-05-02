import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DailyWoReportByOperationsManagerService } from '@app/main/content/private/work-orders/wo-daily-report-operations-manager.service';
import { DailyWoReportByOperationsManagerGridModel } from '@app/core/models/work-order/wo-daily-report-operations-manager-grid.model';

@Injectable({
  providedIn: 'root'
})
export class DailyWoReportByOperationsManagerResolver implements Resolve<DailyWoReportByOperationsManagerGridModel>
{
    constructor(private dailyReportService: DailyWoReportByOperationsManagerService ){}

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
      ): Observable<any>|Promise<any>|any {
        return this.dailyReportService.getElements('ReadDailyReportByOperationsManager', '', '',
        '', 0,  50, {});
      }

}
