import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { BuildingGridModel } from '@app/core/models/building/building-grid.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BuildingsService extends BaseListService<BuildingGridModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'buildings', http);
   }

   getDocumentUrl(
    action = 'getBuildingsReportUrl',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Observable<any> {
    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', this.searchText ? this.searchText : '')
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, extendedParams);

    return this.http.get(`${this.fullUrl}/${action}`, { params: queryParams });
  }

  validateBuildingCodeAvailability(code: string, buildingId: number = -1, action = 'ValidateBuildingCode'): Observable<any> {
    const queryParams = new HttpParams()
      .set('code', code)
      .set('buildingId', buildingId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: queryParams });
  }

  exportCsv(
    action = 'ExportBuildingReportCsv',
    filter = '', sortField = '',
    sortOrder = '', pageNumber = 0, pageSize = 100,
    params: { [key: string]: string } = {}
  ): Promise<any> {
    let extendedParams: { [key: string]: string } = {};
    extendedParams = this.extendParams(params, this.filterBy);

    // Default data source filter params
    let queryParams = new HttpParams()
      .set('filter', this.searchText ? this.searchText : '')
      .set('sortField', sortField)
      .set('sortOrder', sortOrder)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    queryParams = this.extendQueryParams(queryParams, extendedParams);

    this.loadingSubject.next(true);
    return new Promise((resolve, reject) => {
      this.http.get(`${this.fullUrl}/${action}`, { params: queryParams, observe: 'response', responseType: 'text' })
        .subscribe((response: any) => {
          this.loadingSubject.next(false);
          resolve(response);
        },
          (error) => {
            this.loadingSubject.next(false);
            reject(error.error);
          });
    });
  }

  updateSharedBuilding(resource, action = 'UpdateSharedBuildingOperationManager'): Observable<any> {
    return this.http.post(`${this.fullUrl}/${action}`, resource);
  }

  getActivityLog(buildingId: number,
                 params: { [key: string]: string } = {}, action = 'ReadAllActivityLog'): Observable<any> {

      // Default data source filter params
    let queryParams = new HttpParams()
    .set('buildingId', buildingId.toString())
    .set('pageNumber', '0')
    .set('pageSize', '9999'); // HACK: As long as it needs

    queryParams = this.extendQueryParams(queryParams, params);

    return this.http.get(`${this.fullUrl}/${action}`, {
      params: queryParams
    });
  }
}
