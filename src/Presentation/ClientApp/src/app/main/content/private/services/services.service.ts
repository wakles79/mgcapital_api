import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ServiceGridModel } from '@app/core/models/service/service-grid.model';
import { BaseListService } from '@app/core/services/base-list.service';

@Injectable({
  providedIn: 'root'
})
export class ServicesService extends BaseListService<ServiceGridModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'services', http);
  }

}
