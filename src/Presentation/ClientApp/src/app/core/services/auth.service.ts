import { Observable, throwError } from 'rxjs';
import { RegisterInterface } from '@core/models/account/register.interface';
import { Injectable, Inject } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { LoginModel } from '@core/models/account/login.model';
import { BadInput } from '@core/error-handling/bad-input';
import { NotFoundError } from '@core/error-handling/not-found-error';
import { ServerDown } from '@core/error-handling/server-down';
import { UnathorizedError } from '@core/error-handling/unathorized-error';
import { AppError } from '@core/error-handling/app-error';
import { ResetPasswordModel } from '@core/models/account/reset-password.model';
import { ForbiddenError } from '@app/core/error-handling/forbidden-error';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  lastUrl: string;
  jwtHelper = new JwtHelperService();

  constructor(
    @Inject('API_BASE_URL') private apiBaseUrl: string,
    private http: HttpClient,
    private router: Router) {
    this.lastUrl = 'app/home';
  }

  login(loginModel: LoginModel): Observable<any> {
    return this.http
      .post(this.apiBaseUrl + 'api/account/login', loginModel)
      .pipe(
        map((result: any) => {
          if (result &&
            result.token &&
            result.count &&
            result.employees) {
            this.setLocalStorage(result);
            return true;
          }
          return false;
        }),
        catchError(this.handleError)
      );
  }

  private setLocalStorage(result: any): void {
    localStorage.setItem('token', result.token);
    if (result.count === 1) {
      localStorage.setItem('employee', JSON.stringify(result.employees[0]));
    }
    localStorage.setItem('employees', JSON.stringify(result.employees));

    // initialize filters by empty array
    localStorage.setItem('filters', JSON.stringify([]));
  }

  setEmployee(employee: any): void {
    localStorage.setItem('employee', JSON.stringify(employee));
  }

  /**
   * Creates a new user with the same company as the logged in user
   * also send a "confirm email"
   * @param registerModel All the required fields (email, firstName, lastName)
   */
  register(registerModel: RegisterInterface): Observable<any> {
    return this.http
      .post(this.apiBaseUrl + 'api/account/register', registerModel)
      .pipe(
        map((result: any) => {
          if (result) {
            return true;
          }
          return false;
        }),
        catchError(this.handleError)
      );
  }

  /**
   * Makes a POST request with a given email to request a forgot password email
   * @param resetObj Object with the email that asks for password redemption
   */
  sendReset(resetObj: { email: string }): Observable<any> {
    return this.http
      .post(this.apiBaseUrl + 'api/account/forgotpassword', resetObj)
      .pipe(catchError(this.handleError));
  }

  /**
   * Makes a POST request with all the given credentials to perform a
   * reset password action
   * @param resetPasswordObj Model with email, pass, confirmPass and code
   */
  resetPassword(resetPasswordObj: ResetPasswordModel): Observable<any> {
    return this.http
      .post(this.apiBaseUrl + 'api/account/resetpassword', resetPasswordObj)
      .pipe(catchError(this.handleError));
  }

  /**
   * Validates that all the given tokens are fine, confirm the current user and gets its email for reseting its password
   * @param userId Base64 Decoded userId from .net app
   * @param confirm Base64 Decoded confirm code from .net app (this is for email confirmation only)
   * @param code Base64 Decoded code from .net app (this is for reset password only)
   */
  confirmUser(userId: string, confirm: string, code: string): Observable<any> {
    return this.http
      .get(`${this.apiBaseUrl}api/account/confirmEmail?userId=${encodeURIComponent(userId)}&confirm=${encodeURIComponent(confirm)}&code=${encodeURIComponent(code)}`)
      .pipe(catchError(this.handleError));

  }

  /**
   * Sends confirmation or reset password email depending
   * on the user's confirmEmail status
   * @param email User's email
   */
  sendCredentials(email: string): Observable<any> {
    return this.http
      .post(this.apiBaseUrl + 'api/account/sendcredentials', { email: email })
      .pipe(catchError(this.handleError));
  }

  /**
   * Gets the email for the given user identifier
   * @param userId The user's guid
   */
  getUserEmail(userId: string): Observable<any> {
    return this.http
      .get(`${this.apiBaseUrl}api/account/getUserEmail?userId=${userId}`)
      .pipe(catchError(this.handleError));
  }

  /** Removes auth token from local storage and redirects to login page */
  logout(): void {
    this.removeToken();
    this.router.navigate(['auth/login']);
  }

  /** Removes Auth token from local storage */
  removeToken(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('employee');
    localStorage.removeItem('employees');
    localStorage.removeItem('filters');
  }

  /** Gets raw JWT from local storage */
  get rawToken(): string {
    return localStorage.getItem('token');
  }

  /** Gets parsed JWT from local storage */
  get decodedToken(): any {
    return this.jwtHelper.decodeToken(this.rawToken);
  }

  /** Checks if auth token hasn't expired yet */
  isLoggedIn(): boolean {
    const rawToken = this.rawToken;
    const expirationDate = this.jwtHelper.getTokenExpirationDate(rawToken);
    const isExpired = this.jwtHelper.isTokenExpired(rawToken);
    return !isExpired;
  }

  /**
   * Gets the logged in user
   */
  get currentUser(): any {
    const employee = JSON.parse(localStorage.getItem('employee'));
    if (!(employee && employee.companyId)) {
      return null;
    }
    return employee;
  }

  get employees(): any[] {
    return JSON.parse(localStorage.getItem('employees'));
  }

  get employeesCount(): number {
    if (!this.employees) {
      return null;
    }
    return this.employees.length;
  }

  // TODO: DRY this function (is also coded in data.service.ts)
  private handleError(error: Response): Observable<AppError> {

    if (error.status === 0) {
      return throwError(new ServerDown(error));
    }

    if (error.status === 400) {
      return throwError(new BadInput(error));
    }

    if (error.status === 404) {
      return throwError(new NotFoundError());
    }

    if (error.status === 401) {
      return throwError(new UnathorizedError(error));
    }

    if (error.status === 403) {
      return throwError(new ForbiddenError(error));
    }

    return throwError(new AppError(error));
  }

  // Get User Role Module Access
  getModuleAccess(action: string = 'GetRoleModuleAccess'): Observable<any> {
    return this.http
      .get(this.apiBaseUrl + 'api/CompanySettings/' + action);
  }
}

