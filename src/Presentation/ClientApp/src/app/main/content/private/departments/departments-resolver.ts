import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Injectable, Inject } from '@angular/core';
import { DepartmentBaseModel } from '@app/core/models/department/department-base.model';
import { Observable } from 'rxjs';
import { DepartmentsService } from './departments.service';


@Injectable({
  providedIn: 'root'
})
export class DepartmentsResolver implements Resolve<DepartmentBaseModel>
{
  constructor(private departmentService: DepartmentsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    return this.departmentService.getElements();
  }

}
