import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { WorkOrderBaseModel } from '@app/core/models/work-order/work-order-base.model';
import { Observable } from 'rxjs';
import { WorkOrdersService } from './work-orders.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { PermissionService } from '@app/core/services/permission.service';

@Injectable({
  providedIn: 'root'
})
export class WorkOrderResolver implements Resolve<WorkOrderBaseModel>
{
  constructor(
    private woService: WorkOrdersService,
    private _permissionService: PermissionService
  ) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {

    this._permissionService.getUserModulePermissions(ApplicationModule.WorkOrder);
    this.woService.validateModuleAccess(ApplicationModule.WorkOrder);

    this.woService.viewName = 'work-order';
    this.woService.loadSessionFilter();

    if (this.woService.filterBy === undefined || this.woService.filterBy === null) {
      this.woService.filterBy = {};
    }

    return this.woService.onFilterChanged.next(this.woService.filterBy);
  }

}
