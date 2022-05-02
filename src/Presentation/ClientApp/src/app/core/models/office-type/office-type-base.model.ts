import { CompanyEntity } from '@core/models/common/company-entity.model';

export class OfficeTypeBaseModel extends CompanyEntity {
  name: string;
  rate: number;
  rateType: number;
  rateTypeName: string;
  periodicity: string;
  status: boolean;
  supplyFactor: number;

  constructor(officeType: OfficeTypeBaseModel) {
    super(officeType);
    if (officeType == null) {
      this.id = -1;
      this.name = '';
      this.rate = 0;
      this.rateType = 0;
      this.rateTypeName = '';
      this.periodicity = '';
      this.status = true;
      this.supplyFactor =  0;
    }
    else {
      this.id = officeType.id;
      this.name = officeType.name;
      this.rate = officeType.rate;
      this.rateType = officeType.rateType;
      this.rateTypeName = officeType.rateTypeName;
      this.periodicity = officeType.periodicity;
      this.status = officeType.status;
      this.supplyFactor =  officeType.supplyFactor;
    }
  }
}
