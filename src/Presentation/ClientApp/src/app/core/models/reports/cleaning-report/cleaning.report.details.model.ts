import { CleaningReportItemBaseModel } from './cleaning.report.item.model';
import { CleaningReportCreateModel } from './cleaning.report.create.model';
import { CleaningReportNoteModel } from './cleaning.report.note.model';

export class CleaningReportDetailsModel extends CleaningReportCreateModel {

  cleaningItems: CleaningReportItemBaseModel[];
  findingItems: CleaningReportItemBaseModel[];
  notes: CleaningReportNoteModel[];

  submitted: number;
  formattedTo: string;
  companyName: string;
  from: string;
  to: string;
  toEmail: string;
  guid: string;

  constructor(entity: CleaningReportDetailsModel) {
    super(entity);

    this.cleaningItems = entity.cleaningItems;
    this.findingItems = entity.findingItems;
    this.notes = entity.notes;

    this.submitted = entity.submitted;
    this.formattedTo = entity.formattedTo;
    this.companyName = entity.companyName;
    this.from = entity.from;
    this.to = entity.to;
    this.toEmail = entity.toEmail || '';
  }
}
