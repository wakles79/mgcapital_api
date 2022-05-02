import { ErrorHandler, Injectable } from '@angular/core';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppErrorHandler implements ErrorHandler {

  handleError(error: any): void {
    throwError(error);
  }
}
