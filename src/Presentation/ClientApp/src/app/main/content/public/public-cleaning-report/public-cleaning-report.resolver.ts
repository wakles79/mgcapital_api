import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CleaningReportDetailsModel } from '@app/core/models/reports/cleaning-report/cleaning.report.details.model';
import { CleaningReportDetailsService } from '@app/main/content/private/reports/cleaning-report/cleaning-report-details/cleaning-report-details.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PublicCleaningReportResolver implements Resolve<CleaningReportDetailsModel>
{
  constructor(private service: CleaningReportDetailsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    const reportGuid = route.params.guid;
    return this.service.getPublicDetails(reportGuid);
  }

}
