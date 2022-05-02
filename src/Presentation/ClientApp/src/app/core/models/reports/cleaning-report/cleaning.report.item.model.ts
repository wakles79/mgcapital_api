import { EntityModel } from '../../common/entity.model';
import { CleaningReportItemAttachmentModel } from './cleaning.report.item.attachment.model';

export class CleaningReportItemBaseModel {

  id: number;
  guid: string;
  type: number;
  observances: string;
  time: string;
  location: string;
  buildingId: number;
  buildingName: string;
  cleaningReportId: number;

  attachments: any[];

  constructor(entity: CleaningReportItemBaseModel = null) {
    if (entity) {
      this.id = entity.id;
      this.guid = entity.guid;
      this.type = entity.type;
      this.observances = entity.observances;
      this.time = entity.time || '';
      this.location = entity.location;
      this.buildingId = entity.buildingId;
      this.buildingName = entity.buildingName || '';
      this.cleaningReportId = entity.cleaningReportId;

      this.attachments = entity.attachments;
    }
    else {
      this.id = null;
      this.guid = null;
      this.type = 0;
      this.observances = '';
      this.time = '';
      this.location = '';
      this.buildingId = null;
      this.buildingName = '';
      this.cleaningReportId = null;

      this.attachments = [];
    }

  }
}
