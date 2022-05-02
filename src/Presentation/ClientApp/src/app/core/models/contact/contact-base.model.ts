import { CompanyEntity } from '@core/models/common/company-entity.model';

export class ContactBaseModel extends CompanyEntity {
  firstName: string;
  middleName: string;
  lastName: string;
  fullName: string;
  salutation: string;
  title: string;
  dob: Date;
  gender: string;
  phone: string;
  email: string;
  ext: string;
  notes: string;
  fullAddress: string;
  sendNotifications: boolean;
  // Relational fields
  entityId: number;
  type: string;
  default: boolean;
  contactType: string;

  /**
   *
   */
  constructor(contact: ContactBaseModel) {
    super(contact);
    this.firstName = contact.firstName || '';
    this.middleName = contact.middleName || '';
    this.lastName = contact.lastName || '';
    this.fullName = contact.fullName || '';
    this.salutation = contact.salutation || '';
    this.title = contact.title || '';
    this.dob = contact.dob;
    this.gender = contact.gender || 'U';
    this.notes = contact.notes || '';
    this.sendNotifications = contact.sendNotifications || false;
    this.phone = contact.phone || '';
    this.ext = contact.ext || '';
    this.email = contact.email || '';
    this.fullAddress = contact.fullAddress || '';

    // Relational fields
    this.entityId = contact.entityId || 0;
    this.type = contact.type || '';
    this.default = contact.default || false;
    this.contactType = contact.contactType || '';
  }

}
