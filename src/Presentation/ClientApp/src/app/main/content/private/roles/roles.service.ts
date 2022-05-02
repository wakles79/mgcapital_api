import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RolesService extends BaseListService<any> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'companySettings', http);
   }

   getRoles(action: string = 'GetRoles'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}`);
  }

  getModuleAccessByRole(roleId: number, action: string = 'GetModuleAccesByRole'): Observable<any> {
    const pars = new HttpParams()
      .set('roleId', roleId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: pars });
  }

  updateRoleModuleAccess(resource, action: string = 'UpdateRoleModuleAccess'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, resource);
  }

  updateRoleModulePermission(resource: { permissionId: number, roleId: number, isAssigned: boolean }, action: string = 'UpdateRoleModulePermission'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, resource);
  }

  getDefaultRoles(action: string = 'GetDefaultRolesCbo'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}`);
  }

  removeRole(id: number, action: string = 'DeleteRole'): Observable<any> {
    return this.http.delete(`${this.fullUrl}/${action}?id=${id}`);
  }

  getModulePermissions(appModule: ApplicationModule, action: string = 'GetModulePermissions'): Observable<any> {
    const params = new HttpParams()
      .set('module', appModule.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: params });
  }
}
