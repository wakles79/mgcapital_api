import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { UserBaseModel } from '@app/core/models/user/user-base.model';
import { AuthService } from '@app/core/services/auth.service';
import { BaseListService } from '@app/core/services/base-list.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService extends BaseListService<UserBaseModel> {

  authService: AuthService;

  elementsCount = 0;
  onUserDataChanged: BehaviorSubject<any> = new BehaviorSubject([]);

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient,
    authService: AuthService) {
    super(apiBaseUrl, 'employees', http);
    this.authService = authService;
  }

  getUserData(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.get(this.authService.currentUser.employeeGuid)
        .subscribe((response: any) => {
          this.elementChanged = response;
          this.onUserDataChanged.next(this.elementChanged);
          resolve(this.elementChanged);
        }, reject);
    });
  }

  updateElementData(userData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.update(userData)
        .subscribe(response => {
          this.getUserData();
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  createUser(userData): Promise<any> {
    return new Promise((resolve, reject) => {
      this.authService.register(userData)
        .subscribe(response => {
          this.getUserData();
          this.getElements();
          resolve(response);
        },
          (error) => {
            reject(error);
          });
    });
  }

  sendCredentials(user): Observable<any> {
    const email = user.email;
    return this.authService
      .sendCredentials(email);
  }

  deleteUser(id: number, action = 'delete'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}?id=${id}`, { observe: 'response' })
        .subscribe((response: any) => {
          if (response.status === 200) {
            this.getElements();
          }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }

  deleteUserByGuid(guid: string, action = 'DeleteByGuid'): Observable<any> {
    const pars = new HttpParams()
      .set('guid', guid);

    return this.http.delete(`${this.fullUrl}/${action}`, { params: pars });
  }
}
