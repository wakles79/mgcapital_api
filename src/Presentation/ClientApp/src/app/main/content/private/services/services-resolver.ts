import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { ServiceGridModel } from '@core/models/service/service-grid.model';
import { Observable } from 'rxjs';
import { ServicesService } from './services.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ServiceResolver implements Resolve<ServiceGridModel>
{
  constructor(private serviceService: ServicesService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.serviceService.validateModuleAccess(ApplicationModule.Services);
    return this.serviceService.onFilterChanged.next();
  }
}
