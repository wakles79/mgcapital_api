import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { SettingsBudgetService } from './settings-budget.service';
import { Observable } from 'rxjs';
import { BudgetSettingsBaseModel } from '@app/core/models/budget-settings/budget-settings-base.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SettingsBudgetResolver implements Resolve<BudgetSettingsBaseModel>{

  constructor(private service: SettingsBudgetService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.service.validateModuleAccess(ApplicationModule.BudgetSettings);
    return this.service.loadSettings();
  }

}
