import { WoCalendarService } from './wo-calendar.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class WoCalendarResolver implements Resolve<any>{

  constructor(private calendarService: WoCalendarService) { }

  resolve(
    router: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.calendarService.validateModuleAccess(ApplicationModule.Calendar);

    this.calendarService.viewName = 'calendar';
    this.calendarService.loadSessionFilter();

    // this.calendarService.onFilterChanged.next(this.filterBy);
  }
}
