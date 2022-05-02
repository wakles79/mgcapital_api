import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ListDataSourceRequestModel } from '../../../models/common/list-data-source-request.model';

@Injectable({
  providedIn: 'root'
})
export class ElementSelectorFilterService {

  onCleanSelectedValues: Subject<any> = new Subject();

  constructor(
    @Inject('API_BASE_URL') public apiBaseUrl: string,
    public http: HttpClient) {
  }

  getDataToFilter(listDataSourceRequest: ListDataSourceRequestModel): any {

    if (!listDataSourceRequest.elementId || listDataSourceRequest.elementId === 0) {
      listDataSourceRequest.elementId = '';
    }
    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', listDataSourceRequest.filter)
      .set('id', listDataSourceRequest.elementId.toString())
      .set('pageNumber', listDataSourceRequest.pageNumber.toString())
      .set('pageSize', listDataSourceRequest.pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, listDataSourceRequest.params);

    return this.http.get(`${this.apiBaseUrl}api/${listDataSourceRequest.filterApiUrl}`,
      { params: queryParams, observe: 'body' });
  }

  /**
   * Returns a new copy of HttpParam with the extended fields given in "dictparams"
   * @param httpParams The inmutable HttpParam object
   * @param dictParams The dictionary that will "extend" the query params
   */
  protected extendQueryParams(
    httpParams: HttpParams,
    dictParams: { [key: string]: string } = {}): HttpParams {
    if (dictParams) {
      for (const key in dictParams) {
        if (dictParams.hasOwnProperty(key)) {
          const value = dictParams[key];
          httpParams = httpParams.set(key, value);
        }
      }
    }
    return httpParams;
  }
}
