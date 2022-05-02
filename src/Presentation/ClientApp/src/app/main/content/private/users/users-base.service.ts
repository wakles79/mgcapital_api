import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { DataService } from '@app/core/services/data.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersBaseService extends DataService {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'employees', http);
  }

  getSupervisorsByBuildingId(
    action = 'readallSupervisorsCbo',
    filter = '',
    pageNumber = 0,
    pageSize = 20,
    id = null,
    buildingId,
    params: { [key: string]: string } = {}
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
    return this.http.get(`${this.fullUrl}/${action}?buildingId=${buildingId}`, {
      params: queryParams
    });
  }

  getEmployeesByBuilding(buildingId: Number, action = 'ReadAllByBuildingCbo'): Observable<any> {
    const parameters = new HttpParams()
      .set('buildingId', buildingId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: parameters
    });
  }
}
