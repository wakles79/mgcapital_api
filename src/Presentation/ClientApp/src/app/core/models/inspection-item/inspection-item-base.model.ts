import { CompanyEntity } from '../common/company-entity.model';

export class InspectionItemBaseModel extends CompanyEntity {

  position: string;
  description: string;
  latitude: string;
  longitude: string;
  InspectionId: number;
  attachments: any[];
  tasks: any[];
  number: number;
  priority: number;
  type: number;
  notes: any[];
  status: number;

  constructor(inspectionItemModel: InspectionItemBaseModel) {
    super(inspectionItemModel);
    if (inspectionItemModel) {

      this.position = inspectionItemModel.position;
      this.latitude = inspectionItemModel.latitude || '';
      this.longitude = inspectionItemModel.longitude;
      this.description = inspectionItemModel.description;
      this.InspectionId = inspectionItemModel.InspectionId;
      this.attachments = inspectionItemModel.attachments;
      this.tasks = inspectionItemModel.tasks;
      this.number = inspectionItemModel.number || 0;
      this.type = inspectionItemModel.type || 0;
      this.priority = inspectionItemModel.priority || 0;
      this.status = inspectionItemModel.status || 0;
      this.notes = inspectionItemModel.notes || null;
    }
    else {

      this.position = '';
      this.latitude = '';
      this.longitude = '';
      this.description = null;
      this.InspectionId = null;
      this.attachments = null;
      this.tasks = null;
      this.number = null;
      this.type = 0;
      this.priority = 0;
      this.status = 0;
      this.notes = null;
    }
  }
}
