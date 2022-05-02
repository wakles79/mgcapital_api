import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { WorkOrderServiceBaseModel } from '@app/core/models/work-order-services/work-order-services-base.model';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class WorkOrderServicesService extends BaseListService<WorkOrderServiceBaseModel>{

  updateCategoryList = new Subject<boolean>();
  selectedCategoryChanged = new Subject<number>();

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'WorkOrderServices', http);
  }

  readAllCategories(
    action = 'ReadAllCategories',
    params: { [key: string]: string } = {},
    filter = '',
    pageNumber = 0,
    pageSize = 20,
    id = null
  ): Observable<any> {
    if (!id || id === 0) {
      id = '';
    }
    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('id', id.toString())
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);
    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }

  readAllCategoriesAsList(action = 'ReadAllCategoriesCbo'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}`);
  }

  createCategory(resource: any, action = 'AddCategory'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, resource);
  }

  updateCategory(resource: any, action = 'UpdateCategory'): Observable<any> {
    return this.http.put(`${this.fullUrl}/${action}`, resource);
  }
}
