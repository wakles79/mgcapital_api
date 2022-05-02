import { Injectable } from '@angular/core';
import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { WorkOrderServiceBaseModel } from '@app/core/models/work-order-services/work-order-services-base.model';
import { WorkOrderServicesService } from './work-order-services.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable()
export class WorkOrderServicesResolver implements Resolve<WorkOrderServiceBaseModel>{

  constructor(private servicesService: WorkOrderServicesService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {

    this.servicesService.validateModuleAccess(ApplicationModule.WorkOrderServicesCatalog);

    this.servicesService.onFilterChanged.next({});
  }
}
