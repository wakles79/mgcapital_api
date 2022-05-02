import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { UserBaseModel } from '@app/core/models/user/user-base.model';
import { Observable } from 'rxjs';
import { UsersService } from './users.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class UsersResolver implements Resolve<UserBaseModel>
{
  constructor(private usersService: UsersService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.usersService.validateModuleAccess(ApplicationModule.Users);
    return this.usersService.onFilterChanged.next({});
  }
}
