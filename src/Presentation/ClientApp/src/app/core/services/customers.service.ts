import { Injectable, Inject } from '@angular/core';
import { DataService } from '@core/services/data.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PhoneModel } from '@core/models/common/phone.model';
import { EmailModel } from '@core/models/common/email.model';
import { AddressModel } from '@core/models/common/address.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomersBaseService extends DataService {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'customers', http);
  }

  // Phones
  getAllPhones(customerId: number): Observable<any> {
    return this.getAll(`readallphones/${customerId}`);
  }

  getPhone(id): Observable<any> {
    return this.get(id, 'updatephone');
  }

  updatePhone(phone: PhoneModel): Observable<any> {
    return this.update(phone, 'updatephone');
  }

  createPhone(phone: PhoneModel): Observable<any> {
    return this.create(phone, 'addphone');
  }

  deletePhone(id): Promise<any> {
    return this.delete(id, 'deletephone');
  }
  // End Phones

  // Emails
  getAllEmails(customerId: number): Observable<any> {
    return this.getAll(`readallemails/${customerId}`);
  }

  getEmail(id): Observable<any> {
    return this.get(id, 'updateemail');
  }

  updateEmail(email: EmailModel): Observable<any> {
    return this.update(email, 'updateemail');
  }

  createEmail(email: EmailModel): Observable<any> {
    return this.create(email, 'addemail');
  }

  deleteEmail(id): Promise<any> {
    return this.delete(id, 'deleteemail');
  }
  // End Emails

  // Addresses
  getAllAddresses(customerId: number): Observable<any> {
    return this.getAll(`readalladdresses/${customerId}`);
  }

  getAddress(customerId: number, addressId: number): Observable<any> {
    return this.get(`${customerId}?addressId=${addressId}`, `updateaddress`);
  }

  updateAddress(address: AddressModel): Observable<any> {
    return this.update(address, 'updateaddress');
  }

  createAddress(address: AddressModel): Observable<any> {
    return this.create(address, 'addaddress');
  }

  deleteAddress(customerId: number, addressId: number): Promise<any> {
    return this.delete(`${customerId}?addressId=${addressId}`, `deleteaddress`);
  }
  // End Addresses

  // Groups
  getAllAssignedGroups(customerId: number): Observable<any> {
    return this.getAll(`readallassignedgroups/${customerId}`);
  }
  getAllGroups(): Observable<any> {
    return this.getAll('readallgroups');
  }
  // End Groups

  // Employees
  getAllEmployees(customerId: number): Observable<any> {
    return this.getAll(`readallemployees/${customerId}`);
  }
  // End of Employees

  validateCustomerCodeAvailability(code: string, customerId: number = -1, action = 'ValidateCustomerCode'): Observable<any> {
    const queryParams = new HttpParams()
      .set('code', code)
      .set('customerId', customerId.toString());

    return this.http.get(`${this.fullUrl}/${action}`, { params: queryParams });
  }
}
