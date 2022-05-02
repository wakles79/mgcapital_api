import { CompanyEntity } from '@core/models/common/company-entity.model';

export class InspectionNoteModel extends CompanyEntity {
  note: string;
  inspectionId: number;
  employeeId: number;
  createdDate: Date;
  epochCreatedDate: number;
  updatedDate: Date;
  epochUpdatedDate: number;
  employeeEmail: string;
  employeeFullName: string;

  constructor(iNote: InspectionNoteModel) {
    super(iNote);
    this.note = iNote.note || '';

    // Relational fields
    this.inspectionId = iNote.inspectionId || 0;
    this.employeeId = iNote.employeeId || 0;
    this.employeeEmail = iNote.employeeEmail || '';
    this.employeeFullName = iNote.employeeFullName || '';

    this.epochCreatedDate = iNote.epochCreatedDate;
    this.epochUpdatedDate = iNote.epochUpdatedDate;
  }
}