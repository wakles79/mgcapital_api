import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { InspectionsService } from './inspections.service';
import { Observable } from 'rxjs';
import { InspectionBaseModel } from '@app/core/models/inspections/inspection-base.model';
import * as moment from 'moment';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class InspectionsResolver implements Resolve<InspectionBaseModel>{

  filterBy: { [key: string]: any } = {};
  today: Date = new Date();

  constructor(private inspectionService: InspectionsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {

    this.inspectionService.validateModuleAccess(ApplicationModule.Inspections);

    this.filterBy['BeforeSnoozeDate'] = moment(new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate() + 30)).format('YYYY-MM-DD');
    this.inspectionService.onFilterChanged.next(this.filterBy);

    // return this.inspectionService.onFilterChanged.next({});
  }

}
