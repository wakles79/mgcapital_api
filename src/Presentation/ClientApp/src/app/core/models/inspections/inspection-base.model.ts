import { CompanyEntity } from '../common/company-entity.model';

export class InspectionBaseModel extends CompanyEntity {

  number: number;
  snoozeDate: Date;
  buildingId: number;
  employeeId: number;
  stars: number;
  dueDate: number;
  closeDate: number;
  beginNotes: string;
  closingNotes: string;
  score: number;
  status: number;
  allowPublicView: boolean;
  epochSnoozeDate: number;

  constructor(inspectionModel: InspectionBaseModel) {
    super(inspectionModel);

    this.number = inspectionModel.number || 0;
    this.snoozeDate = inspectionModel.snoozeDate || null;
    this.buildingId = inspectionModel.buildingId || 0;
    this.employeeId = inspectionModel.employeeId || 0;
    this.stars = inspectionModel.stars || 0;
    this.dueDate = inspectionModel.dueDate || null;
    this.closeDate = inspectionModel.closeDate || null;
    this.beginNotes = inspectionModel.beginNotes || '';
    this.closingNotes = inspectionModel.closingNotes || '';
    this.status = inspectionModel.status || 0;
    this.allowPublicView = inspectionModel.allowPublicView || false;
    this.epochSnoozeDate = inspectionModel.epochSnoozeDate || 0;
  }

}

export enum InspectionStatus {
  Pending = 0,
  Scheduled = 1,
  Walkthrough = 2,
  WalkthroughComplete = 3,
  Active = 4,
  Closed = 5
}
