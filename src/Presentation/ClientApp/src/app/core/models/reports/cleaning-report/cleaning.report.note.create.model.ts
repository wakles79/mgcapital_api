
export class CleaningReportNoteCreateModel {
  cleaningReportId: number;
  direction: number;
  message: string;

  constructor(entity: CleaningReportNoteCreateModel) {
    this.cleaningReportId = entity.cleaningReportId;
    this.direction = entity.direction;
    this.message = entity.message;
  }
}
