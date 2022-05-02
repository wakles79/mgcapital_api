import { CompanyEntity } from '@core/models/common/company-entity.model';
import { ContactBaseModel } from '@core/models/contact/contact-base.model';
import { ListUserModel } from '@app/core/models/user/list-users.model';

export class BuildingBaseModel extends CompanyEntity {
  name: string;
  addressId: number;
  employees: any[];
  isActive: boolean;
  contacts: ContactBaseModel[];
  address: { [key: string]: any };
  customerId: number;
  emergencyPhone: string;
  emergencyPhoneExt: string;
  emergencyNotes: string;
  code: string;

  constructor(building: BuildingBaseModel) {
    super(building);
    this.name = building.name || '';
    this.addressId = building.addressId || 0;
    this.employees = building.employees || [];
    this.isActive = building.isActive || false;
    this.contacts = building.contacts || [];
    this.address = building.address || {};
    this.customerId = building.customerId || null;
    this.emergencyPhone = building.emergencyPhone || '';
    this.emergencyPhoneExt = building.emergencyPhoneExt || '';
    this.emergencyNotes = building.emergencyNotes || '';
    this.code = building.code || building.id.toString();
  }
}
