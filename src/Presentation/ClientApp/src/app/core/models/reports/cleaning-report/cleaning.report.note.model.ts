import { EntityModel } from "../../common/entity.model";


export class CleaningReportNoteModel extends EntityModel {
  cleaningReportId: number;
  epochCreatedDate: number;
  direction: number; // incoming (-1) or outgoing (1)
  senderName: string;
  message: string;

  constructor(entity: CleaningReportNoteModel) {
    super(entity);

    this.cleaningReportId = entity.cleaningReportId;
    this.epochCreatedDate = entity.epochCreatedDate;
    this.direction = entity.direction;
    this.senderName = entity.senderName;
    this.message = entity.message;
  }
}
