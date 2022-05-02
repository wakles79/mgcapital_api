import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BuildingBaseModel } from '@app/core/models/building/building-base.model';
import { BuildingGridModel } from '@app/core/models/building/building-grid.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BuildingsProfileService extends BaseListService<BuildingGridModel>{

  buildingDetail: BuildingBaseModel;
  onBuildingDetailChanged: BehaviorSubject<BuildingBaseModel> = new BehaviorSubject<BuildingBaseModel>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'buildings', http);
  }

  // HACK: Have to do this but ideally will be
  // to re-use contacts service
  getContactsByBuilding(buildingId: number): Observable<any> {
    const queryParams = new HttpParams()
      .set('contactType', 'building');
    return this.http.get(`${this.apiBaseUrl}api/contacts/ReadAllContactsBuildingProfile/${buildingId}?pageSize=9999`, {
      params: queryParams
    });
  }

  getBuildDetailDate(id: number): Promise<any> {
    return new Promise((resolve, reject) => {
      this.get(id, 'GetDetail')
        .subscribe((response: any) => {
          resolve(response);
          this.buildingDetail = response;
          this.onBuildingDetailChanged.next(this.buildingDetail);
        }, (error) => { reject(error); });
    });
  }

}
