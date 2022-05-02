import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { InspectionBaseModel } from '@app/core/models/inspections/inspection-base.model';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class InspectionsService extends BaseListService<InspectionBaseModel>  {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient
  ) {
    super(apiBaseUrl, 'inspections', http);
  }

  createInspectionFromPreCalendar(elementData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.create(elementData, 'AddInspectionFromPreCalendar')
        .subscribe(response => {
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  deleteInspection(inspectionId, action = 'Delete'): Promise<any> {
    const element = this.allElementsToList.find(e => e.id === inspectionId);
    const elementIndex = this.allElementsToList.indexOf(element);

    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}?id=${inspectionId}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {
            this.allElementsToList.splice(elementIndex, 1);
            this.allElementsChanged.next(this.allElementsToList);
            this.elementsCount = this.allElementsToList.length;
          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }
}
