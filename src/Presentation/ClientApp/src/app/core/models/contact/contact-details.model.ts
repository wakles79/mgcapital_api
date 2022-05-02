import { ContactBaseModel} from '@core/models/contact/contact-base.model';

export class ContactDetailsModel extends ContactBaseModel {

  fullName: string;
  email: string;
  phone: string;
  ext: string;
  fullAddress: string;

  constructor(contact: ContactDetailsModel) {
    super(contact);
    this.fullName = contact.fullName || '';
    this.email = contact.email || '';
    this.phone = contact.phone || '';
    this.ext = contact.ext || '';
    this.fullAddress = contact.fullAddress || '';
  }
}
