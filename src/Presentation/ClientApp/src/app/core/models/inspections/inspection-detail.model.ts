import { InspectionBaseModel } from './inspection-base.model';

export class InspectionDetailModel extends InspectionBaseModel {

  buildingName: string;
  employeeName: string;
  employeeEmail: string;
  employeePhone: string;
  statusName: string;
  inspectionItem: any[];
  InspectionNote: any[];

  constructor(inspectionDetail: InspectionDetailModel) {
    super(inspectionDetail);

    this.buildingName = inspectionDetail.buildingName || '';
    this.employeeName = inspectionDetail.employeeName || '';
    this.employeeEmail = inspectionDetail.employeeEmail || '';
    this.employeePhone = inspectionDetail.employeePhone || '';
    this.statusName = inspectionDetail.statusName || '';
    this.inspectionItem = inspectionDetail.inspectionItem || null;
    this.InspectionNote = inspectionDetail.InspectionNote || null;
  }

}
