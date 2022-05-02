import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { EMPTY } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerInterceptor implements HttpInterceptor {

  constructor(private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        catchError((err: HttpErrorResponse) => {

          if (err.error instanceof Error) {
            // A client-side or network error occurred. Handle it accordingly.
            console.error('An error occurred:', err.error.message);
          } else {
            // The backend returned an unsuccessful response code.
            if (err.status === 401) {
              this.router.navigate(['/auth/login']);
            }

            if (err.status === 403) {
              this.router.navigate(['/auth/login']);
            }

            if (err.status === 404) {
              this.router.navigate(['/errors/error-404']);
            }

            return throwError(err);
          }

          // Return a empty observable so app can continue
          return EMPTY;
          // return Observable.empty<HttpEvent<any>>();
        })
      );
  }
}
