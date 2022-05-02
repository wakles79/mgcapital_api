import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CleaningReportDetailsModel } from '@app/core/models/reports/cleaning-report/cleaning.report.details.model';
import { Observable } from 'rxjs';
import { CleaningReportDetailsService } from './cleaning-report-details.service';


@Injectable({
  providedIn: 'root'
})
export class CleaningReportDetailsResolver implements Resolve<CleaningReportDetailsModel> {

  constructor(private service: CleaningReportDetailsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const reportId = route.params.id;
    return this.service.getDetails(reportId);
  }
}
