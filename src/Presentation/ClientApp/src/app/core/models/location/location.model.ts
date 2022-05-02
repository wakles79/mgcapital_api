import { CompanyEntity } from '@core/models/common/company-entity.model';

export class LocationModel extends CompanyEntity {
  name: string;
  buildingId: number;
  customerId: number;

  constructor(location: LocationModel) {
    super(location);
    this.name = location.name || '';

    // Relational fields
    this.buildingId = location.buildingId || 0;
    this.customerId = location.customerId || 0;
  }
}
