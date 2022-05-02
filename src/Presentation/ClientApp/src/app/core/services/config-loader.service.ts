import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigLoaderService {

  private httpClient: HttpClient;
  config: { [key: string]: any };

  constructor(handler: HttpBackend) {
    // Using a new instance of HttpClient
    // rather that the one used in the rest of the app
    // to avoid interceptors and the like
    this.httpClient = new HttpClient(handler);
  }

  public async loadConfig(): Promise<any> {
    return this.httpClient.get('assets/config.json')
      .toPromise()
      .then((config: any) => {
        this.config = config;
      })
      .catch(error => {
        console.error(error);
      });
  }

}
