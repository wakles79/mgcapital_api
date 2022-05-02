import { CompanyEntity } from '@core/models/common/company-entity.model';

export class ServiceBaseModel extends CompanyEntity {
  name: string;
  unitFactor: string;
  unitPrice: number;
  minPrice: number;

  constructor(service: ServiceBaseModel) {
    super(service);
    this.name = service.name || '';
    this.unitFactor = service.unitFactor || '';
    this.unitPrice = service.unitPrice || 0;
    this.minPrice = service.minPrice || 0;
  }
}
