import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { DepartmentBaseModel } from '@app/core/models/department/department-base.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentsService extends BaseListService<DepartmentBaseModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
      super(apiBaseUrl, 'departments', http);
    }

    deleteDepartment(id: number, action: string = 'Delete'): void {
      this.http.delete(`${this.fullUrl}/${action}/${id}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {
            this.getElements();
          }
        },
          (error) => {
            this.loadingSubject.next(false);
          });
    }
}
