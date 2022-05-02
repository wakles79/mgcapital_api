import { ContractNoteBaseModel } from '@app/core/models/contract/contract-note-base.model';

export class ContractNoteGridModel extends ContractNoteBaseModel {

  employeeEmail: string;
  employeeFullName: string;
  me: boolean;

  constructor(contractNoteGrid: ContractNoteGridModel) {
    super(contractNoteGrid);

    this.employeeEmail = contractNoteGrid.employeeEmail || '';
    this.employeeFullName = contractNoteGrid.employeeFullName || '';
    this.me = contractNoteGrid.me || false;
  }

}
