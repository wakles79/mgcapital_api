import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CompanyInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('token');
    const currentUser = JSON.parse(localStorage.getItem('employee'));
    const d = new Date();
    const timezoneOffset = d.getTimezoneOffset();
    const timezoneId = Intl.DateTimeFormat().resolvedOptions().timeZone;

    if (!(token && currentUser)) {
      return next.handle(request);
    }
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        CompanyId: currentUser.companyId.toString(),
        TimezoneOffset: timezoneOffset.toString(),
        TimezoneId: timezoneId
      }
    });
    return next.handle(request);
  }
}
