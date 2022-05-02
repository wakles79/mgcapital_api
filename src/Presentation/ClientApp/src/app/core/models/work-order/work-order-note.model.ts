import { CompanyEntity } from '@core/models/common/company-entity.model';

export class WorkOrderNoteModel extends CompanyEntity {
  note: string;
  workOrderId: number;
  employeeId: number;
  createdDate: Date;
  epochCreatedDate: number;
  updatedDate: Date;
  epochUpdatedDate: number;
  employeeEmail: string;
  employeeFullName: string;

  constructor(woNote: WorkOrderNoteModel) {
    super(woNote);
    this.note = woNote.note || '';

    // Relational fields
    this.workOrderId = woNote.workOrderId || 0;
    this.employeeId = woNote.employeeId || 0;
    this.employeeEmail = woNote.employeeEmail || '';
    this.employeeFullName = woNote.employeeFullName || '';

    this.epochCreatedDate = woNote.epochCreatedDate;
    this.epochUpdatedDate = woNote.epochUpdatedDate;
  }
}
