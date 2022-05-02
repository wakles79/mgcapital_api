import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { ScheduleSettingsCategoryService } from './schedule-settings-category.service';
import { ScheduleCategoryBaseModel } from '@app/core/models/schedule-category/schedule-category-base.model';

@Injectable()
export class ScheduleSettingsCategoryResolver implements Resolve<ScheduleCategoryBaseModel>
{
  constructor(private schedueSettingsCategoryService: ScheduleSettingsCategoryService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.schedueSettingsCategoryService.validateModuleAccess(ApplicationModule.ScheduledWorkOrderCategories);
    return this.schedueSettingsCategoryService.onFilterChanged.next({});
  }
}
