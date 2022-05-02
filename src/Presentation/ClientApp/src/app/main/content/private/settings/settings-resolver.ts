import { Injectable } from '@angular/core';
import { CompanySettingsBaseModel, ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { SettingsService } from './settings.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SettingsResolver implements Resolve<CompanySettingsBaseModel>{

  constructor(private service: SettingsService ){}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any>|Promise<any>|any {
    this.service.validateModuleAccess(ApplicationModule.CompanySettings);
    return this.service.loadSettings();
  }

}
