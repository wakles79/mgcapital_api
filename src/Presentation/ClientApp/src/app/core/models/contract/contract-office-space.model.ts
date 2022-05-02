import { EntityModel } from '../common/entity.model';

export class ContractOfficeSpaceModel extends EntityModel {

  contractId: number;
  officeTypeId: number;
  officeTypeName: string;

  squareFeet: number;

  constructor(officeSpaceModel: ContractOfficeSpaceModel) {
    super(officeSpaceModel);

    this.contractId = officeSpaceModel.contractId || 0;
    this.officeTypeId = officeSpaceModel.officeTypeId || null;
    this.officeTypeName = officeSpaceModel.officeTypeName || '';
    this.squareFeet = officeSpaceModel.squareFeet || 0;
  }
}
