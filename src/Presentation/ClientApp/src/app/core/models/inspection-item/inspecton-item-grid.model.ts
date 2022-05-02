import { InspectionItemBaseModel } from './inspection-item-base.model';


export class InspectionItemGridModel extends InspectionItemBaseModel {

  createdDate: number;
  statusName: string;
  epochDueDate: number;
  epochCloseDate: number;

  constructor(inspectionItemGridModel: InspectionItemGridModel) {
    super(inspectionItemGridModel);

    this.createdDate = inspectionItemGridModel.createdDate || 0;
    this.statusName = inspectionItemGridModel.statusName || '';
    this.epochDueDate = inspectionItemGridModel.epochDueDate || 0;
    this.epochCloseDate = inspectionItemGridModel.epochCloseDate || 0;
  }

}
