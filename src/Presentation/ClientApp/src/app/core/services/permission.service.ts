import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ApplicationModule } from '../models/company-settings/company-settings-base.model';
import { PermissionAssignmentModel } from '../models/permission/permission-assignment.model';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {


  fullUrl: string;

  isLoading = false;

  permissions: PermissionAssignmentModel[] = [];
  onPermissionsChanged = new Subject<PermissionAssignmentModel[]>();

  constructor(
    @Inject('API_BASE_URL') public apiBaseUrl: string,
    public http: HttpClient,
  ) {
    this.fullUrl = `${this.apiBaseUrl}api/CompanySettings`;
  }

  getUserModulePermissions(appModule: ApplicationModule, action: string = 'GetModulePermissions'): void {
    if (this.isLoading) {
      return;
    }

    const pars = new HttpParams()
      .set('module', appModule.toString());

    this.isLoading = true;
    this.http.get(`${this.fullUrl}/${action}`, { params: pars })
      .subscribe((permissions: any) => {
        this.permissions = permissions;
        this.onPermissionsChanged.next(permissions);
        this.isLoading = false;
      }, () => {
        this.isLoading = false;
      });

  }
}
