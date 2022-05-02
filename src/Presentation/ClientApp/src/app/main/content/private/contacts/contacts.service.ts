import { Inject, Injectable } from '@angular/core';
import { BaseListService } from '@app/core/services/base-list.service';
import { ContactBaseModel } from '@app/core/models/contact/contact-base.model';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PhoneModel } from '@app/core/models/common/phone.model';
import { EmailModel } from '@app/core/models/common/email.model';
import { AddressModel } from '@app/core/models/common/address.model';

@Injectable({
  providedIn: 'root'
})
export class ContactsService extends BaseListService<ContactBaseModel> {

  constructor(
    @Inject('API_BASE_URL') apiBaseUrl: string,
    http: HttpClient) {
    super(apiBaseUrl, 'contacts', http);
  }

  assignContact(resource: any, action = 'assigncontact'): Observable<HttpResponse<any>> {
    return this.http.post(`${this.fullUrl}/${action}`, resource, { observe: 'response' });
  }

  unassignContact(entityId: number, contactId: number, contactType: any): Promise<any> {

    const queryParams = new HttpParams()
      .set('contactType', contactType.toString())
      .set('entityId', entityId.toString());

    return this.http.delete(`${this.fullUrl}/unassign/${contactId}`, {
      params: queryParams
    })
      .toPromise();
  }

  getAllContactsByEntity(entityId: number, contactType: any): Observable<any> {
    const queryParams = new HttpParams()
      .set('contactType', contactType);
    return this.http.get(`${this.fullUrl}/readallcontacts/${entityId}?pageSize=9999`, {
      params: queryParams
    });
  }

  getContactByEntity(buildingId: number, contactId: number, contactType: any): Observable<any> {
    const queryParams = new HttpParams()
      .set('contactType', contactType)
      .set('entityId', buildingId.toString());
    return this.http.get(`${this.fullUrl}/getcontact/${contactId}`, {
      params: queryParams
    });
  }

  sendInvitationEmailContact(resource: any): Observable<HttpResponse<any>> {
    return this.http.post(`${this.apiBaseUrl}api/CustomerAccount/EmailInvitedCustomer`, resource, { observe: 'response' });
  }

  // Phones
  getAllPhones(contactId: number): Observable<any> {
    return this.getAll(`readallphones/${contactId}`);
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

  deletePhone(id: number, action = 'DeletePhone'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/${id}`, { observe: 'response' })
        .subscribe((response: any) => {
          // if (response.status === 200) {
          //   this.getElements();
          // }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }
  // End Phones

  // Emails
  getAllEmails(contactId: number): Observable<any> {
    return this.getAll(`readallemails/${contactId}`);
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

  deleteEmail(contactEmailId: number, action = 'DeleteEmail'): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.delete(`${this.fullUrl}/${action}/${contactEmailId}`, { observe: 'response' })
        .subscribe((response: any) => {
          // if (response.status === 200) {
          //   this.getElements();
          // }
          resolve(response);
        },
          (error) => {
            reject(error.error);
          });
    });
  }
  // End Emails

  // Addresses
  getAllAddresses(contactId: number): Observable<any> {
    return this.getAll(`readalladdresses/${contactId}`);
  }

  getAddress(contactId: number, addressId: number): Observable<any> {
    return this.get(`${contactId}?addressId=${addressId}`, `updateaddress`);
  }

  updateAddress(address: AddressModel): Observable<any> {
    return this.update(address, 'updateaddress');
  }

  createAddress(address: AddressModel): Observable<any> {
    return this.create(address, 'addaddress');
  }

  deleteAddress(contactId: number, addressId: number): Promise<any> {
    return this.delete(`${contactId}?addressId=${addressId}`, `deleteaddress`);
  }
  // End Addresses

  deleteContact(customer, action = 'delete'): Observable<any> {
    return this.http.delete(`${this.fullUrl}/${action}/${customer.guid}`);
  }

  deleteByGuid(guid: string, action = 'DeleteByGuid'): Observable<any> {
    const pars = new HttpParams()
      .set('guid', guid);

    return this.http.delete(`${this.fullUrl}/${action}`, { params: pars });
  }

}
