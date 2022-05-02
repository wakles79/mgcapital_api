import { OfficeTypeBaseModel } from './office-type-base.model';

export class OfficeTypeGridModel extends OfficeTypeBaseModel {

  isUsed: boolean;

  constructor(officeTypeGrid: OfficeTypeGridModel) {
    super(officeTypeGrid);

    this.isUsed = officeTypeGrid.isUsed || false;
  }
}
