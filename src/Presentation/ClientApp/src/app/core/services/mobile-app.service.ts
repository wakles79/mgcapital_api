import { Injectable, Inject } from '@angular/core';
import { DataService } from '@app/core/services/data.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MobileApp, MobilePlatform } from '@app/core/models/mobile/mobile-app.enum';
import { MobileAppVersion } from '@app/core/models/mobile/mobile-app-version.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MobileAppService extends DataService {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'mobileAppVersion', http);
  }


  /**
   * Fetches the last version available given 'mobileApp' and 'platform'
   * @param  {Number} mobileApp
   * @param  {Number} platform
   */
  latestVersion(mobileApp: MobileApp, platform: MobilePlatform): Observable<any> {

    const params = new HttpParams()
      .set('mobileApp', mobileApp.toString())
      .set('platform', platform.toString());

    return this.http.get(`${this.fullUrl}/latestVersion`, {
      params: params
    });
  }

  /**
  * Fetches the last version available for every 'platform'
  */
  latestVersions(): Observable<MobileAppVersion> {
    return this.http.get<MobileAppVersion>(`${this.fullUrl}/latestVersions`);
  }

}
