import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ScheduleCategoryBaseModel } from '@app/core/models/schedule-category/schedule-category-base.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ScheduleSettingsCategoryService extends BaseListService<ScheduleCategoryBaseModel>{

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'schedulesettingscategory', http);
  }

  saveSubcategory(subcategory: any, action = 'addSubcategory'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, subcategory, { observe: 'response' }).pipe(
      map((out: any) => {
        return out;
      }));
  }

  getAllSubcategories(
    scheduleId: number,
    params: { [key: string]: any } = {},
    filter = 'name', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    action = 'readAllSubcategoriesCbo'
  ): Observable<any> {
    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + scheduleId, {
      params: queryParams
    });
  }

  updateSubcategory(subcategory: any, action = 'updateSubcategory'): any {
    return new Promise((resolve, reject) => {
      this.update(subcategory, action)
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  getAllAsListByCategory(
    categoryId: number,
    params: { [key: string]: any } = {},
    filter = 'name', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    action = 'readAllSubcategoriesCbo'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/` + categoryId, {
      params: queryParams
    });
  }

  getAllAsListBySubCategory(
    params: { [key: string]: any } = {},
    filter = 'name', sortField = 'ASC',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    action = 'ReadAllMultipleSubcategoriesCbo'): Observable<any> {

    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}/`, {
      params: queryParams
    });
  }
}
