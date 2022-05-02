import { EntityModel } from '@app/core/models/common/entity.model';

export class ContractNoteBaseModel extends EntityModel {


  epochCreatedDate: number;
  contractId: number;
  employeeId: number;
  note: string;

  constructor(contractNoteModel: ContractNoteBaseModel) {
    super(contractNoteModel);

    this.epochCreatedDate = contractNoteModel.epochCreatedDate || 0;
    this.contractId = contractNoteModel.contractId || 0;
    this.employeeId = contractNoteModel.employeeId || 0;
    this.note = contractNoteModel.note || '';

  }

}
