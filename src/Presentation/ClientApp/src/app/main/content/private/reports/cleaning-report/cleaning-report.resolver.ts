import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CleaningReportGridModel } from '@app/core/models/reports/cleaning-report/cleaning.report.grid.model';
import { Injectable } from '@angular/core';
import { CleaningReportService } from './cleaning-report.service';
import { Observable } from 'rxjs';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class CleaningReportResolver implements Resolve<CleaningReportGridModel> {

    constructor(private cleaningReportService: CleaningReportService) {
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<any> | Promise<any> | any {
      this.cleaningReportService.validateModuleAccess(ApplicationModule.CleaningReport);
      return this.cleaningReportService.getElements();
    }
}
