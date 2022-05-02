import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { Observable } from 'rxjs';
import { RevenuesService } from './revenues.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class RevenuesResolver implements Resolve<RevenueBaseModel>{

  constructor(private revenueService: RevenuesService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.revenueService.validateModuleAccess(ApplicationModule.Revenues);
    return this.revenueService.getElements();
  }

}
