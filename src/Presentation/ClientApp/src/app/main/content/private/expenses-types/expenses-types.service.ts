import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ExpenseTypeBaseModel } from '@app/core/models/expense-type/expense-type-base.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExpensesTypesService extends BaseListService<ExpenseTypeBaseModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'expensetypes', http);
  }

  saveSubcategory(expenseSubcategory: any, action = 'addSubcategory'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, expenseSubcategory, { observe: 'response' })
      .pipe(map((out: any) => {
        return out;
      }));
  }

  updateSubcategory(expenseSubcategory: any, action = 'updateSubcategory'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(expenseSubcategory, action)
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  getSubcategory(id: number, action = 'getSubcategory'): Observable<any> {
    return this.http.get(`${this.fullUrl}/${action}/${id}`);
  }

  getAllSubcategories(
    expenseTypeId: number,
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

    return this.http.get(`${this.fullUrl}/${action}/` + expenseTypeId, {
      params: queryParams
    });
  }
}
