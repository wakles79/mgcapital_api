import { ActivatedRouteSnapshot, RouterStateSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { PermissionService } from '@app/core/services/permission.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class WorkOrderDetailResolver implements Resolve<any> {

  constructor(private _permissionService: PermissionService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {

    if (this._permissionService.permissions.length === 0) {
      this._permissionService.getUserModulePermissions(ApplicationModule.WorkOrder);
    }

    const id = route.params.id;
    return id;

  }
}
