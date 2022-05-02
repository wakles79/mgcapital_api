import { EntityModel } from '../../common/entity.model';

export class CleaningReportCreateModel extends EntityModel {

  contactId: number;
  employeeId: number;
  location: string;
  dateOfService: Date;
  epochDateOfService: number;

  constructor(entity: CleaningReportCreateModel) {
    super(entity);

    this.contactId = entity.contactId;
    this.employeeId = entity.employeeId;
    this.location = entity.location;
    this.dateOfService = entity.dateOfService;
    this.epochDateOfService = entity.epochDateOfService || 0;
  }
}
