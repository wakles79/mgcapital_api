import { ContactBaseModel} from '@core/models/contact/contact-base.model';

export class ContactGridModel extends ContactBaseModel {

  fullName: string;
  phone: string;
  ext: string;
  fullAddress: string;
  email: string;
  type: string;
  count: number;

  constructor(contact: ContactGridModel) {
    super(contact);
    this.fullName = contact.fullName || '';
    this.phone = contact.phone || '';
    this.email = contact.email || '';
    this.type = contact.type || '';
    this.count = contact.count || 0;

  }
}
