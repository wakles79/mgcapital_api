import { CompanyEntity } from '@core/models/common/company-entity.model';

export class UserBaseModel extends CompanyEntity {
  firstName: string;
  middleName: string;
  lastName: string;
  fullName: string;
  salutation: string;
  dob: Date;
  gender: string;
  email: string;
  phone: string;
  ext: string;
  notes: string;
  employeeStatusId: number;
  departmentId: number;
  contactId: number;
  roleId: number;
  roleLevel: number;
  sendNotifications: boolean;
  hasFreshdeskAccount: boolean;
  freshdeskApiKey: string;
  freshdeskAgentId: string;
  emailSignature: string;
  /**
   *
   */
  constructor(user: UserBaseModel) {
    super(user);
    this.firstName = user.firstName || '';
    this.middleName = user.middleName || '';
    this.lastName = user.lastName || '';
    this.fullName = user.fullName || '';
    this.salutation = user.salutation || '';
    this.dob = user.dob;
    this.gender = user.gender || 'U';
    this.notes = user.notes || '';
    this.phone = user.phone || '';
    this.ext = user.ext || '';
    this.email = user.email || '';
    this.employeeStatusId = user.employeeStatusId;
    this.departmentId = user.departmentId || null;
    this.contactId = user.contactId || null;
    this.roleId = user.roleId || null;
    this.roleLevel = user.roleLevel || null;
    this.sendNotifications = user.sendNotifications || false;
    this.hasFreshdeskAccount = user.hasFreshdeskAccount || false;
    this.freshdeskApiKey = user.freshdeskApiKey || '';
    this.freshdeskAgentId = user.freshdeskAgentId || '';
    this.emailSignature = user.emailSignature;
  }
}
