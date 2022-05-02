
import { UnathorizedError } from './../error-handling/unathorized-error';
import { ServerDown } from './../error-handling/server-down';
import { Inject } from '@angular/core';
import { catchError } from 'rxjs/operators';

import { BadInput } from '../error-handling/bad-input';
import { NotFoundError } from '../error-handling/not-found-error';
import { AppError } from '../error-handling/app-error';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { throwError } from 'rxjs';
import { BehaviorSubject } from 'rxjs';

import { ApplicationModuleEnum } from '@app/core/models/permission/application-module-enum';
import { NOACCESS_KEY, READONLY_KEY } from '@app/core/configs/configuration-const';
import { AccessType } from '../models/permission/permission-access-type-enum';
import { ForbiddenError } from '@app/core/error-handling/forbidden-error';

export class DataService {

  readOnlySubject = new BehaviorSubject<boolean>(true);
  noAccessSubject = new BehaviorSubject<boolean>(true);

  constructor(
    @Inject('API_BASE_URL') public apiBaseUrl: string,
    public url: string,
    public http: HttpClient) {
    this.readOnlySubject.next(localStorage.getItem(READONLY_KEY) !== null);
    this.noAccessSubject.next(localStorage.getItem(NOACCESS_KEY) !== null);
  }


  /**
   * Returns a new copy of HttpParam with the extended fields given in "dictparams"
   * @param httpParams The inmutable HttpParam object
   * @param dictParams The dictionary that will "extend" the query params
   */
  protected extendQueryParams(
    httpParams: HttpParams,
    dictParams: { [key: string]: any } = {}): HttpParams {
    if (dictParams) {
      for (const key in dictParams) {
        if (dictParams.hasOwnProperty(key)) {

          const value = dictParams[key];

          if (Array.isArray(value)) {

            value.forEach(multiselectValue => {
              httpParams = httpParams.append(key, multiselectValue);
            });

          }
          else {

            httpParams = httpParams.set(key, value);
          }
        }
      }
    }
    return httpParams;
  }

  protected extendParams(
    params: { [key: string]: any },
    extraParams: { [key: string]: any } = {}) {
    if (extraParams) {
      for (const key in extraParams) {
        if (extraParams.hasOwnProperty(key)) {
          const value = extraParams[key];
          params[key] = value;
        }
      }
    }
    return params;
  }

  get fullUrl() {
    return `${this.apiBaseUrl}api/${this.url}`;
  }

  getAll(
    action = 'readall', filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 20,
    params: { [key: string]: string } = {}) {

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', filter)
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    })
      .pipe(
        catchError(this.handleError)
      );
  }

  getAllAsList(
    action = 'readallcbo',
    filter = '',
    pageNumber = 0,
    pageSize = 20,
    id = null,
    params: { [key: string]: string } = {}
  ) {
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
    })
      .pipe(
        catchError(this.handleError)
      );
  }

  get(id, action = 'get') {
    return this.http.get(`${this.fullUrl}/${action}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  create(resource, action = 'add'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, resource, { observe: 'response' })
      .pipe(
        catchError(this.handleError)
      );
  }

  // create(resource, action = 'add'): Observable<any> {
  //   return this.http.post(`${this.fullUrl}/${action}`, resource, { observe: 'response' });
  // }

  update(resource, action = 'update') {
    return this.http.put(`${this.fullUrl}/${action}`, resource)
      .pipe(
        catchError(this.handleError)
      );
  }

  delete(id, action = 'delete') {
    return this.http.delete(`${this.fullUrl}/${action}/${id}`)
      .pipe(
        catchError(this.handleError)
      )
      .toPromise();
  }

  handleError(error: Response): Observable<AppError> {

    if (error.status === 0) {
      return throwError(new ServerDown(error));
    }

    if (error.status === 400) {
      return throwError(new BadInput(error));
    }

    if (error.status === 404) {
      return throwError(new NotFoundError());
    }

    if (error.status === 401) {
      return throwError(new UnathorizedError(error));
    }

    if (error.status === 403) {
      return throwError(new ForbiddenError(error));
    }

    return throwError(new AppError(error));
  }


}
