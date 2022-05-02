import { RolesService } from './roles.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RolesResolver implements Resolve<any>
{
  constructor(private rolesService: RolesService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.rolesService.validateModuleAccess(ApplicationModule.Roles);
  }

}
