import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { ContactBaseModel } from '@core/models/contact/contact-base.model';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactsService } from './contacts.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';

@Injectable({
  providedIn: 'root'
})
export class ContactsResolver implements Resolve<ContactBaseModel>{

  constructor(private contactService: ContactsService) { }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Promise<any> | any {
    this.contactService.validateModuleAccess(ApplicationModule.Contacts);
    return this.contactService.getElements();
  }

}
