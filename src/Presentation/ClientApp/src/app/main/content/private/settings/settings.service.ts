import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { CompanySettingsBaseModel } from '@app/core/models/company-settings/company-settings-base.model';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { CompanySettingsDetailModel } from '@app/core/models/company-settings/company-settings-detail.model';
@Injectable({
  providedIn: 'root'
})
export class SettingsService extends BaseListService<CompanySettingsBaseModel> {

  onSettingsDetailChanged: BehaviorSubject<CompanySettingsDetailModel> = new BehaviorSubject<CompanySettingsDetailModel>(null);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'companySettings', http);
  }

  loadSettings(action = 'get'): Promise<any> {
    return new Promise((resolve, reject) => {

      return this.http.get(`${this.fullUrl}/${action}`).subscribe(
        (response: CompanySettingsDetailModel) => {
          resolve(response);

          this.onSettingsDetailChanged.next(response);
        }, (error) => { reject(error); });

    });
  }

  loadPublicSettings(action = 'PublicGet'): Promise<any> {
    return new Promise((resolve, reject) => {

      return this.http.get(`${this.fullUrl}/${action}`).subscribe(
        (response: CompanySettingsDetailModel) => {
          resolve(response);

          this.onSettingsDetailChanged.next(response);
        }, (error) => { reject(error); });

    });
  }

  verifyFreshdeskCredentials(value: { key: string, agentId: string }, action = 'VerifyFreshdesk'): Observable<any> {
    return this.create(value, action);
  }
}
